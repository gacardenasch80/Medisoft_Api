using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Servicios Médicos — tabla Addispmed (facturacion.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class DisponibilidadController : ControllerBase
{
    private readonly IAddispmedService _service;
    public DisponibilidadController(IAddispmedService service) => _service = service;

    /// <summary>
    /// Listar disponibilidad con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: Addispcons, Geespecodi, Gemedicodi, FechaInicio, FechaFin, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AddispmedDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AddispmedQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<AddispmedDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener disponibilidad por consecutivo (consecutivo)</summary>
    [HttpGet("{consecutivo}")]
    [ProducesResponseType(typeof(ApiResponse<AddispmedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string consecutivo)
    {
        var item = await _service.GetByCodeAsync(consecutivo);
        if (item is null) return NotFound(ApiResponse<string>.Fail($"Disponibilidad '{consecutivo}' no encontrado."));
        return Ok(ApiResponse<AddispmedDto>.Ok(item));
    }

    /// <summary>Crear nuevo servicio en FoxPro (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AddispmedDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateAddispmedDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode), new { codserv = created.FASERVCODI },
    //        ApiResponse<AddispmedDto>.Ok(created, "Servicio creado."));
    //}

    ///// <summary>Actualizar servicio existente (solo Admin)</summary>
    //[HttpPut("{codserv}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AddispmedDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string codserv, [FromBody] UpdateAddispmedDto dto)
    //{
    //    var updated = await _service.UpdateAsync(codserv, dto);
    //    return Ok(ApiResponse<AddispmedDto>.Ok(updated, "Servicio actualizado."));
    //}

    ///// <summary>Eliminar servicio de FoxPro (solo Admin)</summary>
    //[HttpDelete("{codserv}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string codserv)
    //{
    //    var deleted = await _service.DeleteAsync(codserv);
    //    if (!deleted) return NotFound(ApiResponse<string>.Fail($"Servicio '{codserv}' no encontrado."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Servicio eliminado."));
    //}
}
