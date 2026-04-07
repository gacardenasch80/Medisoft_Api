using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.DTOs.Generales;
using MedisoftAPI.Application.Interfaces.Generales;
using MedisoftAPI.Domain.Entities.Generales;
using MedisoftAPI.Domain.Interfaces.Generales;

namespace MedisoftAPI.Application.UseCases.Generales;

public class GeunidprodService : IGeunidprodService
{
    private readonly IGeunidprodRepository _repo;

    public GeunidprodService(IGeunidprodRepository repo) => _repo = repo;

    public async Task<PagedResult<GeunidprodDto>> GetAllAsync(GeunidprodQueryDto query)
    {
        var filter = new GeunidprodFilter
        {
            Geunprcodi = query.Geunprcodi,
            Geunprnomb = query.Geunprnomb,
            Geunprresp = query.Geunprresp,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina,
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<GeunidprodDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total,
        };
    }

    public async Task<GeunidprodDto?> GetByCodeAsync(string geunprcodi)
    {
        var e = await _repo.GetByCodeAsync(geunprcodi);
        return e is null ? null : ToDto(e);
    }

    public async Task<GeunidprodDto> CreateAsync(CreateGeunidprodDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    public async Task<GeunidprodDto> UpdateAsync(string geunprcodi, UpdateGeunidprodDto dto)
    {
        var existing = await _repo.GetByCodeAsync(geunprcodi)
            ?? throw new KeyNotFoundException(
                $"Unidad de producción '{geunprcodi}' no encontrada.");
        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string geunprcodi)
        => _repo.DeleteAsync(geunprcodi);

    // ── Mappers ───────────────────────────────────────────────────────────

    private static GeunidprodDto ToDto(Geunidprod e) => new()
    {
        Geunprcodi = e.Geunprcodi,
        Geunprnomb = e.Geunprnomb,
        Geunprdire = e.Geunprdire,
        Geunprtele = e.Geunprtele,
        Geunprresp = e.Geunprresp,
        Gefarmpref = e.Gefarmpref,
    };

    private static Geunidprod FromCreate(CreateGeunidprodDto d) => new()
    {
        Geunprcodi = d.Geunprcodi.Trim(),
        Geunprnomb = d.Geunprnomb ?? string.Empty,
        Geunprdire = d.Geunprdire ?? string.Empty,
        Geunprtele = d.Geunprtele ?? string.Empty,
        Geunprresp = d.Geunprresp ?? string.Empty,
        Gefarmpref = d.Gefarmpref ?? string.Empty,
    };

    private static void ApplyUpdate(Geunidprod e, UpdateGeunidprodDto d)
    {
        if (d.Geunprnomb is not null) e.Geunprnomb = d.Geunprnomb;
        if (d.Geunprdire is not null) e.Geunprdire = d.Geunprdire;
        if (d.Geunprtele is not null) e.Geunprtele = d.Geunprtele;
        if (d.Geunprresp is not null) e.Geunprresp = d.Geunprresp;
        if (d.Gefarmpref is not null) e.Gefarmpref = d.Gefarmpref;
        // Geunprcodi NO se actualiza — es la clave principal
    }
}