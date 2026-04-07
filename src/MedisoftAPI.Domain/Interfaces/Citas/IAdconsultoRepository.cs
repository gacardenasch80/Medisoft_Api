using MedisoftAPI.Domain.Entities.Citas;

namespace MedisoftAPI.Domain.Interfaces.Citas;

public interface IAdconsultoRepository
{
    Task<(IEnumerable<Adconsulto> Items, int Total)> GetAllAsync(AdconsultoFilter filter);
    Task<Adconsulto?> GetByCodeAsync(string adconscodi);
    /// <summary>
    /// Devuelve todos los consultorios que pertenecen a alguna de las
    /// unidades de producción indicadas (Geunprcodi IN lista).
    /// </summary>
    Task<IEnumerable<Adconsulto>> GetByUnidadesAsync(IEnumerable<string> geunprcodi);
    Task<Adconsulto> CreateAsync(Adconsulto entity);
    Task<Adconsulto> UpdateAsync(Adconsulto entity);
    Task<bool> DeleteAsync(string adconscodi);
}

public class AdconsultoFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Adconscodi { get; set; }
    public string? Adconsnomb { get; set; }
    public string? Adconsdire { get; set; }
    public string? Adconstele { get; set; }
    public string? Geunprcodi { get; set; }
}