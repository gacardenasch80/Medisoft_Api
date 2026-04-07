using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Application.Interfaces.Citas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Citas de pacientes — tabla Adconsulto.DBF (citas.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ConsultoriosController : ControllerBase
{
    private readonly IAdconsultoService _service;

    public ConsultoriosController(IAdconsultoService service) => _service = service;

    /// <summary>
    /// Listar consulturios con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: Adconscodi, Adconsnomb, Adconsdire,
    /// Adconstele, Geunprcodi,
    /// Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdconsultoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AdconsultoQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<AdconsultoDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener cita por consecutivo (Adcitacons)</summary>
    [HttpGet("{adconscodi}")]
    [ProducesResponseType(typeof(ApiResponse<AdconsultoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string adconscodi)
    {
        var item = await _service.GetByCodeAsync(adconscodi);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail($"Consultorio '{adconscodi}' no encontrada."));
        return Ok(ApiResponse<AdconsultoDto>.Ok(item));
    }

    /// <summary>Actualizar cita existente (solo Admin)</summary>
    //[HttpPut("{adcitacons}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdconsultoDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string adcitacons, [FromBody] UpdateAdconsultoDto dto)
    //{
    //    var updated = await _service.UpdateAsync(adcitacons, dto);
    //    return Ok(ApiResponse<AdconsultoDto>.Ok(updated, "Cita actualizada."));
    //}

    /// <summary>Eliminar cita (solo Admin)</summary>
    //[HttpDelete("{adcitacons}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string adcitacons)
    //{
    //    var deleted = await _service.DeleteAsync(adcitacons);
    //    if (!deleted)
    //        return NotFound(ApiResponse<string>.Fail($"Cita '{adcitacons}' no encontrada."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Cita eliminada."));
    //}
}