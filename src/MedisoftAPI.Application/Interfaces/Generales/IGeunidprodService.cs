using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.DTOs.Generales;

namespace MedisoftAPI.Application.Interfaces.Generales;

public interface IGeunidprodService
{
    Task<PagedResult<GeunidprodDto>> GetAllAsync(GeunidprodQueryDto query);
    Task<GeunidprodDto?> GetByCodeAsync(string geunprcodi);
    Task<GeunidprodDto> CreateAsync(CreateGeunidprodDto dto);
    Task<GeunidprodDto> UpdateAsync(string geunprcodi, UpdateGeunidprodDto dto);
    Task<bool> DeleteAsync(string geunprcodi);
}