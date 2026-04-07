using MedisoftAPI.Domain.Entities.Citas;

namespace MedisoftAPI.Domain.Interfaces.Citas;

public interface IAdparametrosRepository
{
    Task<(IEnumerable<Adparametros> Items, int Total)> GetAllAsync(AdparametrosFilter filter);
    Task<Adparametros?> GetByCodeAsync(string adparametro);
    Task<Adparametros> CreateAsync(Adparametros entity);
    Task<Adparametros> UpdateAsync(Adparametros entity);
    Task<bool> DeleteAsync(string adparametro);
}

public class AdparametrosFilter
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Adparametro { get; set; }
    public string? Advalorpara { get; set; }
}