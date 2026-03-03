using MedisoftAPI.Domain.Entities;

namespace MedisoftAPI.Domain.Interfaces;

public interface IAdpacienteRepository
{
    Task<(IEnumerable<Adpaciente> Items, int Total)> GetAllAsync(AdpacienteFilter filter);
    Task<Adpaciente?> GetByCodeAsync(string codserv);
    Task<Adpaciente> CreateAsync(Adpaciente entity);
    Task<Adpaciente> UpdateAsync(Adpaciente entity);
    Task<bool> DeleteAsync(string codserv);
}

public class AdpacienteFilter
{
    public string? ADTIIDCODI { get; set; }
    public string? ADPACIIDEN { get; set; }
    public string? ADPACIAPE1 { get; set; }
    public string? ADPACIAPE2 { get; set; }
    public string? ADPACINOM1 { get; set; }
    public string ADPACINOM2 { get; set; }

    // Paginación
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
}
