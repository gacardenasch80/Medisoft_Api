using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface ICtcontratoService
{
    Task<PagedResult<CtcontratoDto>> GetAllAsync(CtcontratoQueryDto query);
    Task<CtcontratoDto?> GetByCodeAsync(string ctcontcodi);
    Task<CtcontratoDto> CreateAsync(CreateCtcontratoDto dto);
    Task<CtcontratoDto> UpdateAsync(string ctcontcodi, UpdateCtcontratoDto dto);
    Task<bool> DeleteAsync(string ctcontcodi);
}