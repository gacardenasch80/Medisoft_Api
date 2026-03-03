using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?>    LoginAsync(LoginRequestDto dto);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?>             GetUserByIdAsync(int id);
    Task<UserDto>              CreateUserAsync(CreateUserDto dto);
    Task<UserDto>              UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool>                 DeleteUserAsync(int id);
}
