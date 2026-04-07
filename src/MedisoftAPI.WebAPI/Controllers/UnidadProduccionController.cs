using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.DTOs.Generales;
using MedisoftAPI.Application.Interfaces.Generales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Unidades de producción — tabla Geunidprod (generales.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class UnidadProduccionController : ControllerBase
{
    private readonly IGeunidprodService _service;

    public UnidadProduccionController(IGeunidprodService service) => _service = service;

    /// <summary>
    /// Listar unidades de producción con paginación.
    /// Parámetros opcionales: Geunprcodi, Geunprnomb, Geunprresp,
    /// Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<GeunidprodDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GeunidprodQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<GeunidprodDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener unidad de producción por código (Geunprcodi)</summary>
    [HttpGet("{geunprcodi}")]
    [ProducesResponseType(typeof(ApiResponse<GeunidprodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string geunprcodi)
    {
        var item = await _service.GetByCodeAsync(geunprcodi);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail(
                $"Unidad de producción '{geunprcodi}' no encontrada."));
        return Ok(ApiResponse<GeunidprodDto>.Ok(item));
    }

    /// <summary>Crear nueva unidad de producción (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<GeunidprodDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateGeunidprodDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode),
    //        new { geunprcodi = created.Geunprcodi },
    //        ApiResponse<GeunidprodDto>.Ok(created, "Unidad de producción creada correctamente."));
    //}

    /// <summary>Actualizar unidad de producción existente (solo Admin)</summary>
    //[HttpPut("{geunprcodi}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<GeunidprodDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string geunprcodi, [FromBody] UpdateGeunidprodDto dto)
    //{
    //    var updated = await _service.UpdateAsync(geunprcodi, dto);
    //    return Ok(ApiResponse<GeunidprodDto>.Ok(updated,
    //        "Unidad de producción actualizada correctamente."));
    //}

    /// <summary>Eliminar unidad de producción (solo Admin)</summary>
    //[HttpDelete("{geunprcodi}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string geunprcodi)
    //{
    //    var deleted = await _service.DeleteAsync(geunprcodi);
    //    if (!deleted)
    //        return NotFound(ApiResponse<string>.Fail(
    //            $"Unidad de producción '{geunprcodi}' no encontrada."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Unidad de producción eliminada correctamente."));
    //}
}