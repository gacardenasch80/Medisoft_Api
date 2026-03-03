namespace MedisoftAPI.Application.DTOs;

public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string   Token     { get; set; } = string.Empty;
    public string   Username  { get; set; } = string.Empty;
    public string   Role      { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class UserDto
{
    public int      Id        { get; set; }
    public string   Username  { get; set; } = string.Empty;
    public string   Role      { get; set; } = string.Empty;
    public bool     IsActive  { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role     { get; set; } = "User";
}

public class UpdateUserDto
{
    public string? Password { get; set; }
    public string? Role     { get; set; }
    public bool?   IsActive { get; set; }
}
