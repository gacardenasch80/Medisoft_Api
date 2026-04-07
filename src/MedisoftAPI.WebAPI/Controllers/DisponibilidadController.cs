using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Application.Interfaces.Citas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Disponibilidad médica — tabla Addispmed (citas.dbc)</summary>
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
    /// Devuelve nombre de especialidad, médico y servicio incluidos.
    /// Parámetros opcionales: Addispcons, Geespecodi, Gemedicodi,
    /// FechaInicio, FechaFin, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AddispmedDetalleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] AddispmedQueryDto query)
    {
        try
        {
            var result = await _service.GetAllAsync(query);
            return Ok(ApiResponse<PagedResult<AddispmedDetalleDto>>.Ok(result,
                $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Obtener disponibilidad por consecutivo (Addispcons).
    /// Incluye nombre de especialidad, médico y servicio.
    /// </summary>
    [HttpGet("{consecutivo}")]
    [ProducesResponseType(typeof(ApiResponse<AddispmedDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string consecutivo)
    {
        var item = await _service.GetByCodeAsync(consecutivo);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail($"Disponibilidad '{consecutivo}' no encontrada."));
        return Ok(ApiResponse<AddispmedDetalleDto>.Ok(item));
    }

    /// <summary>Crear nueva disponibilidad (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AddispmedDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateAddispmedDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode), new { consecutivo = created.Addispcons },
    //        ApiResponse<AddispmedDto>.Ok(created, "Disponibilidad creada."));
    //}

    /// <summary>Actualizar disponibilidad existente (solo Admin)</summary>
    //[HttpPut("{consecutivo}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AddispmedDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string consecutivo, [FromBody] UpdateAddispmedDto dto)
    //{
    //    var updated = await _service.UpdateAsync(consecutivo, dto);
    //    return Ok(ApiResponse<AddispmedDto>.Ok(updated, "Disponibilidad actualizada."));
    //}

    /// <summary>Eliminar disponibilidad (solo Admin)</summary>
    //[HttpDelete("{consecutivo}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string consecutivo)
    //{
    //    var deleted = await _service.DeleteAsync(consecutivo);
    //    if (!deleted)
    //        return NotFound(ApiResponse<string>.Fail($"Disponibilidad '{consecutivo}' no encontrada."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Disponibilidad eliminada."));
    //}
}