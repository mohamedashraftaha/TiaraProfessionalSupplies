using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;
using TiaraPro.Server.DTOs;

namespace TiaraPro.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly TiaraDbContext _context;
        public EventsController(TiaraDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEvents()
        {
            var events = await _context.Events.OrderByDescending(e => e.Date).ToListAsync();
            return events.Select(e => ToDTO(e)).ToList();
        }

        // GET: api/events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDTO>> GetEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            return ToDTO(ev);
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<EventDTO>> CreateEvent(EventDTO dto)
        {
            try
            {
                if (!IsAdmin()) return Forbid();
                // Backend validation
                if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description) || dto.Date == default || dto.Date < DateTime.UtcNow.AddMinutes(-5))
                {
                    return BadRequest("Title, Description, and a valid future Date are required.");
                }
                DateTime eventDate = dto.Date;
                if (eventDate.Kind == DateTimeKind.Unspecified)
                {
                    eventDate = DateTime.SpecifyKind(eventDate, DateTimeKind.Utc);
                }
                else if (eventDate.Kind == DateTimeKind.Local)
                {
                    eventDate = eventDate.ToUniversalTime();
                }

                // Use eventDate instead of dto.Date below
                var ev = new Event
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Speakers = dto.Speakers,
                    Date = eventDate,
                    Location = dto.Location,
                    ImageUrl = dto.ImageUrl,
                    Capacity = dto.Capacity,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Events.Add(ev);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetEvent), new { id = ev.Id }, ToDTO(ev));


            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventDTO dto)
        {
            if (!IsAdmin()) return Forbid();
            // Backend validation
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description) || dto.Date == default || dto.Date < DateTime.UtcNow.AddMinutes(-5))
            {
                return BadRequest("Title, Description, and a valid future Date are required.");
            }
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            ev.Title = dto.Title;
            ev.Description = dto.Description;
            ev.Speakers = dto.Speakers;
            ev.Date = dto.Date;
            ev.Location = dto.Location;
            ev.ImageUrl = dto.ImageUrl;
            ev.Capacity = dto.Capacity;
            ev.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!IsAdmin()) return Forbid();
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/events/{eventId}/register?userId=123
        [HttpPost("{eventId}/register")]
        public async Task<IActionResult> RegisterUser(int eventId, [FromQuery] int userId)
        {
            var ev = await _context.Events.FindAsync(eventId);
            var user = await _context.Users.FindAsync(userId);
            if (ev == null || user == null) return NotFound("Event or user not found.");
            if (await _context.EventRegistrations.AnyAsync(r => r.EventId == eventId && r.UserId == userId))
                return BadRequest("User already registered for this event.");
            if (ev.Capacity.HasValue)
            {
                var regCount = await _context.EventRegistrations.CountAsync(r => r.EventId == eventId);
                if (regCount >= ev.Capacity.Value)
                    return BadRequest("Event is full.");
            }
            var reg = new EventRegistration { EventId = eventId, UserId = userId, RegisteredAt = DateTime.UtcNow };
            _context.EventRegistrations.Add(reg);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/events/{eventId}/register?userId=123
        [HttpDelete("{eventId}/register")]
        public async Task<IActionResult> UnregisterUser(int eventId, [FromQuery] int userId)
        {
            var reg = await _context.EventRegistrations.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
            if (reg == null) return NotFound("Registration not found.");
            _context.EventRegistrations.Remove(reg);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/events/{eventId}/registrations
        [HttpGet("{eventId}/registrations")]
        public async Task<IActionResult> GetRegistrations(int eventId)
        {
            if (!IsAdmin()) return Forbid();
            var regs = await _context.EventRegistrations
                .Where(r => r.EventId == eventId)
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

        private static EventDTO ToDTO(Event e) => new EventDTO
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Speakers = e.Speakers,
            Date = e.Date,
            Location = e.Location,
            ImageUrl = e.ImageUrl,
            Capacity = e.Capacity,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        };

        // Dummy admin check (replace with real auth logic)
        private bool IsAdmin()
        {
            // TODO: Replace with real authentication/authorization
            // For now, always allow (for development)
            return true;
        }
    }
} 