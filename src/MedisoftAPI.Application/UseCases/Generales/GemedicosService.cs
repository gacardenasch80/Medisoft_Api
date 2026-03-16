using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.DTOs.Generales;
using MedisoftAPI.Application.Interfaces.Generales;
using MedisoftAPI.Domain.Entities.Generales;
using MedisoftAPI.Domain.Interfaces.Generales;

namespace MedisoftAPI.Application.UseCases.Generales;

public class GemedicosService : IGemedicosService
{
    private readonly IGemedicosRepository _repo;

    public GemedicosService(IGemedicosRepository repo) => _repo = repo;

    public async Task<PagedResult<GemedicosDto>> GetAllAsync(GemedicosQueryDto query)
    {
        var filter = new GemedicosFilter
        {
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<GemedicosDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total
        };
    }

    public async Task<GemedicosDto?> GetByCodeAsync(string codserv)
    {
        var e = await _repo.GetByCodeAsync(codserv);
        return e is null ? null : ToDto(e);
    }

    public async Task<GemedicosDto> CreateAsync(CreateGemedicosDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    public async Task<GemedicosDto> UpdateAsync(string codserv, UpdateGemedicosDto dto)
    {
        var existing = await _repo.GetByCodeAsync(codserv)
            ?? throw new KeyNotFoundException($"Servicio '{codserv}' no encontrado.");
        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string codserv) => _repo.DeleteAsync(codserv);

    // ── Mappers ────────────────────────────────────────────────
    private static GemedicosDto ToDto(Gemedicos e) => new()
    {
        Gemedicodi = e.Gemedicodi,
        Gemedinomb = e.Gemedinomb,
        Gemedireme = e.Gemedireme,
        Gereincodi = e.Gereincodi,
        Gefirmesca = e.Gefirmesca,
        Gemeditele = e.Gemeditele,
        Gemedicelu = e.Gemedicelu,
        Gemedact = e.Gemedact,
        Geesjefeen = e.Geesjefeen
    };

    private static Gemedicos FromCreate(CreateGemedicosDto d) => new()
    {
        Gemedicodi = d.Gemedicodi,
        Gemedinomb = d.Gemedinomb,
        Gemedireme = d.Gemedireme,
        Gereincodi = d.Gereincodi,
        Gefirmesca = d.Gefirmesca,
        Gemeditele = d.Gemeditele,
        Gemedicelu = d.Gemedicelu,
        Gemedact = d.Gemedact,
        Geesjefeen = d.Geesjefeen
    };

    private static void ApplyUpdate(Gemedicos e, UpdateGemedicosDto d)
    {
        e.Gemedicodi = d.Gemedicodi;
        e.Gemedinomb = d.Gemedinomb;
        e.Gemedireme = d.Gemedireme;;
        e.Gereincodi = d.Gereincodi;;
        e.Gefirmesca = d.Gefirmesca;;
        e.Gemeditele = d.Gemeditele;;
        e.Gemedicelu = d.Gemedicelu;;
        e.Gemedact = d.Gemedact;;
        e.Geesjefeen = d.Geesjefeen;
    }
}
