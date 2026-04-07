using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Citas;

public interface IAdcitasService
{
    Task<PagedResult<AdcitasDto>> GetAllAsync(AdcitasQueryDto query);
    Task<AdcitasDto?> GetByCodeAsync(string adcitacons);
    Task<IEnumerable<AdcitasDetalleDto>> GetByPacienteAsync(string adpaciiden);
    Task<AdcitasDto> CreateAsync(CreateAdcitasDto dto);
    Task<AdcitasDto> UpdateAsync(string adcitacons, UpdateAdcitasDto dto);
    Task<bool> DeleteAsync(string adcitacons);
    Task<AdcitasConfirmacionDto> CreateFromDispmedAsync(CreateAdcitasFromDispmedDto dto);
}