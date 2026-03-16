using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface IAdpacienteService
{
    Task<PagedResult<AdpacienteDto>> GetAllAsync(AdpacienteQueryDto query);
    Task<AdpacienteDto?> GetByCodeAsync(string codserv);
    Task<AdpacienteDto?> GetByCelularAsync(string celular);
    Task<AdpacienteDto> CreateAsync(CreateAdpacienteDto dto);
    Task<AdpacienteDto> UpdateAsync(string codserv, UpdateAdpacienteDto dto);
    Task<bool> DeleteAsync(string codserv);
}
