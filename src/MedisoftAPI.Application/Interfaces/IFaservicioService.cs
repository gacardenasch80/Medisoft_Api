using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface IFaservicioService
{
    Task<PagedResult<FaservicioDto>> GetAllAsync(FaservicioQueryDto query);
    Task<FaservicioDto?>             GetByCodeAsync(string codserv);
    Task<FaservicioDto>              CreateAsync(CreateFaservicioDto dto);
    Task<FaservicioDto>              UpdateAsync(string codserv, UpdateFaservicioDto dto);
    Task<bool>                       DeleteAsync(string codserv);
}
