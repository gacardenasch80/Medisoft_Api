using MedisoftAPI.Domain.Entities.Generales;

namespace MedisoftAPI.Domain.Interfaces.Generales;

public interface IGeunidprodRepository
{
    Task<(IEnumerable<Geunidprod> Items, int Total)> GetAllAsync(GeunidprodFilter filter);
    Task<Geunidprod?> GetByCodeAsync(string geunprcodi);
    Task<Geunidprod> CreateAsync(Geunidprod entity);
    Task<Geunidprod> UpdateAsync(Geunidprod entity);
    Task<bool> DeleteAsync(string geunprcodi);
}

public class GeunidprodFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Geunprcodi { get; set; }
    public string? Geunprnomb { get; set; }
    public string? Geunprresp { get; set; }
}