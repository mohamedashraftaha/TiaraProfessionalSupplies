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
    public async Task<bool> CreateTrainingAsync(DentalTraining training)
    {
        if (training == null || string.IsNullOrWhiteSpace(training.Title) || string.IsNullOrWhiteSpace(training.Description) || training.Date == default || training.Date < DateTime.UtcNow.AddMinutes(-5))
        {
            return false;
        }
        _context.DentalTrainings.Add(training);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTrainingAsync(DentalTraining training)
    {
        if (training == null || string.IsNullOrWhiteSpace(training.Title) || string.IsNullOrWhiteSpace(training.Description) || training.Date == default || training.Date < DateTime.UtcNow.AddMinutes(-5))
        {
            return false;
        }
        _context.DentalTrainings.Update(training);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTrainingAsync(int id)
    {
        var training = await _context.DentalTrainings.FindAsync(id);
        if (training == null) return false;
        _context.DentalTrainings.Remove(training);
        return await _context.SaveChangesAsync() > 0;
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

    public async Task<bool> ConfirmRegistrationAsync(int orderId, int userId)
    {

        var registration = await _context.DentalTrainingRegistrations.FirstOrDefaultAsync(dtr => dtr.OrderId == orderId && dtr.UserId == userId);
        if (registration == null) return false;
        registration.Confirmed = true;
        _context.DentalTrainingRegistrations.Update(registration);
        return await _context.SaveChangesAsync() > 0;
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
