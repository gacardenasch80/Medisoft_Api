using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Citas;

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