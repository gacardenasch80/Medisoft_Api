using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.DTOs.Generales;

namespace MedisoftAPI.Application.Interfaces.Generales;

public interface IGemedicosService
{
    Task<PagedResult<GemedicosDto>> GetAllAsync(GemedicosQueryDto query);
    Task<GemedicosDto?> GetByCodeAsync(string codserv);
    Task<GemedicosDto> CreateAsync(CreateGemedicosDto dto);
    Task<GemedicosDto> UpdateAsync(string codserv, UpdateGemedicosDto dto);
    Task<bool> DeleteAsync(string codserv);
}
