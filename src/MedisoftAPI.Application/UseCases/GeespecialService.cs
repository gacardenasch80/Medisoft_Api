using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;

namespace MedisoftAPI.Application.UseCases;

public class GeespecialService : IGeespecialService
{
    private readonly IGeespecialRepository _repo;

    public GeespecialService(IGeespecialRepository repo) => _repo = repo;

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<PagedResult<GeespecialDto>> GetAllAsync(GeespecialQueryDto query)
    {
        var filter = new GeespecialFilter
        {
            Geespecodi = query.Geespecodi,
            Geespenomb = query.Geespenomb,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<GeespecialDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total
        };
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<GeespecialDto?> GetByCodeAsync(string geespecodi)
    {
        var e = await _repo.GetByCodeAsync(geespecodi);
        return e is null ? null : ToDto(e);
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<GeespecialDto> CreateAsync(CreateGeespecialDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<GeespecialDto> UpdateAsync(string geespecodi, UpdateGeespecialDto dto)
    {
        var existing = await _repo.GetByCodeAsync(geespecodi)
            ?? throw new KeyNotFoundException(
                $"Especialidad '{geespecodi}' no encontrada.");

        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public Task<bool> DeleteAsync(string geespecodi)
        => _repo.DeleteAsync(geespecodi);

    // ── Mappers ───────────────────────────────────────────────────────────

    private static GeespecialDto ToDto(Geespecial e) => new()
    {
        Geespecodi = e.Geespecodi,
        Geespenomb = e.Geespenomb,
        Geespesv18 = e.Geespesv18,
        Geespeodon = e.Geespeodon,
        Hcrevartip = e.Hcrevartip,
    };

    private static Geespecial FromCreate(CreateGeespecialDto d) => new()
    {
        Geespecodi = d.Geespecodi,
        Geespenomb = d.Geespenomb,
        Geespesv18 = d.Geespesv18,
        Geespeodon = d.Geespeodon,
        Hcrevartip = d.Hcrevartip,
    };

    private static void ApplyUpdate(Geespecial e, UpdateGeespecialDto d)
    {
        e.Geespenomb = d.Geespenomb;
        e.Geespesv18 = d.Geespesv18;
        e.Geespeodon = d.Geespeodon;
        e.Hcrevartip = d.Hcrevartip;
        // Nota: Geespecodi NO se actualiza — es la clave principal
    }
}