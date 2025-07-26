using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly TiaraDbContext _context;

        public ContentController(TiaraDbContext context)
        {
            _context = context;
        }

        [HttpGet("{page}")]
        public async Task<IActionResult> GetContent(string page)
        {
            var content = await _context.Contents
                .Where(c => c.Page == page)
                .ToListAsync();
            return Ok(content);
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<IActionResult> UpdateContent([FromBody] ContentUpdateDto dto)
        {
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin)
                return Forbid();

            var content = await _context.Contents
                .FirstOrDefaultAsync(c => c.Page == dto.Page && c.Key == dto.Key);

            if (content == null)
            {
                content = new Content { Page = dto.Page, Key = dto.Key, Value = dto.Value };
                _context.Contents.Add(content);
            }
            else
            {
                content.Value = dto.Value;
            }

            await _context.SaveChangesAsync();
            return Ok(content);
        }

        [HttpGet("is-admin")]
        [Authorize]
        public IActionResult IsAdmin()
        {
            var isAdmin = User.IsInRole("User");
            return Ok(new { isAdmin });
        }
    }

    public class ContentUpdateDto
    {
        public string Page { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
} 