using MedisoftAPI.Domain.Entities;

namespace MedisoftAPI.Domain.Interfaces;

public interface IFaprogpypRepository
{
    Task<(IEnumerable<Faprogpyp> Items, int Total)> GetAllAsync(FaprogpypFilter filter);
    Task<Faprogpyp?> GetByCodeAsync(string faprogcodi);
    Task<Faprogpyp> CreateAsync(Faprogpyp entity);
    Task<Faprogpyp> UpdateAsync(Faprogpyp entity);
    Task<bool> DeleteAsync(string faprogcodi);
}

public class FaprogpypFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Faprogcodi { get; set; }
    public string? Faprogcod1 { get; set; }
    public string? Faprognomb { get; set; }
    public string? Faprogclas { get; set; }
    public string? Faservcodi { get; set; }
    public int? Faprogestad { get; set; }
}