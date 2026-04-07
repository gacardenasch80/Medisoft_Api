using MedisoftAPI.Domain.Entities.Citas;

namespace MedisoftAPI.Domain.Interfaces.Citas;

public interface IAdzonificaRepository
{
    Task<(IEnumerable<Adzonifica> Items, int Total)> GetAllAsync(AdzonificaFilter filter);
    Task<Adzonifica?> GetByPacienteYZonaAsync(string adpaciiden, string geunprcodi);
    Task<IEnumerable<Adzonifica>> GetByPacienteAsync(string adpaciiden);
    Task<Adzonifica> CreateAsync(Adzonifica entity);
    Task<Adzonifica> UpdateAsync(Adzonifica entity);
    Task<bool> DeleteAsync(string adpaciiden, string geunprcodi);
}

public class AdzonificaFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Adpaciiden { get; set; }
    public string? Geunprcodi { get; set; }
    public double? Estado { get; set; }
}