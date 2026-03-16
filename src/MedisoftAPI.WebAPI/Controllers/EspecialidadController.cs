using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Generales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Especialidades Médicas — tabla Geespecial (Generales.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EspecialidadController : ControllerBase
{
    private readonly IGeespecialService _service;
    public EspecialidadController(IGeespecialService service) => _service = service;

    /// <summary>
    /// Listar especialidades con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: Addispcons, Geespecodi, Geespenomb, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<GeespecialDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GeespecialQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<GeespecialDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener especialidad por consecutivo (consecutivo)</summary>
    [HttpGet("{consecutivo}")]
    [ProducesResponseType(typeof(ApiResponse<GeespecialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string consecutivo)
    {
        var item = await _service.GetByCodeAsync(consecutivo);
        if (item is null) return NotFound(ApiResponse<string>.Fail($"Especialidad '{consecutivo}' no encontrado."));
        return Ok(ApiResponse<GeespecialDto>.Ok(item));
    }

    /// <summary>Crear nuevo servicio en FoxPro (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<GeespecialDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateGeespecialDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode), new { codserv = created.FASERVCODI },
    //        ApiResponse<GeespecialDto>.Ok(created, "Servicio creado."));
    //}

    ///// <summary>Actualizar servicio existente (solo Admin)</summary>
    //[HttpPut("{codserv}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<GeespecialDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string codserv, [FromBody] UpdateGeespecialDto dto)
    //{
    //    var updated = await _service.UpdateAsync(codserv, dto);
    //    return Ok(ApiResponse<GeespecialDto>.Ok(updated, "Servicio actualizado."));
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
