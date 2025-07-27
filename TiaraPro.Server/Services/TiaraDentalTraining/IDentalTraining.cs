using TiaraPro.Server.Models;

namespace TiaraPro.Server.Services.TiaraDentalTraining;

public interface IDentalTraining
{
    Task<IEnumerable<TiaraPro.Server.Models.DentalTraining>> GetAllTrainingsAsync();
    Task<TiaraPro.Server.Models.DentalTraining?> GetTrainingByIdAsync(int id);
    Task<bool> CreateTrainingAsync(TiaraPro.Server.Models.DentalTraining training);
    Task UpdateTrainingAsync(TiaraPro.Server.Models.DentalTraining training);
    Task<bool> DeleteTrainingAsync(int id);

    Task<IEnumerable<DentalTrainingRegistration>> GetRegistrationsByUserIdAsync(int userId);
    Task<DentalTrainingRegistration?> GetRegistrationByIdAsync(int registrationId);
    Task ConfirmRegistrationAsync(int orderId, int userId);

}
