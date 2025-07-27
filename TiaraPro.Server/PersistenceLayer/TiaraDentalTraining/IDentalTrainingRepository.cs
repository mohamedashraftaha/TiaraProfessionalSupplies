using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.TiaraDentalTraining;

public interface IDentalTrainingRepository
{
    Task<IEnumerable<DentalTraining>> GetAllTrainingsAsync();
    Task<DentalTraining?> GetTrainingByIdAsync(int id);
    Task CreateTrainingAsync(DentalTraining training);
    Task UpdateTrainingAsync(DentalTraining training);
    Task DeleteTrainingAsync(int id);
    Task<IEnumerable<DentalTrainingRegistration>> GetRegistrationsByUserIdAsync(int userId);
    Task<DentalTrainingRegistration?> GetRegistrationByIdAsync(int registrationId);
    Task ConfirmRegistrationAsync(int orderId, int userId);
}
