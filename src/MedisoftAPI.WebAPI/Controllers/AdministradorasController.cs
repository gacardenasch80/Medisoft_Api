using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Contratacion;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Contratacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Administradoras — tabla Ctadminist (contratacion.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class AdministradorasController : ControllerBase
{
    private readonly ICtadministService _service;
    public AdministradorasController(ICtadministService service) => _service = service;

    /// <summary>
    /// Listar administradoras con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: CTADMICODI, CTADMINOMB, CTADMISGSS, GEDEPACODI, GEMUNICODI, CTADMIESTA, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CtadministDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CtadministQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<CtadministDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener Administradoras por consecutivo (consecutivo)</summary>
    [HttpGet("{consecutivo}")]
    [ProducesResponseType(typeof(ApiResponse<CtadministDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string consecutivo)
    {
        var item = await _service.GetByCodeAsync(consecutivo);
        if (item is null) return NotFound(ApiResponse<string>.Fail($"Disponibilidad '{consecutivo}' no encontrado."));
        return Ok(ApiResponse<CtadministDto>.Ok(item));
    }

    /// <summary>Crear nuevo servicio en FoxPro (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<CtadministDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateCtadministDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode), new { codserv = created.FASERVCODI },
    //        ApiResponse<CtadministDto>.Ok(created, "Servicio creado."));
    //}

    ///// <summary>Actualizar servicio existente (solo Admin)</summary>
    //[HttpPut("{codserv}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<CtadministDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string codserv, [FromBody] UpdateCtadministDto dto)
    //{
    //    var updated = await _service.UpdateAsync(codserv, dto);
    //    return Ok(ApiResponse<CtadministDto>.Ok(updated, "Servicio actualizado."));
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
