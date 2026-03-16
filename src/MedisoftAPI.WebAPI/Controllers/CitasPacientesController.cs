using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Citas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Admisión de pacientes — tabla Adcitas.DBF (admision)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class CitasPacientesController : ControllerBase
{
    private readonly IAdcitasService _service;

    public CitasPacientesController(IAdcitasService service) => _service = service;

    /// <summary>
    /// Listar admisiones con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: Adcitacons, Geespecodi, Gemedicodi,
    /// Faservcodi, Adpaciiden, Adconscodi,Adfechcita, Ctadmicodi, Ctcontcodi, Adingrcons,, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdcitasDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AdcitasQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<AdcitasDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener admisión por consecutivo (Adcitacons)</summary>
    [HttpGet("{Adcitacons}")]
    [ProducesResponseType(typeof(ApiResponse<AdcitasDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string Adcitacons)
    {
        var item = await _service.GetByCodeAsync(Adcitacons);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail($"Cita '{Adcitacons}' no encontrada."));
        return Ok(ApiResponse<AdcitasDto>.Ok(item));
    }

    /// <summary>
    /// Obtener todas las admisiones de un paciente con contrato y administradora incluidos.
    /// Retorna lista de admisiones enriquecidas con los objetos Ctcontrato y Ctadminist.
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
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AdcitasDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFromDispmed([FromBody] CreateAdcitasFromDispmedDto dto)
    {
        var created = await _service.CreateFromDispmedAsync(dto);
        return CreatedAtAction(nameof(GetByCode),
            new { adcitacons = created.Adcitacons },
            ApiResponse<AdcitasDto>.Ok(created, "Cita creada correctamente."));
    }

    /// <summary>Crear nueva admisión (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdcitasDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateAdcitasDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode), new { adadpacons = created.ADADPACONS },
    //        ApiResponse<AdcitasDto>.Ok(created, "Admisión creada."));
    //}

    /// <summary>Actualizar admisión existente (solo Admin)</summary>
    //[HttpPut("{adadpacons}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdcitasDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string adadpacons, [FromBody] UpdateAdcitasDto dto)
    //{
    //    var updated = await _service.UpdateAsync(adadpacons, dto);
    //    return Ok(ApiResponse<AdcitasDto>.Ok(updated, "Admisión actualizada."));
    //}

    /// <summary>Eliminar admisión (solo Admin)</summary>
    //[HttpDelete("{adadpacons}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string adadpacons)
    //{
    //    var deleted = await _service.DeleteAsync(adadpacons);
    //    if (!deleted)
    //        return NotFound(ApiResponse<string>.Fail($"Admisión '{adadpacons}' no encontrada."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Admisión eliminada."));
    //}
}