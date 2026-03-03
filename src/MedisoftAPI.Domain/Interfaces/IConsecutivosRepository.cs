namespace MedisoftAPI.Domain.Interfaces;

/// <summary>
/// Genera y actualiza consecutivos desde la tabla Consecutivos de FoxPro.
/// Equivalente al ConsecutivosService del sistema legado.
/// </summary>
public interface IConsecutivosRepository
{
    /// <summary>
    /// Incrementa el consecutivo de la tabla indicada y devuelve el número anterior
    /// (igual que f_consecutivo del legado). El resultado viene sin padding.
    /// </summary>
    Task<string> GetNextAsync(string tabla);
}