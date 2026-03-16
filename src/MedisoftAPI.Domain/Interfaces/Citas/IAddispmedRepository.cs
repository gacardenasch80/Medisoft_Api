using MedisoftAPI.Domain.Entities.Citas;

namespace MedisoftAPI.Domain.Interfaces.Citas;

public interface IAddispmedRepository
{
    Task<(IEnumerable<Addispmed> Items, int Total)> GetAllAsync(AddispmedFilter filter);
    Task<Addispmed?> GetByCodeAsync(string addispcons);
    Task<Addispmed> CreateAsync(Addispmed entity);
    Task<Addispmed> UpdateAsync(Addispmed entity);
    Task<bool> DeleteAsync(string addispcons);
}

public class AddispmedFilter
{
    // ── Paginación ─────────────────────────────────────────────
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    // ── Filtros ────────────────────────────────────────────────

    /// <summary>Código consecutivo exacto</summary>
    public string? Addispcons { get; set; }

    /// <summary>Código especialidad exacto</summary>
    public string? Geespecodi { get; set; }

    /// <summary>Código médico exacto</summary>
    public string? Gemedicodi { get; set; }

    /// <summary>Código servicio exacto</summary>
    public string? Faservcodi { get; set; }

    /// <summary>Código consulta exacto</summary>
    public string? Adconscodi { get; set; }

    /// <summary>Fecha inicio del rango</summary>
    public DateTime? FechaInicio { get; set; }

    /// <summary>Fecha fin del rango</summary>
    public DateTime? FechaFin { get; set; }
}
