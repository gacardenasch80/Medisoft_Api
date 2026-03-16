using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Citas;

public interface IAddispmedService
{
    Task<PagedResult<AddispmedDetalleDto>> GetAllAsync(AddispmedQueryDto query);
    Task<AddispmedDetalleDto?> GetByCodeAsync(string addispcons);
    Task<AddispmedDto> CreateAsync(CreateAddispmedDto dto);
    Task<AddispmedDto> UpdateAsync(string addispcons, UpdateAddispmedDto dto);
    Task<bool> DeleteAsync(string addispcons);
}