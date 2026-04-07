using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.DTOs.Generales;

namespace MedisoftAPI.Application.Interfaces.Generales;

public interface IGehospitalService
{
    Task<PagedResult<GehospitalDto>> GetAllAsync(GehospitalQueryDto query);
    Task<GehospitalDto?> GetByCodeAsync(string gehospcodi);
    Task<GehospitalDto> CreateAsync(CreateGehospitalDto dto);
    Task<GehospitalDto> UpdateAsync(string gehospcodi, UpdateGehospitalDto dto);
    Task<bool> DeleteAsync(string gehospcodi);
}