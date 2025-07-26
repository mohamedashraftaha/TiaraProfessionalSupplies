using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;
namespace TiaraPro.Server.PersistenceLayer.UserRepositories;

public class UserRepository : IUserRepository
{
    private readonly TiaraDbContext _context;
    public UserRepository(TiaraDbContext context)
    {
        _context = context;
    }
    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<List<User>> GetAllAsync()
    {
        try
        {
            return await _context.Users.ToListAsync();

        }
        catch (Exception ex)
        {
            return new List<User>();
        }
    }
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await SaveAsync();
    }
    public async Task Update(User user)
    {
        _context.Users.Update(user);
        await SaveAsync();
    }
    public async Task Delete(User user)
    {
        _context.Users.Remove(user);
        await SaveAsync();
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

}
