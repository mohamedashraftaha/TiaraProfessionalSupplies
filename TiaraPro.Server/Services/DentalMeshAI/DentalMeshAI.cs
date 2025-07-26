using TiaraPro.Server.PersistenceLayer.UnitOfWork;
using TiaraPro.Server.Models.DentalMesh;
using TiaraPro.Server.Services.AwsS3;
using TiaraPro.Server.Services.EmailService;
using TiaraPro.Server.Services.ScanTransaction;
using TiaraPro.Server.Utils;
using Newtonsoft.Json;
using System.Text;
using TiaraPro.Server.Services.TiaraAI;

namespace TiaraPro.Server.Services.DentalMeshAI
{
    public class DentalMeshAI : IDentalMeshAI
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly ILogger<DentalMeshAI> _logger;
        private readonly IAWSS3Service _awsS3Service;
        private readonly IScanTransaction _scanTransaction;
        private readonly IEmailHandler _emailHandler;
        private readonly ITiaraAISubscriptionService _tiaraAISubscriptionService;
        private readonly IUnitOfWork _unitOfWork;
        public DentalMeshAI(IConfiguration config, ILogger<DentalMeshAI> logger, IAWSS3Service awsS3Service, IScanTransaction scanTransaction, IEmailHandler emailHandler
            , IUnitOfWork unitOfWork, ITiaraAISubscriptionService tiaraAISubscriptionService)
        {
            _httpClient = new HttpClient();
            _apiKey = config["DentalMesh:ApiKey"]!;
            _baseUrl = config["DentalMesh:BaseUrl"]!;
            _logger = logger;
            _awsS3Service = awsS3Service;
            _scanTransaction = scanTransaction;
            _emailHandler = emailHandler;
            _unitOfWork = unitOfWork;
            _tiaraAISubscriptionService = tiaraAISubscriptionService;
        }

        public async Task<string> UploadFileAsync(string email, IFormFile file)
        {
            try
            {
                var result = string.Empty;
                var preSignedUrl = string.Empty;
                var publicUrl = string.Empty;
                var dentalMeshResponse = new DentalMeshResponseModel();
                if (file.Length == 0)
                {
                    string errorMessage = "File is empty";
                    Exception ex = new Exception(errorMessage);
                    throw ex;
                }

                #region Get S3 Bucket URLs
                var txnGuid = Guid.NewGuid().ToString();
                (publicUrl, preSignedUrl) = await _awsS3Service.UploadFileAsync(file, txnGuid);
                if (string.IsNullOrEmpty(preSignedUrl) || string.IsNullOrEmpty(publicUrl))
                {
                    _logger.LogError("Error uploading file to S3");
                    return "Error uploading file to S3";
                }
                #endregion

                #region Make DentalMesh API Call
                var url = $"{_baseUrl}/static_token/cbct_task";
                var dentalMeshRequest = new DentalMeshRequestModel
                {
                    Guid = txnGuid,
                    Url = preSignedUrl,
                };
                _logger.LogInformation("PreSigned URL {URL}", preSignedUrl);
                await _emailHandler.SendEmailAsync(
                    email,
                    "EMRA CBCT Scan - Currently Processing",
                    EmailContentGenerator.GenerateStatusUpdateEmail(email, txnGuid),
                    "sales@tiarapro.com"
                );
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new LowercaseContractResolver(), // Use custom resolver
                    Formatting = Formatting.Indented // Optional, for pretty printing
                };

                string jsonContent = JsonConvert.SerializeObject(dentalMeshRequest, settings);

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpContent
                };

                request.Headers.Add("token", _apiKey);

                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();


                result = await response.Content.ReadAsStringAsync();

