using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MedisoftAPI.Application.UseCases;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IConfiguration  _config;

    public AuthService(IUserRepository users, IConfiguration config)
    {
        _users  = users;
        _config = config;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _users.GetByUsernameAsync(dto.Username);
        if (user is null || !user.IsActive)                   return null;
        if (user.PasswordHash != HashPassword(dto.Password))  return null;

        var expires = DateTime.UtcNow.AddHours(8);
        return new LoginResponseDto
        {
            Token     = BuildJwt(user, expires),
            Username  = user.Username,
            Role      = user.Role,
            ExpiresAt = expires
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync() =>
        (await _users.GetAllAsync()).Select(ToDto);

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var u = await _users.GetByIdAsync(id);
        return u is null ? null : ToDto(u);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var user = new AppUser
        {
            Username     = dto.Username,
            PasswordHash = HashPassword(dto.Password),
            Role         = dto.Role,
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow
        };
        return ToDto(await _users.CreateAsync(user));
    }

    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _users.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Usuario {id} no encontrado.");

        if (!string.IsNullOrWhiteSpace(dto.Password)) user.PasswordHash = HashPassword(dto.Password);
        if (!string.IsNullOrWhiteSpace(dto.Role))     user.Role         = dto.Role;
        if (dto.IsActive.HasValue)                     user.IsActive     = dto.IsActive.Value;

        return ToDto(await _users.UpdateAsync(user));
    }

    public Task<bool> DeleteUserAsync(int id) => _users.DeleteAsync(id);

    // ── Métodos públicos usados por DatabaseSeeder ──────────────
    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }

    // ── Privados ────────────────────────────────────────────────
    private string BuildJwt(AppUser user, DateTime expires)
    {
        var cfg = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["SecretKey"]!));

        var token = new JwtSecurityToken(
            issuer:             cfg["Issuer"],
            audience:           cfg["Audience"],
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,           user.Username),
                new Claim(ClaimTypes.Role,           user.Role)
            ],
            expires:            expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserDto ToDto(AppUser u) => new()
    {
        Id        = u.Id,
        Username  = u.Username,
        Role      = u.Role,
        IsActive  = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
