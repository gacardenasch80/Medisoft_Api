using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Citas;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Entities.Facturacion;
using MedisoftAPI.Domain.Entities.Generales;
using MedisoftAPI.Domain.Interfaces.Citas;
using MedisoftAPI.Domain.Interfaces.Facturacion;
using MedisoftAPI.Domain.Interfaces.Generales;

namespace MedisoftAPI.Application.UseCases.Citas;

public class AddispmedService : IAddispmedService
{
    private readonly IAddispmedRepository _repo;
    private readonly IGeespecialRepository _repoEspec;
    private readonly IGemedicosRepository _repoMedic;
    private readonly IFaservicioRepository _repoServ;
    private readonly IAdconsultoRepository _repoConsulto;
    private readonly IAdparametrosRepository _repoParam;
    private readonly IAdzonificaRepository _repoZonifica;

    public AddispmedService(
        IAddispmedRepository repo,
        IGeespecialRepository repoEspec,
        IGemedicosRepository repoMedic,
        IFaservicioRepository repoServ,
        IAdconsultoRepository repoConsulto,
        IAdparametrosRepository repoParam,
        IAdzonificaRepository repoZonifica)
    {
        _repo = repo;
        _repoEspec = repoEspec;
        _repoMedic = repoMedic;
        _repoServ = repoServ;
        _repoConsulto = repoConsulto;
        _repoParam = repoParam;
        _repoZonifica = repoZonifica;
    }

    // ── GET ALL (paginado + enriquecido + zonificación) ───────────────────

