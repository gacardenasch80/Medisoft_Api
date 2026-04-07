using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces.Citas;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Interfaces.Citas;

namespace MedisoftAPI.Application.UseCases.Citas;

public class AdzonificaService : IAdzonificaService
{
    private readonly IAdzonificaRepository _repo;

    public AdzonificaService(IAdzonificaRepository repo) => _repo = repo;

    public async Task<PagedResult<AdzonificaDto>> GetAllAsync(AdzonificaQueryDto query)
    {
        var filter = new AdzonificaFilter
        {
            Adpaciiden = query.Adpaciiden,
            Geunprcodi = query.Geunprcodi,
            Estado = query.Estado,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                         query.TamPagina > 200 ? 200 : query.TamPagina,
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<AdzonificaDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total,
        };
    }

    public async Task<IEnumerable<AdzonificaDto>> GetByPacienteAsync(string adpaciiden)
    {
        var items = await _repo.GetByPacienteAsync(adpaciiden.Trim());
        return items.Select(ToDto);
    }

    public async Task<AdzonificaDto?> GetByPacienteYZonaAsync(string adpaciiden, string geunprcodi)
    {
        var e = await _repo.GetByPacienteYZonaAsync(adpaciiden.Trim(), geunprcodi.Trim());
        return e is null ? null : ToDto(e);
    }

    public async Task<AdzonificaDto> CreateAsync(CreateAdzonificaDto dto)
    {
        // Validar que no exista ya la combinación paciente + zona
        var existe = await _repo.GetByPacienteYZonaAsync(
            dto.Adpaciiden.Trim(), dto.Geunprcodi.Trim());

        if (existe is not null)
            throw new InvalidOperationException(
                $"El paciente '{dto.Adpaciiden}' ya está zonificado en la unidad '{dto.Geunprcodi}'.");

        return ToDto(await _repo.CreateAsync(FromCreate(dto)));
    }

    public async Task<AdzonificaDto> UpdateAsync(
        string adpaciiden, string geunprcodi, UpdateAdzonificaDto dto)
    {
        var existing = await _repo.GetByPacienteYZonaAsync(adpaciiden.Trim(), geunprcodi.Trim())
            ?? throw new KeyNotFoundException(
                $"No se encontró zonificación para el paciente '{adpaciiden}' " +
                $"en la unidad '{geunprcodi}'.");

        existing.Estado = dto.Estado;
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string adpaciiden, string geunprcodi)
        => _repo.DeleteAsync(adpaciiden.Trim(), geunprcodi.Trim());

    // ── Mappers ───────────────────────────────────────────────────────────

    private static AdzonificaDto ToDto(Adzonifica e) => new()
    {
        Adpaciiden = e.Adpaciiden,
        Geunprcodi = e.Geunprcodi,
        Estado = e.Estado,
    };

    private static Adzonifica FromCreate(CreateAdzonificaDto d) => new()
    {
        Adpaciiden = d.Adpaciiden.Trim(),
        Geunprcodi = d.Geunprcodi.Trim(),
        Estado = d.Estado ?? 1,
    };
}