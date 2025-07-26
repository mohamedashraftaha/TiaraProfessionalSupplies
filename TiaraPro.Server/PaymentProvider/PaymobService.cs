using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using TiaraPro.Server.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace TiaraPro.Server.PaymentProvider;

public class PaymobService : IPaymobService
{
    private readonly HttpClient _httpClient;
    private readonly PaymobOptions _paymobOptions;
    public PaymobService(HttpClient httpClient, IOptions<PaymobOptions> options)
    {
        _paymobOptions = options.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://accept.paymob.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _paymobOptions.SecretKey);
    }
    public async Task<PaymobIntentionResponse> CreateIntentionAsync(PaymobIntentionRequest request)
    {
        var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("v1/intention/", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Paymob error: {response.StatusCode} - {error}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PaymobIntentionResponse>(responseContent);
    }
   
}
