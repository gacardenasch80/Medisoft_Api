using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;

namespace MedisoftAPI.Application.Interfaces;

public interface IFaprogpypService
{
    Task<PagedResult<FaprogpypDto>> GetAllAsync(FaprogpypQueryDto query);
    Task<FaprogpypDto?> GetByCodeAsync(string faprogcodi);
    Task<PagedResult<FaprogpypDto>> GetByPacienteAsync(string adpaciiden, int pagina, int tamPagina);
    Task<FaprogpypDto> CreateAsync(CreateFaprogpypDto dto);
    Task<FaprogpypDto> UpdateAsync(string faprogcodi, UpdateFaprogpypDto dto);
    Task<bool> DeleteAsync(string faprogcodi);
}