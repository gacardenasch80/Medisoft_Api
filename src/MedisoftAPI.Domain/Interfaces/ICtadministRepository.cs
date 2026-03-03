using MedisoftAPI.Domain.Entities;

namespace MedisoftAPI.Domain.Interfaces;

public interface ICtadministRepository
{
    Task<(IEnumerable<Ctadminist> Items, int Total)> GetAllAsync(CtadministFilter filter);
    Task<Ctadminist?> GetByCodeAsync(string ctadmicodi);
    Task<Ctadminist> CreateAsync(Ctadminist entity);
    Task<Ctadminist> UpdateAsync(Ctadminist entity);
    Task<bool> DeleteAsync(string ctadmicodi);
}
public class CtadministFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    public string? CTADMICODI { get; set; }
    public string? CTADMINOMB { get; set; }
    public string? CTADMISGSS { get; set; }
    public string? GEDEPACODI { get; set; }
    public string? GEMUNICODI { get; set; }
    public int? CTADMIESTA { get; set; }
}