using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface IAddispmedService
{
    Task<PagedResult<AddispmedDto>> GetAllAsync(AddispmedQueryDto query);
    Task<AddispmedDto?> GetByCodeAsync(string addispcons);
    Task<AddispmedDto> CreateAsync(CreateAddispmedDto dto);
    Task<AddispmedDto> UpdateAsync(string addispcons, UpdateAddispmedDto dto);
    Task<bool> DeleteAsync(string addispcons);
}