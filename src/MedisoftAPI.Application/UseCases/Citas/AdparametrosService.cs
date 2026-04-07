using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Citas;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Interfaces.Citas;

namespace MedisoftAPI.Application.UseCases.Citas;

public class AdparametrosService : IAdparametrosService
{
    private readonly IAdparametrosRepository _repo;

    public AdparametrosService(IAdparametrosRepository repo) => _repo = repo;

    public async Task<PagedResult<AdparametrosDto>> GetAllAsync(AdparametrosQueryDto query)
    {
        var filter = new AdparametrosFilter
        {
            Adparametro = query.Adparametro,
            Advalorpara = query.Advalorpara,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                          query.TamPagina > 200 ? 200 : query.TamPagina,
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<AdparametrosDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total,
        };
    }

    public async Task<AdparametrosDto?> GetByCodeAsync(string adparametro)
    {
        var e = await _repo.GetByCodeAsync(adparametro);
        return e is null ? null : ToDto(e);
    }

    public async Task<AdparametrosDto> CreateAsync(CreateAdparametrosDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    public async Task<AdparametrosDto> UpdateAsync(string adparametro, UpdateAdparametrosDto dto)
    {
        var existing = await _repo.GetByCodeAsync(adparametro)
            ?? throw new KeyNotFoundException($"Parámetro '{adparametro}' no encontrado.");
        existing.Advalorpara = dto.Advalorpara.Trim();
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string adparametro)
        => _repo.DeleteAsync(adparametro);

    // ── Mappers ───────────────────────────────────────────────────────────

    private static AdparametrosDto ToDto(Adparametros e) => new()
    {
        Adparametro = e.Adparametro,
        Advalorpara = e.Advalorpara,
    };

    private static Adparametros FromCreate(CreateAdparametrosDto d) => new()
    {
        Adparametro = d.Adparametro.Trim(),
        Advalorpara = d.Advalorpara?.Trim() ?? string.Empty,
    };
}