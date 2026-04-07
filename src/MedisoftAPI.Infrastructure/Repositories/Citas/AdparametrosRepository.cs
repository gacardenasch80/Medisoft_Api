using System.Data;
using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Interfaces.Citas;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Citas;

/// <summary>
/// Repositorio de Adparametros — tabla: Adparametros (citas.dbc)
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
/// </summary>
public class AdparametrosRepository : IAdparametrosRepository
{
    private readonly string _conn;

    public AdparametrosRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Cit")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Cit' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Adparametros> Items, int Total)> GetAllAsync(AdparametrosFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Adparametros {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adparametros
                {where}
                ORDER BY Adparametro";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adparametros
                {where}
                AND    Adparametro NOT IN (
                           SELECT TOP {offset} Adparametro
                           FROM   Adparametros
                           {where}
                           ORDER BY Adparametro
                       )
                ORDER BY Adparametro";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Adparametros?> GetByCodeAsync(string adparametro)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Adparametros
            WHERE  ALLTRIM(Adparametro) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([adparametro.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Adparametros> CreateAsync(Adparametros e)
    {
        const string sql = @"
            INSERT INTO Adparametros (Adparametro, Advalorpara)
            VALUES (?, ?)";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Adparametros> UpdateAsync(Adparametros e)
    {
        const string sql = @"
            UPDATE Adparametros SET
                Advalorpara = ?
            WHERE ALLTRIM(Adparametro) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string adparametro)
    {
        const string sql = "DELETE FROM Adparametros WHERE ALLTRIM(Adparametro) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([adparametro.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(AdparametrosFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Adparametro))
        {
            sb.Append(" AND ALLTRIM(Adparametro) = ?");
            values.Add(f.Adparametro.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Advalorpara))
        {
            sb.Append(" AND ALLTRIM(Advalorpara) = ?");
            values.Add(f.Advalorpara.Trim());
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Adparametros e) =>
    [
        e.Adparametro,
        e.Advalorpara,
    ];

    private static object[] UpdateValues(Adparametros e) =>
    [
        // SET
        e.Advalorpara,
        // WHERE
        e.Adparametro,
    ];

    // ── ADO.NET HELPERS ───────────────────────────────────────────────────

    private static OleDbParameter[] MakeParams(object[] values) =>
        values.Select(v => new OleDbParameter("?", v ?? DBNull.Value)).ToArray();

    private static async Task<int> ExecuteScalarAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    private static async Task<int> ExecuteAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);
        return await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<List<Adparametros>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Adparametros>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    private static string SafeString(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? string.Empty : r.GetString(i).TrimEnd(); }
        catch { return string.Empty; }
    }

    private static Adparametros MapRow(IDataRecord r)
    {
        // 0:Adparametro  1:Advalorpara
        return new Adparametros
        {
            Adparametro = SafeString(r, 0),
            Advalorpara = SafeString(r, 1),
        };
    }

    private static string SelectColumns() => "Adparametro, Advalorpara";
}