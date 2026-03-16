using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;

namespace MedisoftAPI.Application.UseCases;

public class AdadmipaciService : IAdadmipaciService
{
    private readonly IAdadmipaciRepository _repo;
    private readonly ICtcontratoRepository _repoContrato;
    private readonly ICtadministRepository _repoAdminist;
    private readonly IAdpacienteRepository _repoPaciente;

    public AdadmipaciService(
        IAdadmipaciRepository repo,
        ICtcontratoRepository repoContrato,
        ICtadministRepository repoAdminist,
        IAdpacienteRepository repoPaciente)
    {
        _repo = repo;
        _repoContrato = repoContrato;
        _repoAdminist = repoAdminist;
        _repoPaciente = repoPaciente;
    }

    public async Task<PagedResult<AdadmipaciDto>> GetAllAsync(AdadmipaciQueryDto query)
    {
        var filter = new AdadmipaciFilter
        {
            ADADPACONS = query.ADADPACONS,
            CTADMICODI = query.CTADMICODI,
            ADPACIIDEN = query.ADPACIIDEN,
            CTCONTCODI = query.CTCONTCODI,
            ADADPAESTA = query.ADADPAESTA,
            CTNIVECODI = query.CTNIVECODI,
            FechaInicio = query.FechaInicio,
            FechaFin = query.FechaFin,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                          query.TamPagina > 200 ? 200 : query.TamPagina
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<AdadmipaciDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total
        };
    }

    public async Task<AdadmipaciDto?> GetByCodeAsync(string adadpacons)
    {
        var e = await _repo.GetByCodeAsync(adadpacons);
        return e is null ? null : ToDto(e);
    }

    public async Task<AdadmipaciDto> CreateAsync(CreateAdadmipaciDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    public async Task<AdadmipaciDto> UpdateAsync(string adadpacons, UpdateAdadmipaciDto dto)
    {
        var existing = await _repo.GetByCodeAsync(adadpacons)
            ?? throw new KeyNotFoundException(
                $"Admisión de paciente '{adadpacons}' no encontrada.");

        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string adadpacons)
        => _repo.DeleteAsync(adadpacons);

    // ── GET BY PACIENTE (con Contrato y Administradora) ───────────────────

    public async Task<IEnumerable<AdadmipaciDetalleDto>> GetByPacienteAsync(string adpaciiden)
    {
        // 1. Traer todas las admisiones del paciente
        var filter = new AdadmipaciFilter
        {
            ADPACIIDEN = adpaciiden.Trim(),
            Pagina = 1,
            TamPagina = 200
        };

        var (admisiones, _) = await _repo.GetAllAsync(filter);

        // 2. Para cada admisión buscar contrato y administradora en paralelo
        var tareas = admisiones.Select(async admision =>
        {
            var contratoTask = string.IsNullOrWhiteSpace(admision.CTCONTCODI)
                ? Task.FromResult<Ctcontrato?>(null)
                : _repoContrato.GetByCodeAsync(admision.CTCONTCODI);

            var administTask = string.IsNullOrWhiteSpace(admision.CTADMICODI)
                ? Task.FromResult<Ctadminist?>(null)
                : _repoAdminist.GetByCodeAsync(admision.CTADMICODI);

            await Task.WhenAll(contratoTask, administTask);

            return new AdadmipaciDetalleDto
            {
                Admision = ToDto(admision),
                Contrato = contratoTask.Result is null ? null : ToContratoDto(contratoTask.Result),
                Administradora = administTask.Result is null ? null : ToAdministDto(administTask.Result),
            };
        });

        return await Task.WhenAll(tareas);
    }

    public async Task<IEnumerable<AdadmipaciDetalleDto>> GetByPacienteCelularAsync(string celular)
    {
        var paciente = await _repoPaciente.GetByCelularAsync(celular);
        if (paciente is null)
            throw new KeyNotFoundException(
                $"Paciente con celular '{celular}' no encontrado."); 

        string adpaciiden = paciente.ADPACIIDEN;
        // 1. Traer todas las admisiones del paciente
        var filter = new AdadmipaciFilter
        {
            ADPACIIDEN = adpaciiden.Trim(),
            Pagina = 1,
            TamPagina = 200
        };

        var (admisiones, _) = await _repo.GetAllAsync(filter);

        // 2. Para cada admisión buscar contrato y administradora en paralelo
        var tareas = admisiones.Select(async admision =>
        {
            var contratoTask = string.IsNullOrWhiteSpace(admision.CTCONTCODI)
                ? Task.FromResult<Ctcontrato?>(null)
                : _repoContrato.GetByCodeAsync(admision.CTCONTCODI);

            var administTask = string.IsNullOrWhiteSpace(admision.CTADMICODI)
                ? Task.FromResult<Ctadminist?>(null)
                : _repoAdminist.GetByCodeAsync(admision.CTADMICODI);

            await Task.WhenAll(contratoTask, administTask);

            return new AdadmipaciDetalleDto
            {
                Admision = ToDto(admision),
                Contrato = contratoTask.Result is null ? null : ToContratoDto(contratoTask.Result),
                Administradora = administTask.Result is null ? null : ToAdministDto(administTask.Result),
            };
        });

        return await Task.WhenAll(tareas);
    }

    // ── Mappers ───────────────────────────────────────────────────────────

    private static AdadmipaciDto ToDto(Adadmipaci e) => new()
    {
        ADADPACONS = e.ADADPACONS,
        CTADMICODI = e.CTADMICODI,
        ADPACIIDEN = e.ADPACIIDEN,
        ADTIAFCODI = e.ADTIAFCODI,
        ADADPADOCU = e.ADADPADOCU,
        CTCONTCODI = e.CTCONTCODI,
        ADADPAESTA = e.ADADPAESTA,
        CTNIVECODI = e.CTNIVECODI,
        ADADPAFEIN = e.ADADPAFEIN,
        ADADPAFEFI = e.ADADPAFEFI,
    };

    private static Adadmipaci FromCreate(CreateAdadmipaciDto d) => new()
    {
        ADADPACONS = d.ADADPACONS,
        CTADMICODI = d.CTADMICODI,
        ADPACIIDEN = d.ADPACIIDEN,
        ADTIAFCODI = d.ADTIAFCODI,
        ADADPADOCU = d.ADADPADOCU,
        CTCONTCODI = d.CTCONTCODI,
        ADADPAESTA = d.ADADPAESTA,
        CTNIVECODI = d.CTNIVECODI,
        ADADPAFEIN = d.ADADPAFEIN,
        ADADPAFEFI = d.ADADPAFEFI,
    };

    private static void ApplyUpdate(Adadmipaci e, UpdateAdadmipaciDto d)
    {
        e.CTADMICODI = d.CTADMICODI;
        e.ADPACIIDEN = d.ADPACIIDEN;
        e.ADTIAFCODI = d.ADTIAFCODI;
        e.ADADPADOCU = d.ADADPADOCU;
        e.CTCONTCODI = d.CTCONTCODI;
        e.ADADPAESTA = d.ADADPAESTA;
        e.CTNIVECODI = d.CTNIVECODI;
        e.ADADPAFEIN = d.ADADPAFEIN;
        e.ADADPAFEFI = d.ADADPAFEFI;
        // Nota: ADADPACONS NO se actualiza — es la clave principal
    }

    private static CtcontratoDto ToContratoDto(Ctcontrato e) => new()
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

    private static CtadministDto ToAdministDto(Ctadminist e) => new()
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
}