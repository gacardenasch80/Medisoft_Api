using MedisoftAPI.Application.DTOs.Contratacion;
using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Contratacion;

public interface ICtadministService
{
    Task<PagedResult<CtadministDto>> GetAllAsync(CtadministQueryDto query);
    Task<CtadministDto?> GetByCodeAsync(string ctadmicodi);
    Task<CtadministDto> CreateAsync(CreateCtadministDto dto);
    Task<CtadministDto> UpdateAsync(string ctadmicodi, UpdateCtadministDto dto);
    Task<bool> DeleteAsync(string ctadmicodi);
}