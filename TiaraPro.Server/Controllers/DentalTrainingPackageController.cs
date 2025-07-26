using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;
using TiaraPro.Server.DTOs;

namespace TiaraPro.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DentalTrainingPackageController : ControllerBase
    {
        private readonly TiaraDbContext _context;
        public DentalTrainingPackageController(TiaraDbContext context)
        {
            _context = context;
        }

        // GET: api/dentaltrainingpackage/{trainingId}
        [HttpGet("{trainingId}")]
        public async Task<ActionResult<IEnumerable<DentalTrainingPackageDTO>>> GetPackages(int trainingId)
        {
            var packages = await _context.DentalTrainingPackages
                .Where(p => p.DentalTrainingId == trainingId)
                .ToListAsync();
            return packages.Select(p => ToDTO(p)).ToList();
        }

        // POST: api/dentaltrainingpackage
        [HttpPost]
        public async Task<ActionResult<DentalTrainingPackageDTO>> AddPackage(DentalTrainingPackageDTO dto)
        {
            if (!IsAdmin()) return Forbid();
            var package = new DentalTrainingPackage
            {
                DentalTrainingId = dto.DentalTrainingId,
                Name = dto.Name,
                Price = dto.Price
            };
            _context.DentalTrainingPackages.Add(package);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPackages), new { trainingId = package.DentalTrainingId }, ToDTO(package));
        }

        // PUT: api/dentaltrainingpackage/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPackage(int id, DentalTrainingPackageDTO dto)
        {
            if (!IsAdmin()) return Forbid();
            var package = await _context.DentalTrainingPackages.FindAsync(id);
            if (package == null) return NotFound();
            package.Name = dto.Name;
            package.Price = dto.Price;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/dentaltrainingpackage/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            if (!IsAdmin()) return Forbid();
            var package = await _context.DentalTrainingPackages.FindAsync(id);
            if (package == null) return NotFound();
            _context.DentalTrainingPackages.Remove(package);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static DentalTrainingPackageDTO ToDTO(DentalTrainingPackage p) => new DentalTrainingPackageDTO
        {
            Id = p.Id,
            DentalTrainingId = p.DentalTrainingId,
            Name = p.Name,
            Price = p.Price
        };

        // Dummy admin check (replace with real auth logic)
        private bool IsAdmin() => true;
    }
} 