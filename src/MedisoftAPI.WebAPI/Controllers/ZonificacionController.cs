using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Citas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Zonificación de pacientes — tabla Adzonifica (citas.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ZonificacionController : ControllerBase
{
    private readonly IAdzonificaService _service;

    public ZonificacionController(IAdzonificaService service) => _service = service;

    /// <summary>
    /// Listar zonificaciones con paginación.
    /// Parámetros opcionales: Adpaciiden, Geunprcodi, Estado (1=Activo, 2=Inactivo),
    /// Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdzonificaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AdzonificaQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<AdzonificaDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>
    /// Obtener todas las zonificaciones de un paciente.
    /// </summary>
    [HttpGet("paciente/{adpaciiden}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AdzonificaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPaciente(string adpaciiden)
    {
        var items = (await _service.GetByPacienteAsync(adpaciiden)).ToList();
        if (!items.Any())
            return NotFound(ApiResponse<string>.Fail(
                $"No se encontraron zonificaciones para el paciente '{adpaciiden}'."));
        return Ok(ApiResponse<IEnumerable<AdzonificaDto>>.Ok(items,
            $"{items.Count} zonificación(es) encontrada(s)."));
    }

    /// <summary>
    /// Obtener una zonificación específica por paciente y unidad de producción.
    /// </summary>
    [HttpGet("paciente/{adpaciiden}/zona/{geunprcodi}")]
    [ProducesResponseType(typeof(ApiResponse<AdzonificaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPacienteYZona(string adpaciiden, string geunprcodi)
    {
        var item = await _service.GetByPacienteYZonaAsync(adpaciiden, geunprcodi);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail(
                $"No se encontró zonificación para el paciente '{adpaciiden}' " +
                $"en la unidad '{geunprcodi}'."));
        return Ok(ApiResponse<AdzonificaDto>.Ok(item));
    }

    /// <summary>
    /// Crear nueva zonificación de paciente.
    /// Estado: 1 = Activo (default), 2 = Inactivo.
    /// Retorna error 409 si ya existe la combinación paciente + zona.
    /// </summary>
    //[HttpPost]
    //[ProducesResponseType(typeof(ApiResponse<AdzonificaDto>), StatusCodes.Status201Created)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
    //public async Task<IActionResult> Create([FromBody] CreateAdzonificaDto dto)
    //{
    //    try
    //    {
    //        var created = await _service.CreateAsync(dto);
    //        return CreatedAtAction(nameof(GetByPacienteYZona),
    //            new { adpaciiden = created.Adpaciiden, geunprcodi = created.Geunprcodi },
    //            ApiResponse<AdzonificaDto>.Ok(created, "Zonificación creada correctamente."));
    //    }
    //    catch (InvalidOperationException ex)
    //    {
    //        return Conflict(ApiResponse<string>.Fail(ex.Message));
    //    }
    //}

    /// <summary>
    /// Actualizar el estado de una zonificación (1=Activo, 2=Inactivo).
    /// </summary>
    //[HttpPut("paciente/{adpaciiden}/zona/{geunprcodi}")]
    //[ProducesResponseType(typeof(ApiResponse<AdzonificaDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(
    //    string adpaciiden, string geunprcodi, [FromBody] UpdateAdzonificaDto dto)
    //{
    //    try
    //    {
    //        var updated = await _service.UpdateAsync(adpaciiden, geunprcodi, dto);
    //        return Ok(ApiResponse<AdzonificaDto>.Ok(updated, "Zonificación actualizada correctamente."));
    //    }
    //    catch (KeyNotFoundException ex)
    //    {
    //        return NotFound(ApiResponse<string>.Fail(ex.Message));
    //    }
    //}

    /// <summary>
    /// Eliminar una zonificación de paciente.
    /// </summary>
    //[HttpDelete("paciente/{adpaciiden}/zona/{geunprcodi}")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string adpaciiden, string geunprcodi)
    //{
    //    var deleted = await _service.DeleteAsync(adpaciiden, geunprcodi);
    //    if (!deleted)
    //        return NotFound(ApiResponse<string>.Fail(
    //            $"No se encontró zonificación para el paciente '{adpaciiden}' " +
    //            $"en la unidad '{geunprcodi}'."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Zonificación eliminada correctamente."));
    //}
}