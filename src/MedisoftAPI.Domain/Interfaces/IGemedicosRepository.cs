using MedisoftAPI.Domain.Entities;

namespace MedisoftAPI.Domain.Interfaces;

public interface IGemedicosRepository
{
    Task<(IEnumerable<Gemedicos> Items, int Total)> GetAllAsync(GemedicosFilter filter);
    Task<Gemedicos?> GetByCodeAsync(string codserv);
    Task<Gemedicos> CreateAsync(Gemedicos entity);
    Task<Gemedicos> UpdateAsync(Gemedicos entity);
    Task<bool> DeleteAsync(string codserv);
}

public class GemedicosFilter
{
    // ── Paginación ─────────────────────────────────────────────
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    // ── Filtros ────────────────────────────────────────────────

    /// <summary>Código médico exacto</summary>
    public string? Gemedicodi { get; set; }

    /// <summary>Nombre médico (búsqueda parcial LIKE)</summary>
    public string? Gemedinomb { get; set; }

    /// <summary>Código de reinducción exacto</summary>
    public string? Gereincodi { get; set; }

    /// <summary>Estado activo: 1 = activo, 0 = inactivo</summary>
    public int? Gemedact { get; set; }
}
