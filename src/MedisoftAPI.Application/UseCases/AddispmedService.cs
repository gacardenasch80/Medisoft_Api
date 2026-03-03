using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;

namespace MedisoftAPI.Application.UseCases;

public class AddispmedService : IAddispmedService
{
    private readonly IAddispmedRepository _repo;

    public AddispmedService(IAddispmedRepository repo) => _repo = repo;

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<PagedResult<AddispmedDto>> GetAllAsync(AddispmedQueryDto query)
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
                          query.TamPagina > 200 ? 200 : query.TamPagina
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<AddispmedDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total
        };
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<AddispmedDto?> GetByCodeAsync(string addispcons)
    {
        var e = await _repo.GetByCodeAsync(addispcons);
        return e is null ? null : ToDto(e);
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<AddispmedDto> CreateAsync(CreateAddispmedDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<AddispmedDto> UpdateAsync(string addispcons, UpdateAddispmedDto dto)
    {
        var existing = await _repo.GetByCodeAsync(addispcons)
            ?? throw new KeyNotFoundException(
                $"Disponibilidad '{addispcons}' no encontrada.");

        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public Task<bool> DeleteAsync(string addispcons)
        => _repo.DeleteAsync(addispcons);

    // ── Mappers ───────────────────────────────────────────────────────────

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
        Geespecodi = d.Geespecodi,
        Gemedicodi = d.Gemedicodi,
        Faservcodi = d.Faservcodi,
        Adconscodi = d.Adconscodi,
        Addispfech = d.Addispfech,
        Adhoraini = d.Adhoraini,
        Adhorafin = d.Adhorafin,
        Addispcita = d.Addispcita,
        Addispplan = d.Addispplan,
    };

    private static void ApplyUpdate(Addispmed e, UpdateAddispmedDto d)
    {
        e.Geespecodi = d.Geespecodi;
        e.Gemedicodi = d.Gemedicodi;
        e.Faservcodi = d.Faservcodi;
        e.Adconscodi = d.Adconscodi;
        e.Addispfech = d.Addispfech;
        e.Adhoraini = d.Adhoraini;
        e.Adhorafin = d.Adhorafin;
        e.Addispcita = d.Addispcita;
        e.Addispplan = d.Addispplan;
        // Nota: Addispcons NO se actualiza — es la clave principal
    }
}