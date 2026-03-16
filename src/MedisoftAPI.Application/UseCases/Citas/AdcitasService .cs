using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Contratacion;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Citas;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Entities.Contratacion;
using MedisoftAPI.Domain.Interfaces.Admision;
using MedisoftAPI.Domain.Interfaces.Citas;
using MedisoftAPI.Domain.Interfaces.Contratacion;

namespace MedisoftAPI.Application.UseCases.Citas;

public class AdcitasService : IAdcitasService
{
    private readonly IAdcitasRepository _repo;
    private readonly ICtcontratoRepository _contratos;
    private readonly ICtadministRepository _admins;
    private readonly IConsecutivosRepository _consecutivos; 
    private readonly IAddispmedRepository _addispmed;   
    private readonly IAdadmipaciRepository _adadmipaci;

    public AdcitasService(
        IAdcitasRepository repo,
        ICtcontratoRepository contratos,
        ICtadministRepository admins,
        IConsecutivosRepository consecutivos,
        IAddispmedRepository addispmed,
        IAdadmipaciRepository adadmipaci)
    {
        _repo = repo;
        _contratos = contratos;
        _admins = admins;
        _consecutivos = consecutivos;
        _addispmed = addispmed;
        _adadmipaci = adadmipaci;    
    }

    public async Task<PagedResult<AdcitasDto>> GetAllAsync(AdcitasQueryDto query)
    {
        var filter = new AdcitasFilter
        {
            Adcitacons = query.Adcitacons,
            Geespecodi = query.Geespecodi,
            Gemedicodi = query.Gemedicodi,
            Faservcodi = query.Faservcodi,
            Adpaciiden = query.Adpaciiden,
            Adconscodi = query.Adconscodi,
            Adfechcita = query.Adfechcita,
            Ctadmicodi = query.Ctadmicodi,
            Ctcontcodi = query.Ctcontcodi,
            Adingrcons = query.Adingrcons,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina,
        };
        var (items, total) = await _repo.GetAllAsync(filter);
        return new PagedResult<AdcitasDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total,
        };
    }

    public async Task<AdcitasDto?> GetByCodeAsync(string adcitacons)
    {
        var e = await _repo.GetByCodeAsync(adcitacons);
        return e is null ? null : ToDto(e);
    }

    public async Task<IEnumerable<AdcitasDetalleDto>> GetByPacienteAsync(string adpaciiden)
    {
        var citas = await _repo.GetByPacienteAsync(adpaciiden);
        var tareas = citas.Select(async cita =>
        {
            var tContrato = string.IsNullOrWhiteSpace(cita.Ctcontcodi)
                ? Task.FromResult<Ctcontrato?>(null)
                : _contratos.GetByCodeAsync(cita.Ctcontcodi.Trim());
            var tAdmin = string.IsNullOrWhiteSpace(cita.Ctadmicodi)
                ? Task.FromResult<Ctadminist?>(null)
                : _admins.GetByCodeAsync(cita.Ctadmicodi.Trim());
            await Task.WhenAll(tContrato, tAdmin);
            return new AdcitasDetalleDto
            {
                Admision = ToDto(cita),
                Contrato = tContrato.Result is null ? null : ToContratoDto(tContrato.Result),
                Administradora = tAdmin.Result is null ? null : ToAdministDto(tAdmin.Result),
            };
        });
        return await Task.WhenAll(tareas);
    }

    public async Task<AdcitasDto> CreateAsync(CreateAdcitasDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    public async Task<AdcitasDto> UpdateAsync(string adcitacons, UpdateAdcitasDto dto)
    {
        var existing = await _repo.GetByCodeAsync(adcitacons)
            ?? throw new KeyNotFoundException($"Cita '{adcitacons}' no encontrada.");
        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string adcitacons) => _repo.DeleteAsync(adcitacons);

    public async Task<AdcitasDto> CreateFromDispmedAsync(CreateAdcitasFromDispmedDto dto)
    {
        // ── 1. Buscar disponibilidad ───────────────────────────────────────
        var dispmed = await _addispmed.GetByCodeAsync(dto.Addispcons)
            ?? throw new KeyNotFoundException(
                $"Disponibilidad '{dto.Addispcons}' no encontrada.");

        // ── 2. Buscar admisión del paciente ───────────────────────────────
        var admipaci = await _adadmipaci.GetByCodeAsync(dto.Adadpacons)
            ?? throw new KeyNotFoundException(
                $"Admisión '{dto.Adadpacons}' no encontrada.");

        // ── 3. Generar consecutivo (padLeft 8 igual que el legado) ────────
        string rawConsec = await _consecutivos.GetNextAsync(dto.Tabla);
        string adcitacons = rawConsec.PadLeft(8, '0');

        // ── 4. Armar la entidad — misma lógica que el legado ──────────────
        var cita = new Adcitas
        {
            Adcitacons = adcitacons,
            Geespecodi = (dispmed.Geespecodi ?? string.Empty).Trim(),
            Gemedicodi = (dispmed.Gemedicodi ?? string.Empty).Trim(),
            Faservcodi = (dispmed.Faservcodi ?? string.Empty).Trim(),
            Adpaciiden = dto.Adpaciiden.Trim(),
            Adconscodi = (dispmed.Adconscodi ?? string.Empty).Trim(),
            Adfechcita = dispmed.Addispfech
                ?? throw new InvalidOperationException(
                    "El campo Addispfech de la disponibilidad está vacío."),
            Adhorainic = (dispmed.Adhoraini ?? string.Empty).Trim(),
            Adhorafina = (dispmed.Adhorafin ?? string.Empty).Trim(),
            Adduraminu = 30,
            Adconsdisp = (dispmed.Addispcons ?? string.Empty).Trim(),
            Adcitaest = "A",
            Adanulcodi = "0",
            Ctadmicodi = (admipaci.CTADMICODI ?? string.Empty).Trim(),
            Ctcontcodi = (admipaci.CTCONTCODI ?? string.Empty).Trim(),
            Fechasoli = dispmed.Addispfech.Value,
            Geusuacreo = "ADM1",
            Adingrcons = string.Empty,
            Faorsecons = string.Empty,
            Coconscons = string.Empty,
            Faprogcodi = dto.Faprogcodi.Trim(),
            Fechalleg = null,           // {//} en el legado = null
            Adcodanula = string.Empty,
            Fechprefpa = dispmed.Addispfech.Value,
            Adcitaande = string.Empty,
            Geusuacoan = string.Empty,
            Adcitafean = null,           // {//} en el legado = null
            Adciticodi = "0",
        };

        // ── 5. Insertar cita ──────────────────────────────────────────────
        var creada = await _repo.CreateAsync(cita);

        // ── 6. Marcar disponibilidad como citada (UpdateAddispmed del legado)
        dispmed.Addispcita = true;
        await _addispmed.UpdateAsync(dispmed);

        return ToDto(creada);
    }

    private static AdcitasDto ToDto(Adcitas e) => new()
    {
        Adcitacons = e.Adcitacons,
        Geespecodi = e.Geespecodi,
        Gemedicodi = e.Gemedicodi,
        Faservcodi = e.Faservcodi,
        Adpaciiden = e.Adpaciiden,
        Adconscodi = e.Adconscodi,
        Adfechcita = e.Adfechcita,
        Adhorainic = e.Adhorainic,
        Adhorafina = e.Adhorafina,
        Adduraminu = e.Adduraminu,
        Adconsdisp = e.Adconsdisp,
        Adcitaest = e.Adcitaest,
        Adanulcodi = e.Adanulcodi,
        Ctadmicodi = e.Ctadmicodi,
        Ctcontcodi = e.Ctcontcodi,
        Fechasoli = e.Fechasoli,
        Geusuacreo = e.Geusuacreo,
        Adingrcons = e.Adingrcons,
        Faorsecons = e.Faorsecons,
        Coconscons = e.Coconscons,
        Faprogcodi = e.Faprogcodi,
        Fechalleg = e.Fechalleg,
        Adcodanula = e.Adcodanula,
        Fechprefpa = e.Fechprefpa,
        Adcitaande = e.Adcitaande,
        Geusuacoan = e.Geusuacoan,
        Adcitafean = e.Adcitafean,
        Adciticodi = e.Adciticodi,
    };

    private static Adcitas FromCreate(CreateAdcitasDto d) => new()
    {
        Adcitacons = d.ADADPACONS,
        Geespecodi = d.Geespecodi ?? string.Empty,
        Gemedicodi = d.Gemedicodi ?? string.Empty,
        Faservcodi = d.Faservcodi ?? string.Empty,
        Adpaciiden = d.Adpaciiden ?? string.Empty,
        Adconscodi = d.Adconscodi ?? string.Empty,
        Adfechcita = d.Adfechcita,
        Adhorainic = d.Adhorainic ?? string.Empty,
        Adhorafina = d.Adhorafina ?? string.Empty,
        Adduraminu = d.Adduraminu,
        Adconsdisp = d.Adconsdisp ?? string.Empty,
        Adcitaest = d.Adcitaest ?? string.Empty,
        Adanulcodi = d.Adanulcodi ?? string.Empty,
        Ctadmicodi = d.Ctadmicodi ?? string.Empty,
        Ctcontcodi = d.Ctcontcodi ?? string.Empty,
        Fechasoli = d.Fechasoli,
        Geusuacreo = d.Geusuacreo ?? string.Empty,
        Adingrcons = d.Adingrcons ?? string.Empty,
        Faorsecons = d.Faorsecons ?? string.Empty,
        Coconscons = d.Coconscons ?? string.Empty,
        Faprogcodi = d.Faprogcodi ?? string.Empty,
        Fechalleg = d.Fechalleg,
        Adcodanula = d.Adcodanula ?? string.Empty,
        Fechprefpa = d.Fechprefpa,
        Adcitaande = d.Adcitaande ?? string.Empty,
        Geusuacoan = d.Geusuacoan ?? string.Empty,
        Adcitafean = d.Adcitafean,
        Adciticodi = d.Adciticodi ?? string.Empty,
    };

    private static void ApplyUpdate(Adcitas e, UpdateAdcitasDto d)
    {
        if (d.Geespecodi is not null) e.Geespecodi = d.Geespecodi;
        if (d.Gemedicodi is not null) e.Gemedicodi = d.Gemedicodi;
        if (d.Faservcodi is not null) e.Faservcodi = d.Faservcodi;
        if (d.Adpaciiden is not null) e.Adpaciiden = d.Adpaciiden;
        if (d.Adconscodi is not null) e.Adconscodi = d.Adconscodi;
        e.Adfechcita = d.Adfechcita;
        if (d.Adhorainic is not null) e.Adhorainic = d.Adhorainic;
        if (d.Adhorafina is not null) e.Adhorafina = d.Adhorafina;
        if (d.Adduraminu is not null) e.Adduraminu = d.Adduraminu;
        if (d.Adconsdisp is not null) e.Adconsdisp = d.Adconsdisp;
        if (d.Adcitaest is not null) e.Adcitaest = d.Adcitaest;
        if (d.Adanulcodi is not null) e.Adanulcodi = d.Adanulcodi;
        if (d.Ctadmicodi is not null) e.Ctadmicodi = d.Ctadmicodi;
        if (d.Ctcontcodi is not null) e.Ctcontcodi = d.Ctcontcodi;
        e.Fechasoli = d.Fechasoli;
        if (d.Geusuacreo is not null) e.Geusuacreo = d.Geusuacreo;
        if (d.Adingrcons is not null) e.Adingrcons = d.Adingrcons;
        if (d.Faorsecons is not null) e.Faorsecons = d.Faorsecons;
        if (d.Coconscons is not null) e.Coconscons = d.Coconscons;
        if (d.Faprogcodi is not null) e.Faprogcodi = d.Faprogcodi;
        if (d.Fechalleg is not null) e.Fechalleg = d.Fechalleg;
        if (d.Adcodanula is not null) e.Adcodanula = d.Adcodanula;
        e.Fechprefpa = d.Fechprefpa;
        if (d.Adcitaande is not null) e.Adcitaande = d.Adcitaande;
        if (d.Geusuacoan is not null) e.Geusuacoan = d.Geusuacoan;
        if (d.Adcitafean is not null) e.Adcitafean = d.Adcitafean;
        if (d.Adciticodi is not null) e.Adciticodi = d.Adciticodi;
    }

    private static CtcontratoDto ToContratoDto(Ctcontrato c) => new()
    {
        CTCONTCODI = c.CTCONTCODI,
        CTCONTNUME = c.CTCONTNUME,
        CTADMICODI = c.CTADMICODI,
        CTREGICODI = c.CTREGICODI,
        CTPLBECODI = c.CTPLBECODI,
        CTMANUCODI = c.CTMANUCODI,
        CTCONTLEGA = c.CTCONTLEGA,
        CTCONTFELE = c.CTCONTFELE,
        CTCONTDESD = c.CTCONTDESD,
        CTCONTHAST = c.CTCONTHAST,
        CTCONTVALO = c.CTCONTVALO,
        CTCONTESTA = c.CTCONTESTA,
        CTTICOCODI = c.CTTICOCODI,
    };

    private static CtadministDto ToAdministDto(Ctadminist a) => new()
    {
        CTADMICODI = a.CTADMICODI,
        CTADMINOMB = a.CTADMINOMB,
        CTADMINIT = a.CTADMINIT,
        CTADMISGSS = a.CTADMISGSS,
        CTADMIDIRE = a.CTADMIDIRE,
        CTADMITELE = a.CTADMITELE,
        CTADMIEMAI = a.CTADMIEMAI,
        GEDEPACODI = a.GEDEPACODI,
        GEMUNICODI = a.GEMUNICODI,
        CTADMIESTA = a.CTADMIESTA,
    };
}