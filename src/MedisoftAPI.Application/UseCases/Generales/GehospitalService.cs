using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.DTOs.Generales;
using MedisoftAPI.Application.Interfaces.Generales;
using MedisoftAPI.Domain.Entities.Generales;
using MedisoftAPI.Domain.Interfaces.Generales;

namespace MedisoftAPI.Application.UseCases.Generales;

public class GehospitalService : IGehospitalService
{
    private readonly IGehospitalRepository _repo;

    public GehospitalService(IGehospitalRepository repo) => _repo = repo;

    public async Task<PagedResult<GehospitalDto>> GetAllAsync(GehospitalQueryDto query)
    {
        var filter = new GehospitalFilter
        {
            Gehospcodi = query.Gehospcodi,
            Gehospnomb = query.Gehospnomb,
            Gehospnit = query.Gehospnit,
            Gedepacodi = query.Gedepacodi,
            Gemunicodi = query.Gemunicodi,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina,
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<GehospitalDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total,
        };
    }

    public async Task<GehospitalDto?> GetByCodeAsync(string gehospcodi)
    {
        var e = await _repo.GetByCodeAsync(gehospcodi);
        return e is null ? null : ToDto(e);
    }

    public async Task<GehospitalDto> CreateAsync(CreateGehospitalDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    public async Task<GehospitalDto> UpdateAsync(string gehospcodi, UpdateGehospitalDto dto)
    {
        var existing = await _repo.GetByCodeAsync(gehospcodi)
            ?? throw new KeyNotFoundException($"Hospital '{gehospcodi}' no encontrado.");
        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string gehospcodi)
        => _repo.DeleteAsync(gehospcodi);

    // ── Mappers ───────────────────────────────────────────────────────────

    private static GehospitalDto ToDto(Gehospital e) => new()
    {
        Gehospcodi = e.Gehospcodi,
        Gehospnomb = e.Gehospnomb,
        Gehosptiid = e.Gehosptiid,
        Gehospnit = e.Gehospnit,
        Gehospdire = e.Gehospdire,
        Gehosptele = e.Gehosptele,
        Gehospreso = e.Gehospreso,
        Gehospisgss = e.Gehospisgss,
        Gehosprele = e.Gehosprele,
        Gehospemai = e.Gehospemai,
        Gehosppagi = e.Gehosppagi,
        Gedepacodi = e.Gedepacodi,
        Gemunicodi = e.Gemunicodi,
        Gehosprefa = e.Gehosprefa,
        Gehospreec = e.Gehospreec,
        Geresudian = e.Geresudian,
        Gemensage = e.Gemensage,
        Gehospdigi = e.Gehospdigi,
        Gehospindi = e.Gehospindi,
        Gehospext = e.Gehospext,
        Gemoduacti = e.Gemoduacti,
        Gehoenviro = e.Gehoenviro,
        Gehosetpru = e.Gehosetpru,
        Gehocurren = e.Gehocurren,
        Gehocertif = e.Gehocertif,
        Gehosubtyp = e.Gehosubtyp,
        Faferecodi = e.Faferecodi,
        Gehofaeles = e.Gehofaeles,
        Gehomailsi = e.Gehomailsi,
        Fafetifcod = e.Fafetifcod,
        Gehostyper = e.Gehostyper,
        Gehosrerut = e.Gehosrerut,
        Gehosquaid = e.Gehosquaid,
        Gehosquaes = e.Gehosquaes,
        Gehosproxy = e.Gehosproxy,
        Gehoslogid = e.Gehoslogid,
        Gehosimgid = e.Gehosimgid,
        Gehocerdia = e.Gehocerdia,
        Fafetpdcod = e.Fafetpdcod,
    };

    private static Gehospital FromCreate(CreateGehospitalDto d) => new()
    {
        Gehospcodi = d.Gehospcodi,
        Gehospnomb = d.Gehospnomb ?? string.Empty,
        Gehosptiid = d.Gehosptiid ?? string.Empty,
        Gehospnit = d.Gehospnit ?? string.Empty,
        Gehospdire = d.Gehospdire ?? string.Empty,
        Gehosptele = d.Gehosptele ?? string.Empty,
        Gehospreso = d.Gehospreso ?? string.Empty,
        Gehospisgss = d.Gehospisgss ?? string.Empty,
        Gehosprele = d.Gehosprele ?? string.Empty,
        Gehospemai = d.Gehospemai ?? string.Empty,
        Gehosppagi = d.Gehosppagi ?? string.Empty,
        Gedepacodi = d.Gedepacodi ?? string.Empty,
        Gemunicodi = d.Gemunicodi ?? string.Empty,
        Gehosprefa = d.Gehosprefa ?? string.Empty,
        Gehospreec = d.Gehospreec ?? string.Empty,
        Geresudian = d.Geresudian ?? string.Empty,
        Gemensage = d.Gemensage ?? string.Empty,
        Gehospdigi = d.Gehospdigi ?? string.Empty,
        Gehospindi = d.Gehospindi ?? string.Empty,
        Gehospext = d.Gehospext ?? string.Empty,
        Gemoduacti = d.Gemoduacti,
        Gehoenviro = d.Gehoenviro,
        Gehosetpru = d.Gehosetpru ?? string.Empty,
        Gehocurren = d.Gehocurren ?? string.Empty,
        Gehocertif = d.Gehocertif ?? string.Empty,
        Gehosubtyp = d.Gehosubtyp,
        Faferecodi = d.Faferecodi ?? string.Empty,
        Gehofaeles = d.Gehofaeles,
        Gehomailsi = d.Gehomailsi ?? string.Empty,
        Fafetifcod = d.Fafetifcod ?? string.Empty,
        Gehostyper = d.Gehostyper,
        Gehosrerut = d.Gehosrerut ?? string.Empty,
        Gehosquaid = d.Gehosquaid ?? string.Empty,
        Gehosquaes = d.Gehosquaes,
        Gehosproxy = d.Gehosproxy ?? string.Empty,
        Gehoslogid = d.Gehoslogid ?? string.Empty,
        Gehosimgid = d.Gehosimgid ?? string.Empty,
        Gehocerdia = d.Gehocerdia ?? string.Empty,
        Fafetpdcod = d.Fafetpdcod ?? string.Empty,
    };

    private static void ApplyUpdate(Gehospital e, UpdateGehospitalDto d)
    {
        if (d.Gehospnomb is not null) e.Gehospnomb = d.Gehospnomb;
        if (d.Gehosptiid is not null) e.Gehosptiid = d.Gehosptiid;
        if (d.Gehospnit is not null) e.Gehospnit = d.Gehospnit;
        if (d.Gehospdire is not null) e.Gehospdire = d.Gehospdire;
        if (d.Gehosptele is not null) e.Gehosptele = d.Gehosptele;
        if (d.Gehospreso is not null) e.Gehospreso = d.Gehospreso;
        if (d.Gehospisgss is not null) e.Gehospisgss = d.Gehospisgss;
        if (d.Gehosprele is not null) e.Gehosprele = d.Gehosprele;
        if (d.Gehospemai is not null) e.Gehospemai = d.Gehospemai;
        if (d.Gehosppagi is not null) e.Gehosppagi = d.Gehosppagi;
        if (d.Gedepacodi is not null) e.Gedepacodi = d.Gedepacodi;
        if (d.Gemunicodi is not null) e.Gemunicodi = d.Gemunicodi;
        if (d.Gehosprefa is not null) e.Gehosprefa = d.Gehosprefa;
        if (d.Gehospreec is not null) e.Gehospreec = d.Gehospreec;
        if (d.Geresudian is not null) e.Geresudian = d.Geresudian;
        if (d.Gemensage is not null) e.Gemensage = d.Gemensage;
        if (d.Gehospdigi is not null) e.Gehospdigi = d.Gehospdigi;
        if (d.Gehospindi is not null) e.Gehospindi = d.Gehospindi;
        if (d.Gehospext is not null) e.Gehospext = d.Gehospext;
        if (d.Gemoduacti is not null) e.Gemoduacti = d.Gemoduacti;
        if (d.Gehoenviro is not null) e.Gehoenviro = d.Gehoenviro;
        if (d.Gehosetpru is not null) e.Gehosetpru = d.Gehosetpru;
        if (d.Gehocurren is not null) e.Gehocurren = d.Gehocurren;
        if (d.Gehocertif is not null) e.Gehocertif = d.Gehocertif;
        if (d.Gehosubtyp is not null) e.Gehosubtyp = d.Gehosubtyp;
        if (d.Faferecodi is not null) e.Faferecodi = d.Faferecodi;
        if (d.Gehofaeles is not null) e.Gehofaeles = d.Gehofaeles;
        if (d.Gehomailsi is not null) e.Gehomailsi = d.Gehomailsi;
        if (d.Fafetifcod is not null) e.Fafetifcod = d.Fafetifcod;
        if (d.Gehostyper is not null) e.Gehostyper = d.Gehostyper;
        if (d.Gehosrerut is not null) e.Gehosrerut = d.Gehosrerut;
        if (d.Gehosquaid is not null) e.Gehosquaid = d.Gehosquaid;
        if (d.Gehosquaes is not null) e.Gehosquaes = d.Gehosquaes;
        if (d.Gehosproxy is not null) e.Gehosproxy = d.Gehosproxy;
        if (d.Gehoslogid is not null) e.Gehoslogid = d.Gehoslogid;
        if (d.Gehosimgid is not null) e.Gehosimgid = d.Gehosimgid;
        if (d.Gehocerdia is not null) e.Gehocerdia = d.Gehocerdia;
        if (d.Fafetpdcod is not null) e.Fafetpdcod = d.Fafetpdcod;
        // Nota: Gehospcodi NO se actualiza — es la clave principal
    }
}