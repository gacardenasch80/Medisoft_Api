using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Citas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Parámetros del sistema de citas — tabla Adparametros (citas.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ParametrosController : ControllerBase
{
    private readonly IAdparametrosService _service;

    public ParametrosController(IAdparametrosService service) => _service = service;

    /// <summary>
    /// Listar parámetros con paginación.
    /// Parámetros opcionales: Adparametro, Advalorpara, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdparametrosDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AdparametrosQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<AdparametrosDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener parámetro por código (Adparametro)</summary>
    [HttpGet("{adparametro}")]
    [ProducesResponseType(typeof(ApiResponse<AdparametrosDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string adparametro)
    {
        var item = await _service.GetByCodeAsync(adparametro);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail($"Parámetro '{adparametro}' no encontrado."));
        return Ok(ApiResponse<AdparametrosDto>.Ok(item));
    }

    /// <summary>Crear nuevo parámetro (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdparametrosDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateAdparametrosDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode),
    //        new { adparametro = created.Adparametro },
    //        ApiResponse<AdparametrosDto>.Ok(created, "Parámetro creado correctamente."));
    //}

    /// <summary>Actualizar valor de un parámetro (solo Admin)</summary>
    //[HttpPut("{adparametro}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<AdparametrosDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string adparametro, [FromBody] UpdateAdparametrosDto dto)
    //{
    //    try
    //    {
    //        var updated = await _service.UpdateAsync(adparametro, dto);
    //        return Ok(ApiResponse<AdparametrosDto>.Ok(updated, "Parámetro actualizado correctamente."));
    //    }
    //    catch (KeyNotFoundException ex)
    //    {
    //        return NotFound(ApiResponse<string>.Fail(ex.Message));
    //    }
    //}

    ///// <summary>Eliminar parámetro (solo Admin)</summary>
    //[HttpDelete("{adparametro}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string adparametro)
    //{
    //    var deleted = await _service.DeleteAsync(adparametro);
    //    if (!deleted)
    //        return NotFound(ApiResponse<string>.Fail($"Parámetro '{adparametro}' no encontrado."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Parámetro eliminado correctamente."));
    //}
}