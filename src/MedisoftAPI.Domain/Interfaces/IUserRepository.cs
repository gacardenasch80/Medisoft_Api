using MedisoftAPI.Domain.Entities;

namespace MedisoftAPI.Domain.Interfaces;

public interface IUserRepository
{
    Task<AppUser?>             GetByUsernameAsync(string username);
    Task<AppUser?>             GetByIdAsync(int id);
    Task<IEnumerable<AppUser>> GetAllAsync();
    Task<AppUser>              CreateAsync(AppUser user);
    Task<AppUser>              UpdateAsync(AppUser user);
    Task<bool>                 DeleteAsync(int id);
}
