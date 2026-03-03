using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Servicios Médicos — tabla Adpaciente (facturacion.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class PacientesController : ControllerBase
{
    private readonly IAdpacienteService _service;
    public PacientesController(IAdpacienteService service) => _service = service;

    /// <summary>
    /// Listar pacientes con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: ADTIIDCODI, ADPACIIDEN, ADPACIAPE1, ADPACIAPE2, ADPACINOM1, ADPACINOM2, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdpacienteDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AdpacienteQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<AdpacienteDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener paciente por identificación (identificacion)</summary>
    [HttpGet("{identificacion}")]
    [ProducesResponseType(typeof(ApiResponse<AdpacienteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string identificacion)
    {
        var item = await _service.GetByCodeAsync(identificacion);
        if (item is null) return NotFound(ApiResponse<string>.Fail($"Paciente '{identificacion}' no encontrado."));
        return Ok(ApiResponse<AdpacienteDto>.Ok(item));
    }

    /// <summary>Crear nuevo servicio en FoxPro (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdpacienteDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateAdpacienteDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode), new { codserv = created.FASERVCODI },
    //        ApiResponse<AdpacienteDto>.Ok(created, "Servicio creado."));
    //}

    ///// <summary>Actualizar servicio existente (solo Admin)</summary>
    //[HttpPut("{codserv}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdpacienteDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string codserv, [FromBody] UpdateAdpacienteDto dto)
    //{
    //    var updated = await _service.UpdateAsync(codserv, dto);
    //    return Ok(ApiResponse<AdpacienteDto>.Ok(updated, "Servicio actualizado."));
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
