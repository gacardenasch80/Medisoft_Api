using MedisoftAPI.Domain.Entities;

namespace MedisoftAPI.Domain.Interfaces;

public interface IFaservicioRepository
{
    Task<(IEnumerable<Faservicio> Items, int Total)> GetAllAsync(FaservicioFilter filter);
    Task<Faservicio?>                                GetByCodeAsync(string codserv);
    Task<Faservicio>                                 CreateAsync(Faservicio entity);
    Task<Faservicio>                                 UpdateAsync(Faservicio entity);
    Task<bool>                                       DeleteAsync(string codserv);
}

public class FaservicioFilter
{
    public string? FASERVCODI { get; set; }
    public string? FASERVNOMB { get; set; }
    public string? FASERVESTA { get; set; }
    public string? CTCLMACODI { get; set; }
    public string? FACLSECODI { get; set; }
    public int?    FASERVTIPO { get; set; }

    // Paginación
    public int Pagina    { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
}
