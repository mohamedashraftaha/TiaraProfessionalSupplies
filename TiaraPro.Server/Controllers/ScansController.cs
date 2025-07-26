using TiaraPro.Server.PersistenceLayer.UnitOfWork;
using TiaraPro.Server.Services.AwsS3;
using TiaraPro.Server.Services.DentalMeshAI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace TiaraPro.Server.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ScansController : ControllerBase
{
    private readonly IDentalMeshAI _dentalMeshAI;
    private readonly IAWSS3Service _awsS3Service;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ScansController> _logger;
    public ScansController(IDentalMeshAI dentalMeshAI, IAWSS3Service awsS3Service, IUnitOfWork unitOfWork, ILogger<ScansController> logger)
    {
        _dentalMeshAI = dentalMeshAI;
        _awsS3Service = awsS3Service;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email not found in token");
            }
            // Backend file extension validation
            var allowedExtensions = new[] { ".nii", ".nii.gz", ".dcm", ".nrrd", ".zip" };
            var fileName = file?.FileName?.ToLower() ?? string.Empty;
            bool isAllowed = allowedExtensions.Any(ext => fileName.EndsWith(ext));
            if (!isAllowed)
            {
                return BadRequest("Unsupported file format. Please upload .nii, .nii.gz, .dcm, .nrrd, or ZIPs of DICOMs.");
            }
            var result = await _dentalMeshAI.UploadFileAsync(email, file);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, "Internal server error");
        }
    }

} 