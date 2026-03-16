using MedisoftAPI.Domain.Entities.Admision;

namespace MedisoftAPI.Domain.Interfaces.Admision;

public interface IAdadmipaciRepository
{
    Task<(IEnumerable<Adadmipaci> Items, int Total)> GetAllAsync(AdadmipaciFilter filter);
    Task<Adadmipaci?> GetByCodeAsync(string adadpacons);
    Task<Adadmipaci> CreateAsync(Adadmipaci entity);
    Task<Adadmipaci> UpdateAsync(Adadmipaci entity);
    Task<bool> DeleteAsync(string adadpacons);
}
public class AdadmipaciFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    public string? ADADPACONS { get; set; }
    public string? CTADMICODI { get; set; }
    public string? ADPACIIDEN { get; set; }
    public string? CTCONTCODI { get; set; }
    public int? ADADPAESTA { get; set; }
    public string? CTNIVECODI { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}