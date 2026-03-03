using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Admisión de pacientes — tabla ADADMIPACI.DBF (admision)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class AdmisionPacientesController : ControllerBase
{
    private readonly IAdadmipaciService _service;

    public AdmisionPacientesController(IAdadmipaciService service) => _service = service;

    /// <summary>
    /// Listar admisiones con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: ADADPACONS, CTADMICODI, ADPACIIDEN, CTCONTCODI,
    /// ADADPAESTA, CTNIVECODI, FechaInicio, FechaFin, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdadmipaciDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AdadmipaciQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<AdadmipaciDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener admisión por consecutivo (ADADPACONS)</summary>
    [HttpGet("{adadpacons}")]
    [ProducesResponseType(typeof(ApiResponse<AdadmipaciDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string adadpacons)
    {
        var item = await _service.GetByCodeAsync(adadpacons);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail($"Admisión '{adadpacons}' no encontrada."));
        return Ok(ApiResponse<AdadmipaciDto>.Ok(item));
    }

    /// <summary>
    /// Obtener todas las admisiones de un paciente con contrato y administradora incluidos.
    /// Retorna lista de admisiones enriquecidas con los objetos Ctcontrato y Ctadminist.
    /// </summary>
    [HttpGet("paciente/{adpaciiden}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AdadmipaciDetalleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPaciente(string adpaciiden)
    {
        var items = await _service.GetByPacienteAsync(adpaciiden);
        if (!items.Any())
            return NotFound(ApiResponse<string>.Fail(
                $"No se encontraron admisiones para el paciente '{adpaciiden}'."));
        return Ok(ApiResponse<IEnumerable<AdadmipaciDetalleDto>>.Ok(items,
            $"{items.Count()} admisión(es) encontrada(s) para el paciente '{adpaciiden}'."));
    }

    /// <summary>Crear nueva admisión (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdadmipaciDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateAdadmipaciDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode), new { adadpacons = created.ADADPACONS },
    //        ApiResponse<AdadmipaciDto>.Ok(created, "Admisión creada."));
    //}

    /// <summary>Actualizar admisión existente (solo Admin)</summary>
    //[HttpPut("{adadpacons}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdadmipaciDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string adadpacons, [FromBody] UpdateAdadmipaciDto dto)
    //{
    //    var updated = await _service.UpdateAsync(adadpacons, dto);
    //    return Ok(ApiResponse<AdadmipaciDto>.Ok(updated, "Admisión actualizada."));
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