using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Servicios Médicos — tabla FASERVICIO (facturacion.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ServiciosController : ControllerBase
{
    private readonly IFaservicioService _service;
    public ServiciosController(IFaservicioService service) => _service = service;

    /// <summary>
    /// Listar servicios médicos con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: FASERVCODI, FASERVNOMB, FASERVESTA, CTCLMACODI, FACLSECODI, FASERVTIPO, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FaservicioDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] FaservicioQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<FaservicioDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    ///// <summary>Obtener servicio por código (FASERVCODI)</summary>
    //[HttpGet("{codserv}")]
    //[ProducesResponseType(typeof(ApiResponse<FaservicioDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> GetByCode(string codserv)
    //{
    //    var item = await _service.GetByCodeAsync(codserv);
    //    if (item is null) return NotFound(ApiResponse<string>.Fail($"Servicio '{codserv}' no encontrado."));
    //    return Ok(ApiResponse<FaservicioDto>.Ok(item));
    //}

    /// <summary>Crear nuevo servicio en FoxPro (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<FaservicioDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateFaservicioDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode), new { codserv = created.FASERVCODI },
    //        ApiResponse<FaservicioDto>.Ok(created, "Servicio creado."));
    //}

    ///// <summary>Actualizar servicio existente (solo Admin)</summary>
    //[HttpPut("{codserv}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<FaservicioDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string codserv, [FromBody] UpdateFaservicioDto dto)
    //{
    //    var updated = await _service.UpdateAsync(codserv, dto);
    //    return Ok(ApiResponse<FaservicioDto>.Ok(updated, "Servicio actualizado."));
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
