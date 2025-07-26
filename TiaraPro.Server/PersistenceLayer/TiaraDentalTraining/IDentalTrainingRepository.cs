using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.TiaraDentalTraining;

public interface IDentalTrainingRepository
{
    Task<IEnumerable<DentalTraining>> GetAllTrainingsAsync();
    Task<DentalTraining?> GetTrainingByIdAsync(int id);
    Task<bool> CreateTrainingAsync(DentalTraining training);
    Task<bool> UpdateTrainingAsync(DentalTraining training);
    Task<bool> DeleteTrainingAsync(int id);
    Task<IEnumerable<DentalTrainingRegistration>> GetRegistrationsByUserIdAsync(int userId);
    Task<DentalTrainingRegistration?> GetRegistrationByIdAsync(int registrationId);
    Task<bool> ConfirmRegistrationAsync(int orderId, int userId);
}
