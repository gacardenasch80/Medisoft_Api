using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface IGeespecialService
{
    Task<PagedResult<GeespecialDto>> GetAllAsync(GeespecialQueryDto query);
    Task<GeespecialDto?> GetByCodeAsync(string geespecodi);
    Task<GeespecialDto> CreateAsync(CreateGeespecialDto dto);
    Task<GeespecialDto> UpdateAsync(string geespecodi, UpdateGeespecialDto dto);
    Task<bool> DeleteAsync(string geespecodi);
}