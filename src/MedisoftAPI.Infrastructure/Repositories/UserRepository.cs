using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;
using MedisoftAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedisoftAPI.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<AppUser?> GetByUsernameAsync(string username) =>
        _db.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<AppUser?> GetByIdAsync(int id) =>
        await _db.Users.FindAsync(id);

    public async Task<IEnumerable<AppUser>> GetAllAsync() =>
        await _db.Users.OrderBy(u => u.Username).ToListAsync();

    public async Task<AppUser> CreateAsync(AppUser user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<AppUser> UpdateAsync(AppUser user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return false;
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }
}
