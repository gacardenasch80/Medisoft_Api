using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Citas;

public interface IAdparametrosService
{
    Task<PagedResult<AdparametrosDto>> GetAllAsync(AdparametrosQueryDto query);
    Task<AdparametrosDto?> GetByCodeAsync(string adparametro);
    Task<AdparametrosDto> CreateAsync(CreateAdparametrosDto dto);
    Task<AdparametrosDto> UpdateAsync(string adparametro, UpdateAdparametrosDto dto);
    Task<bool> DeleteAsync(string adparametro);
}