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

    public AddispmedService(
        IAddispmedRepository repo,
        IGeespecialRepository repoEspec,
        IGemedicosRepository repoMedic,
        IFaservicioRepository repoServ)
    {
        _repo = repo;
        _repoEspec = repoEspec;
        _repoMedic = repoMedic;
        _repoServ = repoServ;
    }

    // ── GET ALL (paginado + enriquecido) ──────────────────────────────────

    public async Task<PagedResult<AddispmedDetalleDto>> GetAllAsync(AddispmedQueryDto query)
    {
        var filter = new AddispmedFilter
        {
            Addispcons = query.Addispcons,
            Geespecodi = query.Geespecodi,
            Gemedicodi = query.Gemedicodi,
            FechaInicio = query.FechaInicio,
            FechaFin = query.FechaFin,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                          query.TamPagina > 200 ? 200 : query.TamPagina,
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        var detallados = await EnriquecerAsync(items);

        return new PagedResult<AddispmedDetalleDto>
        {
            Items = detallados,
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total,
        };
    }

    // ── GET BY CODE (enriquecido) ─────────────────────────────────────────

    public async Task<AddispmedDetalleDto?> GetByCodeAsync(string addispcons)
    {
        var e = await _repo.GetByCodeAsync(addispcons);
        if (e is null) return null;

        var detalle = await EnriquecerUnoAsync(e);
        return detalle;
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

    /// <summary>
    /// Para una página de registros, lanza las 3 consultas de cada item
    /// en paralelo con Task.WhenAll.
    /// </summary>
    private async Task<IEnumerable<AddispmedDetalleDto>> EnriquecerAsync(
        IEnumerable<Addispmed> items)
    {
        var tareas = items.Select(EnriquecerUnoAsync);
        return await Task.WhenAll(tareas);
    }

    private async Task<AddispmedDetalleDto> EnriquecerUnoAsync(Addispmed e)
    {
        var especTask = string.IsNullOrWhiteSpace(e.Geespecodi)
            ? Task.FromResult<Geespecial?>(null)
            : _repoEspec.GetByCodeAsync(e.Geespecodi);

        var medicTask = string.IsNullOrWhiteSpace(e.Gemedicodi)
            ? Task.FromResult<Gemedicos?>(null)
            : _repoMedic.GetByCodeAsync(e.Gemedicodi);

        var servTask = string.IsNullOrWhiteSpace(e.Faservcodi)
            ? Task.FromResult<Faservicio?>(null)
            : _repoServ.GetByCodeAsync(e.Faservcodi);

        await Task.WhenAll(especTask, medicTask, servTask);

        return new AddispmedDetalleDto
        {
            Addispcons = e.Addispcons?.Trim(),
            Geespecodi = e.Geespecodi?.Trim(),
            Geespenomb = especTask.Result?.Geespenomb?.Trim(),
            Gemedicodi = e.Gemedicodi?.Trim(),
            Gemedinomb = medicTask.Result?.Gemedinomb?.Trim(),
            Faservcodi = e.Faservcodi?.Trim(),
            Faservnomb = servTask.Result?.FASERVNOMB?.Trim(),
            Adconscodi = e.Adconscodi?.Trim(),
            Addispfech = e.Addispfech,
            Adhoraini = e.Adhoraini?.Trim(),
            Adhorafin = e.Adhorafin?.Trim(),
            Addispcita = e.Addispcita,
            Addispplan = e.Addispplan,
        };
    }

    // ── Mappers simples (para Create/Update que devuelven AddispmedDto) ───

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
        // Addispcons NO se actualiza — es la clave principal
    }
}