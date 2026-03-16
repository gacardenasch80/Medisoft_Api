using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface IFaprogpypService
{
    Task<PagedResult<FaprogpypDto>> GetAllAsync(FaprogpypQueryDto query);
    Task<FaprogpypDto?> GetByCodeAsync(string faprogcodi);
    Task<IEnumerable<FaprogpypDto>> GetByPacienteAsync(string adpaciiden);
    Task<FaprogpypDto> CreateAsync(CreateFaprogpypDto dto);
    Task<FaprogpypDto> UpdateAsync(string faprogcodi, UpdateFaprogpypDto dto);
    Task<bool> DeleteAsync(string faprogcodi);
}