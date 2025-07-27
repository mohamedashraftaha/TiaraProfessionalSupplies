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
        try
        {
            if (training == null)
            {
                throw new ArgumentNullException(nameof(training), "Training cannot be null");
            }
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.DentalTraining.CreateTrainingAsync(training);
            int rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected == 0)
            {
                throw new Exception("No rows affected while creating training");
            }
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new Exception("Error creating training", ex);
        }
    }
    public async Task UpdateTrainingAsync(TiaraPro.Server.Models.DentalTraining training)
    {
        try
        {
            await _unitOfWork.DentalTraining.UpdateTrainingAsync(training);

        }
        catch (Exception ex)
        {
            throw new Exception("Error updating training", ex);
        }
    }
    public async Task<bool> DeleteTrainingAsync(int id)
    {

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.DentalTraining.DeleteTrainingAsync(id);
            int rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected == 0)
            {
                throw new Exception("No rows affected while deleting training");
            }
            return true;

        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting training", ex);
        }
    }
    public async Task<IEnumerable<DentalTrainingRegistration>> GetRegistrationsByUserIdAsync(int userId)
    {
        return await _unitOfWork.DentalTraining.GetRegistrationsByUserIdAsync(userId);
    }
    public async Task<DentalTrainingRegistration?> GetRegistrationByIdAsync(int registrationId)
    {
        return await _unitOfWork.DentalTraining.GetRegistrationByIdAsync(registrationId);
    }
    public async Task ConfirmRegistrationAsync(int orderId, int userId)
    {
        try
        {
            await _unitOfWork.DentalTraining.ConfirmRegistrationAsync(orderId, userId);

        }
        catch (Exception ex)
        {           
            throw ex;
        }
    }

}
