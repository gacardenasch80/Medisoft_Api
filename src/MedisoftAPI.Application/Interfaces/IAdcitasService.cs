using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface IAdcitasService
{
    Task<PagedResult<AdcitasDto>> GetAllAsync(AdcitasQueryDto query);
    Task<AdcitasDto?> GetByCodeAsync(string adadpacons);
    Task<IEnumerable<AdcitasDetalleDto>> GetByPacienteAsync(string adpaciiden);
    Task<AdcitasDto> CreateAsync(CreateAdcitasDto dto);
    Task<AdcitasDto> UpdateAsync(string adadpacons, UpdateAdcitasDto dto);
    Task<bool> DeleteAsync(string adadpacons);
    Task<AdcitasDto> CreateFromDispmedAsync(CreateAdcitasFromDispmedDto dto);
}