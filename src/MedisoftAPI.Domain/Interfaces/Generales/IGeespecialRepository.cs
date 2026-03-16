using MedisoftAPI.Domain.Entities.Generales;

namespace MedisoftAPI.Domain.Interfaces.Generales;

public interface IGeespecialRepository
{
    Task<(IEnumerable<Geespecial> Items, int Total)> GetAllAsync(GeespecialFilter filter);
    Task<Geespecial?> GetByCodeAsync(string geespecodi);
    Task<Geespecial> CreateAsync(Geespecial entity);
    Task<Geespecial> UpdateAsync(Geespecial entity);
    Task<bool> DeleteAsync(string geespecodi);
}

public class GeespecialFilter
{
    // ── Paginación ─────────────────────────────────────────────
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    // ── Filtros ────────────────────────────────────────────────

    /// <summary>Código especialidad exacto</summary>
    public string? Geespecodi { get; set; }

    /// <summary>Nombre especialidad (búsqueda parcial LIKE)</summary>
    public string? Geespenomb { get; set; }
}