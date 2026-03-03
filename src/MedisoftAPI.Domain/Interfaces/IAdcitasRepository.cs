using MedisoftAPI.Domain.Entities;

namespace MedisoftAPI.Domain.Interfaces;

public interface IAdcitasRepository
{
    Task<(IEnumerable<Adcitas> Items, int Total)> GetAllAsync(AdcitasFilter filter);
    Task<Adcitas?> GetByCodeAsync(string adadpacons);
    Task<IEnumerable<Adcitas>> GetByPacienteAsync(string adpaciiden);
    Task<Adcitas> CreateAsync(Adcitas entity);
    Task<Adcitas> UpdateAsync(Adcitas entity);
    Task<bool> DeleteAsync(string adadpacons);
}
public class AdcitasFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;


    public string? Adcitacons { get; set; }
    public string? Geespecodi { get; set; }
    public string? Gemedicodi { get; set; }
    public string? Faservcodi { get; set; }
    public string? Adpaciiden { get; set; }
    public string? Adconscodi { get; set; }
    public DateTime? Adfechcita { get; set; }
    public string? Ctadmicodi { get; set; }
    public string? Ctcontcodi { get; set; }
    public string? Adingrcons { get; set; }
}