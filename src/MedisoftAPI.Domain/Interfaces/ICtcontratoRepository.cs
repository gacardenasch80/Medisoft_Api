using MedisoftAPI.Domain.Entities;

namespace MedisoftAPI.Domain.Interfaces;

public interface ICtcontratoRepository
{
    Task<(IEnumerable<Ctcontrato> Items, int Total)> GetAllAsync(CtcontratoFilter filter);
    Task<Ctcontrato?> GetByCodeAsync(string ctcontcodi);
    Task<Ctcontrato> CreateAsync(Ctcontrato entity);
    Task<Ctcontrato> UpdateAsync(Ctcontrato entity);
    Task<bool> DeleteAsync(string ctcontcodi);
}

public class CtcontratoFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    public string? CTCONTCODI { get; set; }
    public string? CTCONTNUME { get; set; }
    public string? CTADMICODI { get; set; }
    public string? CTREGICODI { get; set; }
    public string? CTCONTESTA { get; set; }
    public string? CTTICOCODI { get; set; }
    public string? FAFEMPCODI { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
}