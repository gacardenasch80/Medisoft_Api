using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Application.Interfaces.Citas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Citas de pacientes — tabla Adcitas.DBF (citas.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class CitasPacientesController : ControllerBase
{
    private readonly IAdcitasService _service;

    public CitasPacientesController(IAdcitasService service) => _service = service;

    /// <summary>
    /// Listar citas con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: Adcitacons, Geespecodi, Gemedicodi,
    /// Faservcodi, Adpaciiden, Adconscodi, Adfechcita, Ctadmicodi,
    /// Ctcontcodi, Adingrcons, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdcitasDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AdcitasQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<AdcitasDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener cita por consecutivo (Adcitacons)</summary>
    [HttpGet("{adcitacons}")]
    [ProducesResponseType(typeof(ApiResponse<AdcitasDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string adcitacons)
    {
        var item = await _service.GetByCodeAsync(adcitacons);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail($"Cita '{adcitacons}' no encontrada."));
        return Ok(ApiResponse<AdcitasDto>.Ok(item));
    }

    /// <summary>
    /// Obtener todas las citas de un paciente con contrato y administradora incluidos.
    /// Retorna lista de citas enriquecidas con los objetos Ctcontrato y Ctadminist.
    /// </summary>
    [HttpGet("paciente/{adpaciiden}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AdcitasDetalleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPaciente(string adpaciiden)
    {
        var items = await _service.GetByPacienteAsync(adpaciiden);
        if (!items.Any())
            return NotFound(ApiResponse<string>.Fail(
                $"No se encontraron citas para el paciente '{adpaciiden}'."));
        return Ok(ApiResponse<IEnumerable<AdcitasDetalleDto>>.Ok(items,
            $"{items.Count()} cita(s) encontrada(s) para el paciente '{adpaciiden}'."));
    }

    /// <summary>
    /// Crear cita a partir de una disponibilidad médica (Addispmed).
    /// El sistema genera el consecutivo automáticamente, toma los datos del
    /// médico/especialidad/servicio/horario del Addispmed indicado, y toma
    /// Ctadmicodi y Ctcontcodi del Adadmipaci del paciente.
    /// Al finalizar marca la disponibilidad como citada (Addispcita = true).
    /// </summary>
    [HttpPost("desde-disponibilidad")]
    [ProducesResponseType(typeof(ApiResponse<AdcitasConfirmacionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFromDispmed([FromBody] CreateAdcitasFromDispmedDto dto)
    {
        try
        {
            var created = await _service.CreateFromDispmedAsync(dto);
            return CreatedAtAction(nameof(GetByCode),
                new { adcitacons = created.Adcitacons },
                ApiResponse<AdcitasConfirmacionDto>.Ok(created, "Cita creada correctamente."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    /// <summary>Actualizar cita existente (solo Admin)</summary>
    //[HttpPut("{adcitacons}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdcitasDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string adcitacons, [FromBody] UpdateAdcitasDto dto)
    //{
    //    var updated = await _service.UpdateAsync(adcitacons, dto);
    //    return Ok(ApiResponse<AdcitasDto>.Ok(updated, "Cita actualizada."));
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