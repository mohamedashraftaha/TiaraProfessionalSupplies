using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;

namespace TiaraPro.Server.Services.TiaraDentalTraining;

public class DentalTraining : IDentalTraining
{
    private readonly IUnitOfWork _unitOfWork;
    public DentalTraining(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IEnumerable<TiaraPro.Server.Models.DentalTraining>> GetAllTrainingsAsync()
    {
        return await _unitOfWork.DentalTraining.GetAllTrainingsAsync();
    }
    public async Task<TiaraPro.Server.Models.DentalTraining?> GetTrainingByIdAsync(int id)
    {
        return await _unitOfWork.DentalTraining.GetTrainingByIdAsync(id);
    }
    public async Task<bool> CreateTrainingAsync(TiaraPro.Server.Models.DentalTraining training)
    {
        return await _unitOfWork.DentalTraining.CreateTrainingAsync(training);
    }
    public async Task<bool> UpdateTrainingAsync(TiaraPro.Server.Models.DentalTraining training)
    {
        return await _unitOfWork.DentalTraining.UpdateTrainingAsync(training);
    }
    public async Task<bool> DeleteTrainingAsync(int id)
    {
        return await _unitOfWork.DentalTraining.DeleteTrainingAsync(id);
    }
    public async Task<IEnumerable<DentalTrainingRegistration>> GetRegistrationsByUserIdAsync(int userId)
    {
        return await _unitOfWork.DentalTraining.GetRegistrationsByUserIdAsync(userId);
    }
    public async Task<DentalTrainingRegistration?> GetRegistrationByIdAsync(int registrationId)
    {
        return await _unitOfWork.DentalTraining.GetRegistrationByIdAsync(registrationId);
    }
    public async Task<bool> ConfirmRegistrationAsync(int orderId, int userId)
    {
        try
        {
            return await _unitOfWork.DentalTraining.ConfirmRegistrationAsync(orderId, userId);

        }
        catch (Exception ex)
        {
            return false;
        }
    }

}
