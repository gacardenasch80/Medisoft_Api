using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Citas;

public interface IAdconsultoService
{
    Task<PagedResult<AdconsultoDto>> GetAllAsync(AdconsultoQueryDto query);
    Task<AdconsultoDto?> GetByCodeAsync(string adadpacons);
    Task<AdconsultoDto> CreateAsync(CreateAdconsultoDto dto);
    Task<AdconsultoDto> UpdateAsync(string adadpacons, UpdateAdconsultoDto dto);
    Task<bool> DeleteAsync(string adadpacons);
}