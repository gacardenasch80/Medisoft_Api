using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface ICtadministService
{
    Task<PagedResult<CtadministDto>> GetAllAsync(CtadministQueryDto query);
    Task<CtadministDto?> GetByCodeAsync(string ctadmicodi);
    Task<CtadministDto> CreateAsync(CreateCtadministDto dto);
    Task<CtadministDto> UpdateAsync(string ctadmicodi, UpdateCtadministDto dto);
    Task<bool> DeleteAsync(string ctadmicodi);
}