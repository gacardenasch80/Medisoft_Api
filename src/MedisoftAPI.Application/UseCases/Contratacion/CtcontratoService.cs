using MedisoftAPI.Application.DTOs.Contratacion;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Contratacion;
using MedisoftAPI.Domain.Entities.Contratacion;
using MedisoftAPI.Domain.Interfaces.Contratacion;

namespace MedisoftAPI.Application.UseCases.Contratacion;

public class CtcontratoService : ICtcontratoService
{
    private readonly ICtcontratoRepository _repo;

    public CtcontratoService(ICtcontratoRepository repo) => _repo = repo;

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<PagedResult<CtcontratoDto>> GetAllAsync(CtcontratoQueryDto query)
    {
        var filter = new CtcontratoFilter
        {
            CTCONTCODI = query.CTCONTCODI,
            CTCONTNUME = query.CTCONTNUME,
            CTADMICODI = query.CTADMICODI,
            CTREGICODI = query.CTREGICODI,
            CTCONTESTA = query.CTCONTESTA,
            CTTICOCODI = query.CTTICOCODI,
            FAFEMPCODI = query.FAFEMPCODI,
            FechaDesde = query.FechaDesde,
            FechaHasta = query.FechaHasta,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<CtcontratoDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total
        };
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<CtcontratoDto?> GetByCodeAsync(string ctcontcodi)
    {
        var e = await _repo.GetByCodeAsync(ctcontcodi);
        return e is null ? null : ToDto(e);
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<CtcontratoDto> CreateAsync(CreateCtcontratoDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<CtcontratoDto> UpdateAsync(string ctcontcodi, UpdateCtcontratoDto dto)
    {
        var existing = await _repo.GetByCodeAsync(ctcontcodi)
            ?? throw new KeyNotFoundException(
                $"Contrato '{ctcontcodi}' no encontrado.");

        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public Task<bool> DeleteAsync(string ctcontcodi)
        => _repo.DeleteAsync(ctcontcodi);

    // ── Mappers ───────────────────────────────────────────────────────────

    private static CtcontratoDto ToDto(Ctcontrato e) => new()
    {
        CTCONTCODI = e.CTCONTCODI,
        CTCONTNUME = e.CTCONTNUME,
        CTADMICODI = e.CTADMICODI,
        CTREGICODI = e.CTREGICODI,
        CTPLBECODI = e.CTPLBECODI,
        CTMANUCODI = e.CTMANUCODI,
        CTCONTLEGA = e.CTCONTLEGA,
        CTCONTFELE = e.CTCONTFELE,
        CTCONTDESD = e.CTCONTDESD,
        CTCONTHAST = e.CTCONTHAST,
        CTCONTVALO = e.CTCONTVALO,
        CTCONTESTA = e.CTCONTESTA,
        CTTICOCODI = e.CTTICOCODI,
        CTCONTASIG = e.CTCONTASIG,
        CTCONTFACT = e.CTCONTFACT,
        FAOBSHORA = e.FAOBSHORA,
        CTCTAACRE = e.CTCTAACRE,
        CTCITXDIA = e.CTCITXDIA,
        CTCONTCONS = e.CTCONTCONS,
        CTCONTFECR = e.CTCONTFECR,
        CTCONTUSCR = e.CTCONTUSCR,
        CTCONTFEED = e.CTCONTFEED,
        CTCONTUSED = e.CTCONTUSED,
        CTCONTVADE = e.CTCONTVADE,
        CTCONTPYP = e.CTCONTPYP,
        CTMANUPROD = e.CTMANUPROD,
        PUCDEBRADI = e.PUCDEBRADI,
        FAPRESUDEB = e.FAPRESUDEB,
        FAPRESUCRE = e.FAPRESUCRE,
        PUCDEBCOPA = e.PUCDEBCOPA,
        PREDEBCOPA = e.PREDEBCOPA,
        PRECRECOPA = e.PRECRECOPA,
        PRCRREVACT = e.PRCRREVACT,
        PRDEREVAFA = e.PRDEREVAFA,
        PRCRREVAFA = e.PRCRREVAFA,
        PRDEREVARE = e.PRDEREVARE,
        PRCRREVARE = e.PRCRREVARE,
        PUCDEVA360 = e.PUCDEVA360,
        PRDEREVEFA = e.PRDEREVEFA,
        PRCRREVEFA = e.PRCRREVEFA,
        PRDEREVERE = e.PRDEREVERE,
        PRCRREVERE = e.PRCRREVERE,
        PUCDEGLOSA = e.PUCDEGLOSA,
        PUCCRGLOSA = e.PUCCRGLOSA,
        PUCDEGLOVA = e.PUCDEGLOVA,
        PUCCRGLOVA = e.PUCCRGLOVA,
        PCRGLVA360 = e.PCRGLVA360,
        PUCBANCO = e.PUCBANCO,
        CTCONTCANFA = e.CTCONTCANFA,
        PUCDEBCOUR = e.PUCDEBCOUR,
        PUCDEBCOHO = e.PUCDEBCOHO,
        CTCONTFOPA = e.CTCONTFOPA,
        FAFEMPCODI = e.FAFEMPCODI,
        CTCONTEMSU = e.CTCONTEMSU,
        CTCONTSOAT = e.CTCONTSOAT,
        CTCATACODI = e.CTCATACODI,
    };

    private static Ctcontrato FromCreate(CreateCtcontratoDto d) => new()
    {
        CTCONTCODI = d.CTCONTCODI,
        CTCONTNUME = d.CTCONTNUME,
        CTADMICODI = d.CTADMICODI,
        CTREGICODI = d.CTREGICODI,
        CTPLBECODI = d.CTPLBECODI,
        CTMANUCODI = d.CTMANUCODI,
        CTCONTLEGA = d.CTCONTLEGA,
        CTCONTFELE = d.CTCONTFELE,
        CTCONTDESD = d.CTCONTDESD,
        CTCONTHAST = d.CTCONTHAST,
        CTCONTVALO = d.CTCONTVALO,
        CTCONTESTA = d.CTCONTESTA,
        CTTICOCODI = d.CTTICOCODI,
        CTCONTASIG = d.CTCONTASIG,
        CTCONTFACT = d.CTCONTFACT,
        FAOBSHORA = d.FAOBSHORA,
        CTCTAACRE = d.CTCTAACRE,
        CTCITXDIA = d.CTCITXDIA,
        CTCONTCONS = d.CTCONTCONS,
        CTCONTVADE = d.CTCONTVADE,
        CTCONTPYP = d.CTCONTPYP,
        CTMANUPROD = d.CTMANUPROD,
        PUCDEBRADI = d.PUCDEBRADI,
        FAPRESUDEB = d.FAPRESUDEB,
        FAPRESUCRE = d.FAPRESUCRE,
        PUCDEBCOPA = d.PUCDEBCOPA,
        PREDEBCOPA = d.PREDEBCOPA,
        PRECRECOPA = d.PRECRECOPA,
        PRCRREVACT = d.PRCRREVACT,
        PRDEREVAFA = d.PRDEREVAFA,
        PRCRREVAFA = d.PRCRREVAFA,
        PRDEREVARE = d.PRDEREVARE,
        PRCRREVARE = d.PRCRREVARE,
        PUCDEVA360 = d.PUCDEVA360,
        PRDEREVEFA = d.PRDEREVEFA,
        PRCRREVEFA = d.PRCRREVEFA,
        PRDEREVERE = d.PRDEREVERE,
        PRCRREVERE = d.PRCRREVERE,
        PUCDEGLOSA = d.PUCDEGLOSA,
        PUCCRGLOSA = d.PUCCRGLOSA,
        PUCDEGLOVA = d.PUCDEGLOVA,
        PUCCRGLOVA = d.PUCCRGLOVA,
        PCRGLVA360 = d.PCRGLVA360,
        PUCBANCO = d.PUCBANCO,
        CTCONTCANFA = d.CTCONTCANFA,
        PUCDEBCOUR = d.PUCDEBCOUR,
        PUCDEBCOHO = d.PUCDEBCOHO,
        CTCONTFOPA = d.CTCONTFOPA,
        FAFEMPCODI = d.FAFEMPCODI,
        CTCONTEMSU = d.CTCONTEMSU,
        CTCONTSOAT = d.CTCONTSOAT,
        CTCATACODI = d.CTCATACODI,
    };

    private static void ApplyUpdate(Ctcontrato e, UpdateCtcontratoDto d)
    {
        e.CTCONTNUME = d.CTCONTNUME; e.CTADMICODI = d.CTADMICODI;
        e.CTREGICODI = d.CTREGICODI; e.CTPLBECODI = d.CTPLBECODI;
        e.CTMANUCODI = d.CTMANUCODI; e.CTCONTLEGA = d.CTCONTLEGA;
        e.CTCONTFELE = d.CTCONTFELE; e.CTCONTDESD = d.CTCONTDESD;
        e.CTCONTHAST = d.CTCONTHAST; e.CTCONTVALO = d.CTCONTVALO;
        e.CTCONTESTA = d.CTCONTESTA; e.CTTICOCODI = d.CTTICOCODI;
        e.CTCONTASIG = d.CTCONTASIG; e.CTCONTFACT = d.CTCONTFACT;
        e.FAOBSHORA = d.FAOBSHORA; e.CTCTAACRE = d.CTCTAACRE;
        e.CTCITXDIA = d.CTCITXDIA; e.CTCONTCONS = d.CTCONTCONS;
        e.CTCONTVADE = d.CTCONTVADE; e.CTCONTPYP = d.CTCONTPYP;
        e.CTMANUPROD = d.CTMANUPROD; e.PUCDEBRADI = d.PUCDEBRADI;
        e.FAPRESUDEB = d.FAPRESUDEB; e.FAPRESUCRE = d.FAPRESUCRE;
        e.PUCDEBCOPA = d.PUCDEBCOPA; e.PREDEBCOPA = d.PREDEBCOPA;
        e.PRECRECOPA = d.PRECRECOPA; e.PRCRREVACT = d.PRCRREVACT;
        e.PRDEREVAFA = d.PRDEREVAFA; e.PRCRREVAFA = d.PRCRREVAFA;
        e.PRDEREVARE = d.PRDEREVARE; e.PRCRREVARE = d.PRCRREVARE;
        e.PUCDEVA360 = d.PUCDEVA360; e.PRDEREVEFA = d.PRDEREVEFA;
        e.PRCRREVEFA = d.PRCRREVEFA; e.PRDEREVERE = d.PRDEREVERE;
        e.PRCRREVERE = d.PRCRREVERE; e.PUCDEGLOSA = d.PUCDEGLOSA;
        e.PUCCRGLOSA = d.PUCCRGLOSA; e.PUCDEGLOVA = d.PUCDEGLOVA;
        e.PUCCRGLOVA = d.PUCCRGLOVA; e.PCRGLVA360 = d.PCRGLVA360;
        e.PUCBANCO = d.PUCBANCO; e.CTCONTCANFA = d.CTCONTCANFA;
        e.PUCDEBCOUR = d.PUCDEBCOUR; e.PUCDEBCOHO = d.PUCDEBCOHO;
        e.CTCONTFOPA = d.CTCONTFOPA; e.FAFEMPCODI = d.FAFEMPCODI;
        e.CTCONTEMSU = d.CTCONTEMSU; e.CTCONTSOAT = d.CTCONTSOAT;
        e.CTCATACODI = d.CTCATACODI;
        // Nota: CTCONTCODI NO se actualiza — es la clave principal
    }
}