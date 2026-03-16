using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Contratacion;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Contratacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Contratos — tabla Ctcontrato (contratacion.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ContratosController : ControllerBase
{
    private readonly ICtcontratoService _service;

    public ContratosController(ICtcontratoService service) => _service = service;

    /// <summary>
    /// Listar contratos con paginación.
    /// Por defecto devuelve 50 registros por página.
    /// Parámetros opcionales: CTCONTCODI, CTCONTNUME, CTADMICODI, CTREGICODI,
    /// CTCONTESTA, CTTICOCODI, FAFEMPCODI, FechaDesde, FechaHasta,
    /// Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CtcontratoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CtcontratoQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<CtcontratoDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener contrato por código (CTCONTCODI)</summary>
    [HttpGet("{ctcontcodi}")]
    [ProducesResponseType(typeof(ApiResponse<CtcontratoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string ctcontcodi)
    {
        var item = await _service.GetByCodeAsync(ctcontcodi);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail($"Contrato '{ctcontcodi}' no encontrado."));
        return Ok(ApiResponse<CtcontratoDto>.Ok(item));
    }

    /// <summary>Crear nuevo contrato en FoxPro (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<CtcontratoDto>), StatusCodes.Status201Created)]
    //public async Task<IActionResult> Create([FromBody] CreateCtcontratoDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode), new { ctcontcodi = created.CTCONTCODI },
    //        ApiResponse<CtcontratoDto>.Ok(created, "Contrato creado."));
    //}

    /// <summary>Actualizar contrato existente (solo Admin)</summary>
    //[HttpPut("{ctcontcodi}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<CtcontratoDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string ctcontcodi, [FromBody] UpdateCtcontratoDto dto)
    //{
    //    var updated = await _service.UpdateAsync(ctcontcodi, dto);
    //    return Ok(ApiResponse<CtcontratoDto>.Ok(updated, "Contrato actualizado."));
    //}

    /// <summary>Eliminar contrato de FoxPro (solo Admin)</summary>
    //[HttpDelete("{ctcontcodi}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string ctcontcodi)
    //{
    //    var deleted = await _service.DeleteAsync(ctcontcodi);
    //    if (!deleted)
    //        return NotFound(ApiResponse<string>.Fail($"Contrato '{ctcontcodi}' no encontrado."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Contrato eliminado."));
    //}
}