using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Facturacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedisoftAPI.WebAPI.Controllers;

/// <summary>Programas PyP — tabla Faprogpyp (facturacion.dbc)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProgramasPypController : ControllerBase
{
    private readonly IFaprogpypService _service;

    public ProgramasPypController(IFaprogpypService service) => _service = service;

    /// <summary>
    /// Listar programas PyP con paginación.
    /// Parámetros opcionales: Faprogcodi, Faprogcod1, Faprognomb, Faprogclas,
    /// Faservcodi, Faprogestad, Pagina, TamPagina (máx 200)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FaprogpypDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] FaprogpypQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<FaprogpypDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros en total."));
    }

    /// <summary>Obtener programa PyP por código (Faprogcodi)</summary>
    [HttpGet("{faprogcodi}")]
    [ProducesResponseType(typeof(ApiResponse<FaprogpypDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string faprogcodi)
    {
        var item = await _service.GetByCodeAsync(faprogcodi);
        if (item is null)
            return NotFound(ApiResponse<string>.Fail($"Programa PyP '{faprogcodi}' no encontrado."));
        return Ok(ApiResponse<FaprogpypDto>.Ok(item));
    }

    /// <summary>
    /// Devuelve los programas PyP habilitados para un paciente,
    /// filtrando por su edad (calculada desde ADPACIFENA) y su sexo (ADPACISEXO).
    ///
    /// Reglas replicadas desde FoxPro:
    ///   - Edad = (Hoy - FechaNacimiento) / 365
    ///   - Edad debe estar en [Faprogdesd, Faproghast]
    ///   - Faproggene = 0 → ambos sexos
    ///   - Faproggene = 1 → solo hombres (excluye pacientes con sexo 'F')
    ///   - Faproggene = 2 → solo mujeres (excluye pacientes con sexo 'M')
    /// </summary>
    [HttpGet("paciente/{adpaciiden}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FaprogpypDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPaciente(string adpaciiden)
    {
        var items = await _service.GetByPacienteAsync(adpaciiden);
        var list = items.ToList();

        if (!list.Any())
            return NotFound(ApiResponse<string>.Fail(
                $"No se encontraron programas PyP habilitados para el paciente '{adpaciiden}'."));

        return Ok(ApiResponse<IEnumerable<FaprogpypDto>>.Ok(list,
            $"{list.Count} programa(s) PyP habilitado(s) para el paciente '{adpaciiden}'."));
    }

    /// <summary>Crear nuevo programa PyP (solo Admin)</summary>
    //[HttpPost]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<FaprogpypDto>), StatusCodes.Status201Created)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> Create([FromBody] CreateFaprogpypDto dto)
    //{
    //    var created = await _service.CreateAsync(dto);
    //    return CreatedAtAction(nameof(GetByCode),
    //        new { faprogcodi = created.Faprogcodi },
    //        ApiResponse<FaprogpypDto>.Ok(created, "Programa PyP creado correctamente."));
    //}

    /// <summary>Actualizar programa PyP existente (solo Admin)</summary>
    //[HttpPut("{faprogcodi}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<FaprogpypDto>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Update(string faprogcodi, [FromBody] UpdateFaprogpypDto dto)
    //{
    //    var updated = await _service.UpdateAsync(faprogcodi, dto);
    //    return Ok(ApiResponse<FaprogpypDto>.Ok(updated, "Programa PyP actualizado correctamente."));
    //}

    ///// <summary>Eliminar programa PyP (solo Admin)</summary>
    //[HttpDelete("{faprogcodi}")]
    //[Authorize(Roles = "Admin")]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(string faprogcodi)
    //{
    //    var deleted = await _service.DeleteAsync(faprogcodi);
    //    if (!deleted)
    //        return NotFound(ApiResponse<string>.Fail($"Programa PyP '{faprogcodi}' no encontrado."));
    //    return Ok(ApiResponse<string>.Ok("ok", "Programa PyP eliminado correctamente."));
    //}
}