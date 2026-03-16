using System.Data.OleDb;
using MedisoftAPI.Domain.Interfaces.Admision;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Admision;

/// <summary>
/// Implementación de IConsecutivosRepository sobre FoxPro (admision.dbc).
/// Replica exactamente la lógica de f_consecutivo() del sistema legado:
///   - Lee el Numero actual de la tabla Consecutivos para la clave indicada
///   - Incrementa el Numero en +1
///   - Devuelve el valor ANTERIOR (antes del incremento), sin padding
/// El caller es responsable de aplicar PadLeft si lo necesita.
/// </summary>
public class ConsecutivosRepository : IConsecutivosRepository
{
    private readonly string _conn;

    public ConsecutivosRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Adm")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Adm' no está configurada en appsettings.json.");
    }

    public async Task<string> GetNextAsync(string tabla)
    {
        return await Task.Run(() =>
        {
            using var conn = new OleDbConnection(_conn);
            conn.Open();

            // 1. Leer el número actual
            int numeroActual;
            using (var cmdRead = new OleDbCommand(
                "SELECT Numero FROM Consecutivos WHERE ALLTRIM(Tabla) = ?", conn))
            {
                cmdRead.Parameters.Add(new OleDbParameter("?", tabla.Trim()));
                var result = cmdRead.ExecuteScalar()
                    ?? throw new KeyNotFoundException(
                        $"No se encontró el consecutivo para la tabla '{tabla}'.");
                numeroActual = Convert.ToInt32(result);
            }

            // 2. Incrementar en +1 (igual que el legado)
            using (var cmdUpd = new OleDbCommand(
                "UPDATE Consecutivos SET Numero = ? WHERE ALLTRIM(Tabla) = ?", conn))
            {
                cmdUpd.Parameters.Add(new OleDbParameter("?", numeroActual + 1));
                cmdUpd.Parameters.Add(new OleDbParameter("?", tabla.Trim()));
                int rows = cmdUpd.ExecuteNonQuery();
                if (rows == 0)
                    throw new InvalidOperationException(
                        $"No se pudo actualizar el consecutivo para la tabla '{tabla}'.");
            }

            // 3. Devolver el valor anterior (igual que f_consecutivo del legado)
            return numeroActual.ToString();
        });
    }
}