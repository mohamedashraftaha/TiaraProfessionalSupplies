using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.TiaraDentalTraining;

public class DentalTrainingRepository : IDentalTrainingRepository
{
    private readonly TiaraDbContext _context;
    public DentalTrainingRepository(TiaraDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<DentalTraining>> GetAllTrainingsAsync()
    {
        return await _context.DentalTrainings
            .Where(dt => !string.IsNullOrEmpty(dt.Title) && !string.IsNullOrEmpty(dt.Description))
            .OrderByDescending(dt => dt.Date)
            .ToListAsync();
    }
    public async Task<DentalTraining?> GetTrainingByIdAsync(int id)
    {
        return await _context.DentalTrainings.FindAsync(id);
    }
    public async Task CreateTrainingAsync(DentalTraining training)
    {
        _context.DentalTrainings.Add(training);
    }

    public async Task UpdateTrainingAsync(DentalTraining training)
    {
        _context.DentalTrainings.Update(training);
    }

    public async Task DeleteTrainingAsync(int id)
    {
        var training = await _context.DentalTrainings.FindAsync(id);
        _context.DentalTrainings.Remove(training);
    }

    public async Task<IEnumerable<DentalTrainingRegistration>> GetRegistrationsByUserIdAsync(int userId)
    {
        return await _context.DentalTrainingRegistrations
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<DentalTrainingRegistration?> GetRegistrationByIdAsync(int registrationId)
    {
        return await _context.DentalTrainingRegistrations.FindAsync(registrationId);
    }

    public async Task ConfirmRegistrationAsync(int orderId, int userId)
    {

        var registration = await _context.DentalTrainingRegistrations.FirstOrDefaultAsync(dtr => dtr.OrderId == orderId && dtr.UserId == userId);
        registration.Confirmed = true;
        _context.DentalTrainingRegistrations.Update(registration);
    }
    public async Task<bool> UnregisterUserAsync(int trainingId, int userId)
    {
        var registration = await _context.DentalTrainingRegistrations
            .FirstOrDefaultAsync(r => r.DentalTrainingId == trainingId && r.UserId == userId);
        if (registration == null) return false;
        _context.DentalTrainingRegistrations.Remove(registration);
        return await _context.SaveChangesAsync() > 0;
    }

}
