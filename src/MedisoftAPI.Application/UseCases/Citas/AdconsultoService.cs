using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Citas;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Interfaces.Citas;

namespace MedisoftAPI.Application.UseCases.Citas;

public class AdconsultoService : IAdconsultoService
{
    private readonly IAdconsultoRepository _repo;

    public AdconsultoService(IAdconsultoRepository repo) => _repo = repo;

    public async Task<PagedResult<AdconsultoDto>> GetAllAsync(AdconsultoQueryDto query)
    {
        var filter = new AdconsultoFilter
        {
            Adconscodi = query.Adconscodi,
            Adconsnomb = query.Adconsnomb,
            Adconsdire = query.Adconsdire,
            Adconstele = query.Adconstele,
            Geunprcodi = query.Geunprcodi,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina,
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<AdconsultoDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total,
        };
    }

    public async Task<AdconsultoDto?> GetByCodeAsync(string adconscodi)
    {
        var e = await _repo.GetByCodeAsync(adconscodi);
        return e is null ? null : ToDto(e);
    }

    public async Task<AdconsultoDto> CreateAsync(CreateAdconsultoDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    public async Task<AdconsultoDto> UpdateAsync(string adconscodi, UpdateAdconsultoDto dto)
    {
        var existing = await _repo.GetByCodeAsync(adconscodi)
            ?? throw new KeyNotFoundException($"Consultorio '{adconscodi}' no encontrado.");
        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string adconscodi)
        => _repo.DeleteAsync(adconscodi);

    // ── Mappers ───────────────────────────────────────────────────────────

    private static AdconsultoDto ToDto(Adconsulto e) => new()
    {
        Adconscodi = e.Adconscodi,
        Adconsnomb = e.Adconsnomb,
        Adconsdire = e.Adconsdire,
        Adconstele = e.Adconstele,
        Adconsesur = e.Adconsesur,
        Geunprcodi = e.Geunprcodi,
    };

    private static Adconsulto FromCreate(CreateAdconsultoDto d) => new()
    {
        Adconscodi = d.Adconscodi.Trim(),
        Adconsnomb = d.Adconsnomb ?? string.Empty,
        Adconsdire = d.Adconsdire ?? string.Empty,
        Adconstele = d.Adconstele ?? string.Empty,
        Adconsesur = d.Adconsesur,
        Geunprcodi = d.Geunprcodi ?? string.Empty,
    };

    private static void ApplyUpdate(Adconsulto e, UpdateAdconsultoDto d)
    {
        if (d.Adconsnomb is not null) e.Adconsnomb = d.Adconsnomb;
        if (d.Adconsdire is not null) e.Adconsdire = d.Adconsdire;
        if (d.Adconstele is not null) e.Adconstele = d.Adconstele;
        if (d.Adconsesur is not null) e.Adconsesur = d.Adconsesur;
        if (d.Geunprcodi is not null) e.Geunprcodi = d.Geunprcodi;
        // Adconscodi NO se actualiza — es la clave principal
    }
}