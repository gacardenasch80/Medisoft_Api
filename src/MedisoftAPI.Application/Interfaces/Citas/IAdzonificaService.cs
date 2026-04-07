using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Citas;
using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces.Citas;

public interface IAdzonificaService
{
    Task<PagedResult<AdzonificaDto>> GetAllAsync(AdzonificaQueryDto query);
    Task<IEnumerable<AdzonificaDto>> GetByPacienteAsync(string adpaciiden);
    Task<AdzonificaDto?> GetByPacienteYZonaAsync(string adpaciiden, string geunprcodi);
    Task<AdzonificaDto> CreateAsync(CreateAdzonificaDto dto);
    Task<AdzonificaDto> UpdateAsync(string adpaciiden, string geunprcodi, UpdateAdzonificaDto dto);
    Task<bool> DeleteAsync(string adpaciiden, string geunprcodi);
}