using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.UserRepositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task Update(User user);
        Task Delete(User user);
        Task SaveAsync();
    }
}
