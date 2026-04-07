using MedisoftAPI.Domain.Entities.Generales;

namespace MedisoftAPI.Domain.Interfaces.Generales;

public interface IGehospitalRepository
{
    Task<(IEnumerable<Gehospital> Items, int Total)> GetAllAsync(GehospitalFilter filter);
    Task<Gehospital?> GetByCodeAsync(string gehospcodi);
    Task<Gehospital> CreateAsync(Gehospital entity);
    Task<Gehospital> UpdateAsync(Gehospital entity);
    Task<bool> DeleteAsync(string gehospcodi);
}

public class GehospitalFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Gehospcodi { get; set; }
    public string? Gehospnomb { get; set; }
    public string? Gehospnit { get; set; }
    public string? Gedepacodi { get; set; }
    public string? Gemunicodi { get; set; }
}