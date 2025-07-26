using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;
using TiaraPro.Server.DTOs;

namespace TiaraPro.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DentalTrainingController : ControllerBase
    {
        private readonly TiaraDbContext _context;
        public DentalTrainingController(TiaraDbContext context)
        {
            _context = context;
        }

        // GET: api/dentaltraining
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DentalTrainingDTO>>> GetDentalTrainings()
        {
            var trainings = await _context.DentalTrainings
                .Where(dt => !string.IsNullOrEmpty(dt.Title) && !string.IsNullOrEmpty(dt.Description))
                .OrderByDescending(dt => dt.Date)
                .ToListAsync();
            return trainings.Select(dt => ToDTO(dt)).ToList();
        }

        // GET: api/dentaltraining/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DentalTrainingDTO>> GetDentalTraining(int id)
        {
            var training = await _context.DentalTrainings.FindAsync(id);
            if (training == null) return NotFound();
            return ToDTO(training);
        }

        // POST: api/dentaltraining
        [HttpPost]
        public async Task<ActionResult<DentalTrainingDTO>> CreateDentalTraining(DentalTrainingDTO dto)
        {
            try
            {
                if (!IsAdmin()) return Forbid();
                // Backend validation
                if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description) || dto.Date == default || dto.Date < DateTime.UtcNow.AddMinutes(-5))
                {
                    return BadRequest("Title, Description, and a valid future Date are required.");
                }
                if (dto.Packages == null || dto.Packages.Count == 0)
                {
                    return BadRequest("At least one package is required for a dental training.");
                }
                DateTime trainingDate = dto.Date;
                if (trainingDate.Kind == DateTimeKind.Unspecified)
                {
                    trainingDate = DateTime.SpecifyKind(trainingDate, DateTimeKind.Utc);
                }
                else if (trainingDate.Kind == DateTimeKind.Local)
                {
                    trainingDate = trainingDate.ToUniversalTime();
                }

                var training = new DentalTraining
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Instructors = dto.Instructors,
                    Date = trainingDate,
                    Location = dto.Location,
                    ImageUrl = dto.ImageUrl,
                    Capacity = dto.Capacity,
                    CreatedAt = DateTime.UtcNow
                };
                _context.DentalTrainings.Add(training);
                await _context.SaveChangesAsync();

                // Add packages
                foreach (var pkg in dto.Packages)
                {
                    var package = new DentalTrainingPackage
                    {
                        DentalTrainingId = training.Id,
                        Name = pkg.Name,
                        Price = pkg.Price
                    };
                    _context.DentalTrainingPackages.Add(package);
                }
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDentalTraining), new { id = training.Id }, ToDTO(training));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/dentaltraining/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDentalTraining(int id, DentalTrainingDTO dto)
        {
            if (!IsAdmin()) return Forbid();
            // Backend validation
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description) || dto.Date == default || dto.Date < DateTime.UtcNow.AddMinutes(-5))
            {
                return BadRequest("Title, Description, and a valid future Date are required.");
            }
            var training = await _context.DentalTrainings.FindAsync(id);
            if (training == null) return NotFound();
            training.Title = dto.Title;
            training.Description = dto.Description;
            training.Instructors = dto.Instructors;
            training.Date = dto.Date;
            training.Location = dto.Location;
            training.ImageUrl = dto.ImageUrl;
            training.Capacity = dto.Capacity;
            training.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/dentaltraining/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDentalTraining(int id)
        {
            if (!IsAdmin()) return Forbid();
            var training = await _context.DentalTrainings.FindAsync(id);
            if (training == null) return NotFound();
            _context.DentalTrainings.Remove(training);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/dentaltraining/{trainingId}/register?userId=123
        [HttpPost("{trainingId}/register/{userId}/{orderId}")]
        public async Task<IActionResult> RegisterUser(int trainingId, int userId, int orderId)
        {
            var training = await _context.DentalTrainings.FindAsync(trainingId);
            var user = await _context.Users.FindAsync(userId);
            if (training == null || user == null) return NotFound("Training or user not found.");
            if (await _context.DentalTrainingRegistrations.AnyAsync(r => r.DentalTrainingId == trainingId && r.UserId == userId))
                return BadRequest("User already registered for this training.");
            if (training.Capacity.HasValue)
            {
                var regCount = await _context.DentalTrainingRegistrations.CountAsync(r => r.DentalTrainingId == trainingId);
                if (regCount >= training.Capacity.Value)
                    return BadRequest("Training is full.");
            }
            var reg = new DentalTrainingRegistration { DentalTrainingId = trainingId, UserId = userId, RegisteredAt = DateTime.UtcNow, OrderId = orderId, Confirmed = false };
            _context.DentalTrainingRegistrations.Add(reg);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/dentaltraining/{trainingId}/register?userId=123
        [HttpDelete("{trainingId}/register")]
        public async Task<IActionResult> UnregisterUser(int trainingId, [FromQuery] int userId)
        {
            var reg = await _context.DentalTrainingRegistrations.FirstOrDefaultAsync(r => r.DentalTrainingId == trainingId && r.UserId == userId);
            if (reg == null) return NotFound("Registration not found.");
            _context.DentalTrainingRegistrations.Remove(reg);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/dentaltraining/{trainingId}/registrations
        [HttpGet("{trainingId}/registrations")]
        public async Task<IActionResult> GetRegistrations(int trainingId)
        {
            if (!IsAdmin()) return Forbid();
            var regs = await _context.DentalTrainingRegistrations
                .Where(r => r.DentalTrainingId == trainingId)
                .Include(r => r.User)
                .Select(r => new {
                    r.UserId,
                    r.User.FirstName,
                    r.User.LastName,
                    r.User.Email,
                    r.RegisteredAt
                })
                .ToListAsync();
            return Ok(regs);
        }

        // GET: api/dentaltraining/{trainingId}/is-registered/{userId}
        [HttpGet("{trainingId}/is-registered/{userId}")]
        public async Task<IActionResult> IsUserRegistered(int trainingId, int userId)
        {
            var isRegistered = await _context.DentalTrainingRegistrations.AnyAsync(r => r.DentalTrainingId == trainingId && r.UserId == userId);
            return Ok(new { registered = isRegistered });
        }

        private static DentalTrainingDTO ToDTO(DentalTraining dt) => new DentalTrainingDTO
        {
            Id = dt.Id,
            Title = dt.Title,
            Description = dt.Description,
            Instructors = dt.Instructors,
            Date = dt.Date,
            Location = dt.Location,
            ImageUrl = dt.ImageUrl,
            Capacity = dt.Capacity,
            CreatedAt = dt.CreatedAt,
            UpdatedAt = dt.UpdatedAt
        };

        // DELETE: api/dentaltraining/cleanup-invalid
        [HttpDelete("cleanup-invalid")]
        public async Task<IActionResult> CleanupInvalidRecords()
        {
            if (!IsAdmin()) return Forbid();
            try
            {
                var invalidTrainings = await _context.DentalTrainings
                    .Where(dt => string.IsNullOrEmpty(dt.Title) || string.IsNullOrEmpty(dt.Description))
                    .ToListAsync();
                
                if (invalidTrainings.Any())
                {
                    _context.DentalTrainings.RemoveRange(invalidTrainings);
                    await _context.SaveChangesAsync();
                    return Ok($"Removed {invalidTrainings.Count} invalid training records.");
                }
                
                return Ok("No invalid records found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error cleaning up invalid records.");
            }
        }

        // Dummy admin check (replace with real auth logic)
        private bool IsAdmin()
        {
            // TODO: Replace with real authentication/authorization
            // For now, always allow (for development)
            return true;
        }
    }
} 