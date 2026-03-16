using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;

namespace MedisoftAPI.Application.UseCases;

public class AdpacienteService : IAdpacienteService
{
    private readonly IAdpacienteRepository _repo;

    public AdpacienteService(IAdpacienteRepository repo) => _repo = repo;

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<PagedResult<AdpacienteDto>> GetAllAsync(AdpacienteQueryDto query)
    {
        var filter = new AdpacienteFilter
        {
            ADPACIIDEN = query.ADPACIIDEN,
            ADTIIDCODI = query.ADTIIDCODI,
            ADPACIAPE1 = query.ADPACIAPE1,
            ADPACIAPE2 = query.ADPACIAPE2,
            ADPACINOM1 = query.ADPACINOM1,
            ADPACINOM2 = query.ADPACINOM2,
            ADPACICELU = query.ADPACICELU,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<AdpacienteDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total
        };
    }

    // ── GET BY ID ─────────────────────────────────────────────────────────

    public async Task<AdpacienteDto?> GetByCodeAsync(string identificacion)
    {
        var e = await _repo.GetByCodeAsync(identificacion);
        return e is null ? null : ToDto(e);
    }

    public async Task<AdpacienteDto?> GetByCelularAsync(string celular)
    {
        var e = await _repo.GetByCelularAsync(celular);
        return e is null ? null : ToDto(e);
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<AdpacienteDto> CreateAsync(CreateAdpacienteDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<AdpacienteDto> UpdateAsync(string identificacion, UpdateAdpacienteDto dto)
    {
        var existing = await _repo.GetByCodeAsync(identificacion)
            ?? throw new KeyNotFoundException(
                $"Paciente con identificación '{identificacion}' no encontrado.");

        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public Task<bool> DeleteAsync(string identificacion)
        => _repo.DeleteAsync(identificacion);

    // ── Mappers ───────────────────────────────────────────────────────────

    private static AdpacienteDto ToDto(Adpaciente e) => new()
    {
        // Identificación
        ADTIIDCODI = e.ADTIIDCODI,
        ADPACIIDEN = e.ADPACIIDEN,
        ADTIIDCAFA = e.ADTIIDCAFA,
        ADCAFAIDEN = e.ADCAFAIDEN,
        // Datos personales
        ADPACIAPE1 = e.ADPACIAPE1,
        ADPACIAPE2 = e.ADPACIAPE2,
        ADPACINOM1 = e.ADPACINOM1,
        ADPACINOM2 = e.ADPACINOM2,
        ADPACIFENA = e.ADPACIFENA,
        ADPACISEXO = e.ADPACISEXO,
        // Ubicación
        GEBARRZONA = e.GEBARRZONA,
        GEDEPACODI = e.GEDEPACODI,
        GEMUNICODI = e.GEMUNICODI,
        GEBARRCODI = e.GEBARRCODI,
        GEDEPACOD1 = e.GEDEPACOD1,
        GEMUNICOD1 = e.GEMUNICOD1,
        // Régimen y contacto
        CTREGICODI = e.CTREGICODI,
        ADPACIDIRE = e.ADPACIDIRE,
        ADPACITELE = e.ADPACITELE,
        CTNIVECODI = e.CTNIVECODI,
        // Historia clínica
        ADPACIHIST = e.ADPACIHIST,
        ADFEINSGSS = e.ADFEINSGSS,
        ADPACIFEIN = e.ADPACIFEIN,
        ADTIAFCODI = e.ADTIAFCODI,
        ADESTACODI = e.ADESTACODI,
        // Familia
        ADPACIPADR = e.ADPACIPADR,
        ADPACIMADR = e.ADPACIMADR,
        // Contacto adicional
        ADPACICELU = e.ADPACICELU,
        ADPACIMAIL = e.ADPACIMAIL,
        ADPACIHECL = e.ADPACIHECL,
        // Códigos geográficos
        GECODIESC = e.GECODIESC,
        GECODIPECT = e.GECODIPECT,
        GECODIOCGG = e.GECODIOCGG,
        GECODIOSGP = e.GECODIOSGP,
        GECODIOCSG = e.GECODIOCSG,
        GECODIOCGP = e.GECODIOCGP,
        // Clasificación
        CODIGOPOB = e.CODIGOPOB,
        CODIGONUT = e.CODIGONUT,
        // Observaciones y referencias
        ADPACIOBSE = e.ADPACIOBSE,
        ADPACIIDRE = e.ADPACIIDRE,
        // Facturación
        FAFERECODI = e.FAFERECODI,
        FAFETIFCOD = e.FAFETIFCOD,
        // Adicionales
        ADPACTYPER = e.ADPACTYPER,
        ADPACRERUT = e.ADPACRERUT,
        ADCLDICODI = e.ADCLDICODI
    };

    private static Adpaciente FromCreate(CreateAdpacienteDto d) => new()
    {
        // Identificación
        ADTIIDCODI = d.ADTIIDCODI,
        ADPACIIDEN = d.ADPACIIDEN,
        ADTIIDCAFA = d.ADTIIDCAFA,
        ADCAFAIDEN = d.ADCAFAIDEN,
        // Datos personales
        ADPACIAPE1 = d.ADPACIAPE1,
        ADPACIAPE2 = d.ADPACIAPE2,
        ADPACINOM1 = d.ADPACINOM1,
        ADPACINOM2 = d.ADPACINOM2,
        ADPACIFENA = d.ADPACIFENA,
        ADPACISEXO = d.ADPACISEXO,
        // Ubicación
        GEBARRZONA = d.GEBARRZONA,
        GEDEPACODI = d.GEDEPACODI,
        GEMUNICODI = d.GEMUNICODI,
        GEBARRCODI = d.GEBARRCODI,
        GEDEPACOD1 = d.GEDEPACOD1,
        GEMUNICOD1 = d.GEMUNICOD1,
        // Régimen y contacto
        CTREGICODI = d.CTREGICODI,
        ADPACIDIRE = d.ADPACIDIRE,
        ADPACITELE = d.ADPACITELE,
        CTNIVECODI = d.CTNIVECODI,
        // Historia clínica
        ADPACIHIST = d.ADPACIHIST,
        ADFEINSGSS = d.ADFEINSGSS,
        ADPACIFEIN = d.ADPACIFEIN,
        ADTIAFCODI = d.ADTIAFCODI,
        ADESTACODI = d.ADESTACODI,
        // Familia
        ADPACIPADR = d.ADPACIPADR,
        ADPACIMADR = d.ADPACIMADR,
        // Contacto adicional
        ADPACICELU = d.ADPACICELU,
        ADPACIMAIL = d.ADPACIMAIL,
        ADPACIHECL = d.ADPACIHECL,
        // Códigos geográficos
        GECODIESC = d.GECODIESC,
        GECODIPECT = d.GECODIPECT,
        GECODIOCGG = d.GECODIOCGG,
        GECODIOSGP = d.GECODIOSGP,
        GECODIOCSG = d.GECODIOCSG,
        GECODIOCGP = d.GECODIOCGP,
        // Clasificación
        CODIGOPOB = d.CODIGOPOB,
        CODIGONUT = d.CODIGONUT,
        // Observaciones y referencias
        ADPACIOBSE = d.ADPACIOBSE,
        ADPACIIDRE = d.ADPACIIDRE,
        // Facturación
        FAFERECODI = d.FAFERECODI,
        FAFETIFCOD = d.FAFETIFCOD,
        // Adicionales
        ADPACTYPER = d.ADPACTYPER,
        ADPACRERUT = d.ADPACRERUT,
        ADCLDICODI = d.ADCLDICODI
    };

    private static void ApplyUpdate(Adpaciente e, UpdateAdpacienteDto d)
    {
        // Identificación
        e.ADTIIDCODI = d.ADTIIDCODI; e.ADTIIDCAFA = d.ADTIIDCAFA;
        e.ADCAFAIDEN = d.ADCAFAIDEN;
        // Datos personales
        e.ADPACIAPE1 = d.ADPACIAPE1; e.ADPACIAPE2 = d.ADPACIAPE2;
        e.ADPACINOM1 = d.ADPACINOM1; e.ADPACINOM2 = d.ADPACINOM2;
        e.ADPACIFENA = d.ADPACIFENA; e.ADPACISEXO = d.ADPACISEXO;
        // Ubicación
        e.GEBARRZONA = d.GEBARRZONA; e.GEDEPACODI = d.GEDEPACODI;
        e.GEMUNICODI = d.GEMUNICODI; e.GEBARRCODI = d.GEBARRCODI;
        e.GEDEPACOD1 = d.GEDEPACOD1; e.GEMUNICOD1 = d.GEMUNICOD1;
        // Régimen y contacto
        e.CTREGICODI = d.CTREGICODI; e.ADPACIDIRE = d.ADPACIDIRE;
        e.ADPACITELE = d.ADPACITELE; e.CTNIVECODI = d.CTNIVECODI;
        // Historia clínica
        e.ADPACIHIST = d.ADPACIHIST; e.ADFEINSGSS = d.ADFEINSGSS;
        e.ADPACIFEIN = d.ADPACIFEIN; e.ADTIAFCODI = d.ADTIAFCODI;
        e.ADESTACODI = d.ADESTACODI;
        // Familia
        e.ADPACIPADR = d.ADPACIPADR; e.ADPACIMADR = d.ADPACIMADR;
        // Contacto adicional
        e.ADPACICELU = d.ADPACICELU; e.ADPACIMAIL = d.ADPACIMAIL;
        e.ADPACIHECL = d.ADPACIHECL;
        // Códigos geográficos
        e.GECODIESC = d.GECODIESC; e.GECODIPECT = d.GECODIPECT;
        e.GECODIOCGG = d.GECODIOCGG; e.GECODIOSGP = d.GECODIOSGP;
        e.GECODIOCSG = d.GECODIOCSG; e.GECODIOCGP = d.GECODIOCGP;
        // Clasificación
        e.CODIGOPOB = d.CODIGOPOB; e.CODIGONUT = d.CODIGONUT;
        // Observaciones y referencias
        e.ADPACIOBSE = d.ADPACIOBSE; e.ADPACIIDRE = d.ADPACIIDRE;
        // Facturación
        e.FAFERECODI = d.FAFERECODI; e.FAFETIFCOD = d.FAFETIFCOD;
        // Adicionales
        e.ADPACTYPER = d.ADPACTYPER; e.ADPACRERUT = d.ADPACRERUT;
        e.ADCLDICODI = d.ADCLDICODI;
        // Nota: ADPACIIDEN NO se actualiza — es la clave principal
    }
}