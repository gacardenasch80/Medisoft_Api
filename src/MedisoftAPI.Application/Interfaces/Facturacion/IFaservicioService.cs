using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Facturacion;

public interface IFaservicioService
{
    Task<PagedResult<FaservicioDto>> GetAllAsync(FaservicioQueryDto query);
    Task<FaservicioDto?>             GetByCodeAsync(string codserv);
    Task<FaservicioDto>              CreateAsync(CreateFaservicioDto dto);
    Task<FaservicioDto>              UpdateAsync(string codserv, UpdateFaservicioDto dto);
    Task<bool>                       DeleteAsync(string codserv);
}
