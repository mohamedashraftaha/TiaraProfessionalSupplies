using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiaraPro.Server.DTOs;
using TiaraPro.Server.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using TiaraPro.Server.Services.PromoCodes;

namespace TiaraPro.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromoCodeController : ControllerBase
    {
        private readonly IPromoCodeService _promoCodeService;

        public PromoCodeController(IPromoCodeService promoCodeService)
        {
            _promoCodeService = promoCodeService;
        }

        [HttpPost("validate")]
        public async Task<ActionResult<ValidatePromoCodeResponse>> ValidatePromoCode([FromBody] ValidatePromoCodeRequest request)
        {
            var response = await _promoCodeService.ValidatePromoCodeAsync(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<PromoCodeResponse>> CreatePromoCode([FromBody] PromoCode promoCode)
        {
            try
            {
                var response = await _promoCodeService.CreatePromoCodeAsync(promoCode);
                return Ok(response);


            }
            catch(Exception Ex)
            {
                return StatusCode(500,"Something Went wrong.");
            }
        }

        [HttpPost("createUserUsage")]
        public async Task<IActionResult> CreateUserPromoCodeUsage(UserPromoCodeUsage userPromoCodeUsage)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var response = await _promoCodeService.CreateUserPromoCodeAsync(userPromoCodeUsage);
                if (!response)
                {
                    return BadRequest();
                }
                return Ok(response);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
                return NotFound();

            }
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<PromoCodeResponse>> GetPromoCode(string code)
        {
            var response = await _promoCodeService.GetPromoCodeAsync(code);
            if (response == null)
                return NotFound();
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromoCodeResponse>>> GetAllPromoCodes()
        {
            var promoCodes = await _promoCodeService.GetAllPromoCodesAsync();
            return Ok(promoCodes);
        }
    }
} 