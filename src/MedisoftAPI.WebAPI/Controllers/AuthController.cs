using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Autenticación y gestión de usuarios del sistema</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Autenticar usuario — retorna token JWT. Usuario por defecto: Admin / 123456</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _auth.LoginAsync(dto);
        if (result is null)
            return Unauthorized(ApiResponse<string>.Fail("Usuario o contraseña incorrectos."));

        return Ok(ApiResponse<LoginResponseDto>.Ok(result, "Login exitoso."));
    }

    /// <summary>Listar todos los usuarios (solo Admin)</summary>
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
        => Ok(ApiResponse<IEnumerable<UserDto>>.Ok(await _auth.GetAllUsersAsync()));

    /// <summary>Obtener usuario por Id (solo Admin)</summary>
    [HttpGet("users/{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _auth.GetUserByIdAsync(id);
        if (user is null) return NotFound(ApiResponse<string>.Fail($"Usuario {id} no encontrado."));
        return Ok(ApiResponse<UserDto>.Ok(user));
    }

    /// <summary>Crear nuevo usuario (solo Admin)</summary>
    [HttpPost("users")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = await _auth.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id },
            ApiResponse<UserDto>.Ok(user, "Usuario creado."));
    }

    /// <summary>Actualizar contraseña, rol o estado (solo Admin)</summary>
    [HttpPut("users/{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        var user = await _auth.UpdateUserAsync(id, dto);
        return Ok(ApiResponse<UserDto>.Ok(user, "Usuario actualizado."));
    }

    /// <summary>Eliminar usuario (solo Admin)</summary>
    [HttpDelete("users/{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _auth.DeleteUserAsync(id);
        if (!deleted) return NotFound(ApiResponse<string>.Fail($"Usuario {id} no encontrado."));
        return Ok(ApiResponse<string>.Ok("ok", "Usuario eliminado."));
    }
}
