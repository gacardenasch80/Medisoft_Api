using MedisoftAPI.Application.DTOs.Contratacion;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Contratacion;
using MedisoftAPI.Domain.Entities.Contratacion;
using MedisoftAPI.Domain.Interfaces.Contratacion;

namespace MedisoftAPI.Application.UseCases.Contratacion;

public class CtadministService : ICtadministService
{
    private readonly ICtadministRepository _repo;

    public CtadministService(ICtadministRepository repo) => _repo = repo;

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<PagedResult<CtadministDto>> GetAllAsync(CtadministQueryDto query)
    {
        var filter = new CtadministFilter
        {
            CTADMICODI = query.CTADMICODI,
            CTADMINOMB = query.CTADMINOMB,
            CTADMISGSS = query.CTADMISGSS,
            GEDEPACODI = query.GEDEPACODI,
            GEMUNICODI = query.GEMUNICODI,
            CTADMIESTA = query.CTADMIESTA,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<CtadministDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total
        };
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<CtadministDto?> GetByCodeAsync(string ctadmicodi)
    {
        var e = await _repo.GetByCodeAsync(ctadmicodi);
        return e is null ? null : ToDto(e);
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<CtadministDto> CreateAsync(CreateCtadministDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<CtadministDto> UpdateAsync(string ctadmicodi, UpdateCtadministDto dto)
    {
        var existing = await _repo.GetByCodeAsync(ctadmicodi)
            ?? throw new KeyNotFoundException(
                $"Administradora '{ctadmicodi}' no encontrada.");

        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public Task<bool> DeleteAsync(string ctadmicodi)
        => _repo.DeleteAsync(ctadmicodi);

    // ── Mappers ───────────────────────────────────────────────────────────

    private static CtadministDto ToDto(Ctadminist e) => new()
    {
        CTADMICODI = e.CTADMICODI,
        CTADMINOMB = e.CTADMINOMB,
        CTADMINIT = e.CTADMINIT,
        CTADMISGSS = e.CTADMISGSS,
        CTADMIDIRE = e.CTADMIDIRE,
        CTADMITELE = e.CTADMITELE,
        CTADMIRELE = e.CTADMIRELE,
        CTADMIEMAI = e.CTADMIEMAI,
        CTADMIPAGI = e.CTADMIPAGI,
        GEDEPACODI = e.GEDEPACODI,
        GEMUNICODI = e.GEMUNICODI,
        CTCONCOPYP = e.CTCONCOPYP,
        CTADMIGIDI = e.CTADMIGIDI,
        FAFERECODI = e.FAFERECODI,
        FAFETIFCOD = e.FAFETIFCOD,
        CTADMTYPER = e.CTADMTYPER,
        CTADMRERUT = e.CTADMRERUT,
        CTADMIESTA = e.CTADMIESTA,
        CTADMSGFAR = e.CTADMSGFAR,
    };

    private static Ctadminist FromCreate(CreateCtadministDto d) => new()
    {
        CTADMICODI = d.CTADMICODI,
        CTADMINOMB = d.CTADMINOMB,
        CTADMINIT = d.CTADMINIT,
        CTADMISGSS = d.CTADMISGSS,
        CTADMIDIRE = d.CTADMIDIRE,
        CTADMITELE = d.CTADMITELE,
        CTADMIRELE = d.CTADMIRELE,
        CTADMIEMAI = d.CTADMIEMAI,
        CTADMIPAGI = d.CTADMIPAGI,
        GEDEPACODI = d.GEDEPACODI,
        GEMUNICODI = d.GEMUNICODI,
        CTCONCOPYP = d.CTCONCOPYP,
        CTADMIGIDI = d.CTADMIGIDI,
        FAFERECODI = d.FAFERECODI,
        FAFETIFCOD = d.FAFETIFCOD,
        CTADMTYPER = d.CTADMTYPER,
        CTADMRERUT = d.CTADMRERUT,
        CTADMIESTA = d.CTADMIESTA,
        CTADMSGFAR = d.CTADMSGFAR,
    };

    private static void ApplyUpdate(Ctadminist e, UpdateCtadministDto d)
    {
        e.CTADMINOMB = d.CTADMINOMB;
        e.CTADMINIT = d.CTADMINIT;
        e.CTADMISGSS = d.CTADMISGSS;
        e.CTADMIDIRE = d.CTADMIDIRE;
        e.CTADMITELE = d.CTADMITELE;
        e.CTADMIRELE = d.CTADMIRELE;
        e.CTADMIEMAI = d.CTADMIEMAI;
        e.CTADMIPAGI = d.CTADMIPAGI;
        e.GEDEPACODI = d.GEDEPACODI;
        e.GEMUNICODI = d.GEMUNICODI;
        e.CTCONCOPYP = d.CTCONCOPYP;
        e.CTADMIGIDI = d.CTADMIGIDI;
        e.FAFERECODI = d.FAFERECODI;
        e.FAFETIFCOD = d.FAFETIFCOD;
        e.CTADMTYPER = d.CTADMTYPER;
        e.CTADMRERUT = d.CTADMRERUT;
        e.CTADMIESTA = d.CTADMIESTA;
        e.CTADMSGFAR = d.CTADMSGFAR;
        // Nota: CTADMICODI NO se actualiza — es la clave principal
    }
}