using MedisoftAPI.Application.DTOs;

namespace MedisoftAPI.Application.Interfaces;

public interface IAdadmipaciService
{
    Task<PagedResult<AdadmipaciDto>> GetAllAsync(AdadmipaciQueryDto query);
    Task<AdadmipaciDto?> GetByCodeAsync(string adadpacons);
    Task<IEnumerable<AdadmipaciDetalleDto>> GetByPacienteAsync(string adpaciiden);
    Task<AdadmipaciDto> CreateAsync(CreateAdadmipaciDto dto);
    Task<AdadmipaciDto> UpdateAsync(string adadpacons, UpdateAdadmipaciDto dto);
    Task<bool> DeleteAsync(string adadpacons);
}