                try
                {
                    dentalMeshResponse = JsonConvert.DeserializeObject<DentalMeshResponseModel>(result);
                    if (string.IsNullOrEmpty(dentalMeshResponse?.Guid))
                    {
                        _logger.LogError("Error deserializing response: Result is null.");
                        throw new InvalidOperationException("Failed to deserialize response to DentalMeshResponseModel.");
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing DentalMesh response");
                }
                #endregion
                #region Record Transaction in DB
                var recordToDbSuccessful = await _scanTransaction.AddUserTransaction(email, dentalMeshResponse?.Guid!, publicUrl, "running");
                if (!recordToDbSuccessful)
                {
                    _logger.LogError("Error recording transaction in DB");
                    return "Error recording transaction in DB";
                }
                #endregion

                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                throw;
            }
        }

        public async Task ProcessPendingTransactionsAsync()
        {
            try
            {
                #region Get all transactions with pending statuses
                var allTransactions = await _unitOfWork.Transactions.GetAllAsync();
                var pendingTransactions = allTransactions.Where(t => t.Status == "pending" || t.Status == "running").ToList();

                if (pendingTransactions.Count == 0)
                {
                    _logger.LogInformation("No pending transactions to process.");
                    return;
                }

                foreach (var transaction in pendingTransactions)
                {
                    _logger.LogInformation("Processing Transactions with {Guid}", transaction.TransactionGuid);
                    await CheckStatus(transaction.TransactionGuid.ToString());
                }
                #endregion

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending transactions.");
            }
        }

        private async Task CheckStatus(string guid)
        {
            try
            {
                var viewLink = string.Empty;
                var shortStlViewLink = string.Empty;
                var dentalMeshStatusResponse = new DentalMeshStatusResponseModel();
                var url = $"{_baseUrl}/static_token/cbct_task/{guid}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Add("accept", "application/json");
                request.Headers.Add("token", _apiKey);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {

                    _logger.LogError("Error: Received unsuccessful status code {StatusCode} for GUID: {Guid}", response.StatusCode, guid);
                    throw new InvalidOperationException($"Request failed with status code: {response.StatusCode}");
                }

                var result = await response.Content.ReadAsStringAsync();

                try
                {
                    dentalMeshStatusResponse = JsonConvert.DeserializeObject<DentalMeshStatusResponseModel>(result);

                    if (dentalMeshStatusResponse == null)
                    {
                        _logger.LogError("Error deserializing response: Result is null.");
                        throw new InvalidOperationException("Failed to deserialize response to DentalMeshStatusResponseModel.");
                    }
                    _logger.LogInformation(JsonConvert.SerializeObject(dentalMeshStatusResponse));
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing DentalMesh status response");
                    throw new InvalidOperationException("Error deserializing the DentalMesh response.", ex);
                }

                if (dentalMeshStatusResponse?.Status != null)
                {
                    _logger.LogInformation("Received {Status}", dentalMeshStatusResponse.Status);
                    if (dentalMeshStatusResponse?.Status != "pending" && dentalMeshStatusResponse?.Status != "running")
                    {
                        _logger.LogInformation("Received {Status}", dentalMeshStatusResponse!.Status);
                        if (dentalMeshStatusResponse?.SignedDownloadUrl != null && !string.IsNullOrEmpty(dentalMeshStatusResponse?.SignedDownloadUrl))
                        {
                            viewLink = dentalMeshStatusResponse.SignedDownloadUrl;

                            shortStlViewLink = dentalMeshStatusResponse.ShortViewerUrl;

                        }
                        await HandleProcessingResult(dentalMeshStatusResponse!.Status, guid, viewLink, shortStlViewLink);

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking status for GUID: {Guid}, {ExceptionMessage}", guid, ex.Message);
                throw;
            }
        }

        private async Task HandleProcessingResult(string status, string guid, string download_url = "", string shortStlViewLink = "")
        {
            try
            {
                var txn = await _unitOfWork.Transactions.GetByTransactionGuidAsync(guid);
                var user = await _unitOfWork.Users.GetByIdAsync(txn.UserId);
                var email = user?.Email;
                try
                {
                    if (txn != null)
                    {
                        await _unitOfWork.BeginTransactionAsync();
                        txn.Status = status;
                        txn.DentalMeshResponseStlFolder = download_url;
                        txn.DentalMeshResponseStlViewUrl = shortStlViewLink;
                        _unitOfWork.Transactions.Update(txn);
                        await _unitOfWork.CompleteAsync();
                    }

                    if (status == "success")
                    {
                        await _tiaraAISubscriptionService.UpdateUserSubscriptionUsageByEmailAsync(email!);

                    }
                    string? emailBody = status switch
                    {
                        "error" => EmailContentGenerator.GenerateErrorEmail(guid),
                        "success" => EmailContentGenerator.GenerateSuccessEmail(guid, download_url, shortStlViewLink),
                        _ => null
                    };
                    if (!string.IsNullOrEmpty(email))
                    {
                        _logger.LogInformation("Sending email to {Email}", email);
                        _logger.LogInformation("Status {Status}", status);
                        await _emailHandler.SendEmailAsync(
                             email,
                             "EMRA CBCT Scan - Result",
                             emailBody!,
                             "sales@tiarapro.com"
                         );
                    }


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,ex.Message);
                    await _unitOfWork.RollbackAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                throw;
            }
        }

    }
}
