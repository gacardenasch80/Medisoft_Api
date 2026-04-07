using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.DTOs.Generales;
using MedisoftAPI.Application.Interfaces.Generales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Hospital — tabla Gehospital (generales.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class HospitalController : ControllerBase
{
    private readonly IGehospitalService _service;

    public HospitalController(IGehospitalService service) => _service = service;

    /// <summary>
    /// Listar hospitales con paginación.
    /// Parámetros opcionales: Gehospcodi, Gehospnomb, Gehospnit,
    /// Gedepacodi, Gemunicodi, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<GehospitalDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GehospitalQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<GehospitalDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener hospital por código (Gehospcodi)</summary>
    [HttpGet("{gehospcodi}")]
    [ProducesResponseType(typeof(ApiResponse<GehospitalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string gehospcodi)
    {
        var item = await _service.GetByCodeAsync(gehospcodi);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail($"Hospital '{gehospcodi}' no encontrado."));
        return Ok(ApiResponse<GehospitalDto>.Ok(item));
    }

    /// <summary>Crear nuevo hospital (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<GehospitalDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateGehospitalDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode),
    //        new { gehospcodi = created.Gehospcodi },
    //        ApiResponse<GehospitalDto>.Ok(created, "Hospital creado correctamente."));
    //}

    /// <summary>Actualizar hospital existente (solo Admin)</summary>
    //[HttpPut("{gehospcodi}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<GehospitalDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string gehospcodi, [FromBody] UpdateGehospitalDto dto)
    //{
    //    var updated = await _service.UpdateAsync(gehospcodi, dto);
    //    return Ok(ApiResponse<GehospitalDto>.Ok(updated, "Hospital actualizado correctamente."));
    //}

    /// <summary>Eliminar hospital (solo Admin)</summary>
    //[HttpDelete("{gehospcodi}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string gehospcodi)
    //{
    //    var deleted = await _service.DeleteAsync(gehospcodi);
    //    if (!deleted)
    //        return NotFound(ApiResponse<string>.Fail($"Hospital '{gehospcodi}' no encontrado."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Hospital eliminado correctamente."));
    //}
}