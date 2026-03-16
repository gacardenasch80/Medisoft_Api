using MedisoftAPI.Application.DTOs.Admision;
using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Admision;

public interface IAdadmipaciService
{
    Task<PagedResult<AdadmipaciDto>> GetAllAsync(AdadmipaciQueryDto query);
    Task<AdadmipaciDto?> GetByCodeAsync(string adadpacons);
    Task<IEnumerable<AdadmipaciDetalleDto>> GetByPacienteAsync(string adpaciiden);
    Task<IEnumerable<AdadmipaciDetalleDto>> GetByPacienteCelularAsync(string celular);
    Task<AdadmipaciDto> CreateAsync(CreateAdadmipaciDto dto);
    Task<AdadmipaciDto> UpdateAsync(string adadpacons, UpdateAdadmipaciDto dto);
    Task<bool> DeleteAsync(string adadpacons);
}