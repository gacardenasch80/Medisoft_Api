using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Generales;

public interface IGeespecialService
{
    Task<PagedResult<GeespecialDto>> GetAllAsync(GeespecialQueryDto query);
    Task<GeespecialDto?> GetByCodeAsync(string geespecodi);
    Task<GeespecialDto> CreateAsync(CreateGeespecialDto dto);
    Task<GeespecialDto> UpdateAsync(string geespecodi, UpdateGeespecialDto dto);
    Task<bool> DeleteAsync(string geespecodi);
}