    public async Task<PagedResult<AddispmedDetalleDto>> GetAllAsync(AddispmedQueryDto query)
    {
        // ── 1. Validar fechas ─────────────────────────────────────────────
        DateTime ahora = DateTime.Now;

        if (query.FechaInicio.HasValue && query.FechaInicio.Value < ahora.Date)
            throw new ArgumentException(
                $"FechaInicio ({query.FechaInicio.Value:dd/MM/yyyy}) no puede ser anterior " +
                $"a la fecha actual ({ahora:dd/MM/yyyy}).");

        if (query.FechaFin.HasValue && query.FechaFin.Value < ahora.Date)
            throw new ArgumentException(
                $"FechaFin ({query.FechaFin.Value:dd/MM/yyyy}) no puede ser anterior " +
                $"a la fecha actual ({ahora:dd/MM/yyyy}).");

        if (query.FechaInicio.HasValue && query.FechaFin.HasValue
            && query.FechaFin.Value < query.FechaInicio.Value)
            throw new ArgumentException("FechaFin no puede ser anterior a FechaInicio.");

        // ── 2. Verificar parámetro de zonificación ────────────────────────
        // Si "zonifica" = "2" o no existe → sin filtro por zonas
        // Si existe y != "2" y viene Adpaciiden → filtrar por zonas del paciente
        var consultorosPermitidos = await ResolverConsultoriosFiltradosAsync(query.Adpaciiden);

        // ── 3. Armar filtro base ──────────────────────────────────────────
        var filter = new AddispmedFilter
        {
            Addispcons = query.Addispcons,
            Geespecodi = query.Geespecodi,
            Gemedicodi = query.Gemedicodi,
            Faservcodi = query.Faservcodi,
            Adconscodi = query.Adconscodi,
            FechaInicio = query.FechaInicio,
            FechaFin = query.FechaFin,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                          query.TamPagina > 200 ? 200 : query.TamPagina,
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        // ── 4. Aplicar filtro de consultorios si aplica zonificación ──────
        if (consultorosPermitidos is not null)
        {
            items = items
                .Where(d => !string.IsNullOrWhiteSpace(d.Adconscodi)
                            && consultorosPermitidos.Contains(d.Adconscodi.Trim()))
                .ToList();
            total = items.Count();
        }

        var detallados = await EnriquecerAsync(items);

        return new PagedResult<AddispmedDetalleDto>
        {
            Items = detallados,
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total,
        };
    }

    /// <summary>
    /// Resuelve el conjunto de códigos de consultorio permitidos para el paciente
    /// según la configuración del parámetro "zonifica".
    ///
    /// Retorna:
    ///   null  → sin filtro (mostrar todos los consultorios)
    ///   HashSet → solo mostrar disponibilidades de esos consultorios
    /// </summary>
    private async Task<HashSet<string>?> ResolverConsultoriosFiltradosAsync(string? adpaciiden)
    {
        // Buscar parámetro "zonifica"
        var param = await _repoParam.GetByCodeAsync("zonifica");

        // Si no existe o su valor es "2" → sin filtro
        if (param is null || param.Advalorpara?.Trim() == "2")
            return null;

        // Si no viene identificación del paciente → sin filtro
        if (string.IsNullOrWhiteSpace(adpaciiden))
            return null;

        // Traer zonas activas del paciente
        var zonas = await _repoZonifica.GetByPacienteAsync(adpaciiden.Trim());
        var zonasActivas = zonas
            .Where(z => z.Estado == 1)   // 1 = Activo
            .Select(z => z.Geunprcodi.Trim())
            .Distinct()
            .ToList();

        // Si el paciente no tiene zonas activas → devolver set vacío
        // (no debe ver ninguna disponibilidad)
        if (!zonasActivas.Any())
            return [];

        // Buscar consultorios asociados a esas unidades de producción
        var consultorios = await _repoConsulto.GetByUnidadesAsync(zonasActivas);
        var codigos = consultorios
            .Select(c => c.Adconscodi.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return codigos;
    }

    // ── GET BY CODE (enriquecido) ─────────────────────────────────────────

    public async Task<AddispmedDetalleDto?> GetByCodeAsync(string addispcons)
    {
        var e = await _repo.GetByCodeAsync(addispcons);
        return e is null ? null : await EnriquecerUnoAsync(e);
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<AddispmedDto> CreateAsync(CreateAddispmedDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<AddispmedDto> UpdateAsync(string addispcons, UpdateAddispmedDto dto)
    {
        var existing = await _repo.GetByCodeAsync(addispcons)
            ?? throw new KeyNotFoundException($"Disponibilidad '{addispcons}' no encontrada.");
        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public Task<bool> DeleteAsync(string addispcons)
        => _repo.DeleteAsync(addispcons);

    // ── Enriquecimiento paralelo ──────────────────────────────────────────

    private async Task<IEnumerable<AddispmedDetalleDto>> EnriquecerAsync(
        IEnumerable<Addispmed> items)
    {
        var tareas = items.Select(EnriquecerUnoAsync);
        return await Task.WhenAll(tareas);
    }

    private async Task<AddispmedDetalleDto> EnriquecerUnoAsync(Addispmed e)
    {
        var tEspec = string.IsNullOrWhiteSpace(e.Geespecodi)
            ? Task.FromResult<Geespecial?>(null)
            : _repoEspec.GetByCodeAsync(e.Geespecodi);

        var tMedic = string.IsNullOrWhiteSpace(e.Gemedicodi)
            ? Task.FromResult<Gemedicos?>(null)
            : _repoMedic.GetByCodeAsync(e.Gemedicodi);

        var tServ = string.IsNullOrWhiteSpace(e.Faservcodi)
            ? Task.FromResult<Faservicio?>(null)
            : _repoServ.GetByCodeAsync(e.Faservcodi);

        var tConsulto = string.IsNullOrWhiteSpace(e.Adconscodi)
            ? Task.FromResult<Adconsulto?>(null)
            : _repoConsulto.GetByCodeAsync(e.Adconscodi);

        await Task.WhenAll(tEspec, tMedic, tServ, tConsulto);

        return new AddispmedDetalleDto
        {
            Addispcons = e.Addispcons?.Trim(),
            Geespecodi = e.Geespecodi?.Trim(),
            Geespenomb = tEspec.Result?.Geespenomb?.Trim(),
            Gemedicodi = e.Gemedicodi?.Trim(),
            Gemedinomb = tMedic.Result?.Gemedinomb?.Trim(),
            Faservcodi = e.Faservcodi?.Trim(),
            Faservnomb = tServ.Result?.FASERVNOMB?.Trim(),
            Adconscodi = e.Adconscodi?.Trim(),
            Adconsnomb = tConsulto.Result?.Adconsnomb?.Trim(),
            Addispfech = e.Addispfech,
            Adhoraini = e.Adhoraini?.Trim(),
            Adhorafin = e.Adhorafin?.Trim(),
            Addispcita = e.Addispcita,
            Addispplan = e.Addispplan,
        };
    }

    // ── Mappers simples ───────────────────────────────────────────────────

    private static AddispmedDto ToDto(Addispmed e) => new()
    {
        Addispcons = e.Addispcons,
        Geespecodi = e.Geespecodi,
        Gemedicodi = e.Gemedicodi,
        Faservcodi = e.Faservcodi,
        Adconscodi = e.Adconscodi,
        Addispfech = e.Addispfech,
        Adhoraini = e.Adhoraini,
        Adhorafin = e.Adhorafin,
        Addispcita = e.Addispcita,
        Addispplan = e.Addispplan,
    };

    private static Addispmed FromCreate(CreateAddispmedDto d) => new()
    {
        Geespecodi = d.Geespecodi ?? string.Empty,
        Gemedicodi = d.Gemedicodi ?? string.Empty,
        Faservcodi = d.Faservcodi ?? string.Empty,
        Adconscodi = d.Adconscodi ?? string.Empty,
        Addispfech = d.Addispfech,
        Adhoraini = d.Adhoraini ?? string.Empty,
        Adhorafin = d.Adhorafin ?? string.Empty,
        Addispcita = d.Addispcita,
        Addispplan = d.Addispplan,
    };

    private static void ApplyUpdate(Addispmed e, UpdateAddispmedDto d)
    {
        e.Geespecodi = d.Geespecodi ?? e.Geespecodi;
        e.Gemedicodi = d.Gemedicodi ?? e.Gemedicodi;
        e.Faservcodi = d.Faservcodi ?? e.Faservcodi;
        e.Adconscodi = d.Adconscodi ?? e.Adconscodi;
        e.Addispfech = d.Addispfech ?? e.Addispfech;
        e.Adhoraini = d.Adhoraini ?? e.Adhoraini;
        e.Adhorafin = d.Adhorafin ?? e.Adhorafin;
        e.Addispcita = d.Addispcita ?? e.Addispcita;
        e.Addispplan = d.Addispplan ?? e.Addispplan;
    }
}