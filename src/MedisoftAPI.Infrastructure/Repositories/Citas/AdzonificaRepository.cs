using System.Data;
using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Interfaces.Citas;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Citas;

/// <summary>
/// Repositorio de Adzonifica — tabla: Adzonifica (citas.dbc)
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    La clave primaria es compuesta: Adpaciiden + Geunprcodi.
/// </summary>
public class AdzonificaRepository : IAdzonificaRepository
{
    private readonly string _conn;

    public AdzonificaRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Cit")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Cit' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Adzonifica> Items, int Total)> GetAllAsync(AdzonificaFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Adzonifica {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adzonifica
                {where}
                ORDER BY Adpaciiden, Geunprcodi";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adzonifica
                {where}
                AND    ALLTRIM(Adpaciiden)+ALLTRIM(Geunprcodi) NOT IN (
                           SELECT TOP {offset} ALLTRIM(Adpaciiden)+ALLTRIM(Geunprcodi)
                           FROM   Adzonifica
                           {where}
                           ORDER BY Adpaciiden, Geunprcodi
                       )
                ORDER BY Adpaciiden, Geunprcodi";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY PACIENTE ───────────────────────────────────────────────────

    public async Task<IEnumerable<Adzonifica>> GetByPacienteAsync(string adpaciiden)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Adzonifica
            WHERE  ALLTRIM(Adpaciiden) = ?
            ORDER BY Geunprcodi";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        return await QueryAsync(conn, sql, MakeParams([adpaciiden.Trim()]));
    }

    // ── GET BY PACIENTE + ZONA (clave compuesta) ──────────────────────────

    public async Task<Adzonifica?> GetByPacienteYZonaAsync(string adpaciiden, string geunprcodi)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Adzonifica
            WHERE  ALLTRIM(Adpaciiden) = ?
            AND    ALLTRIM(Geunprcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql,
            MakeParams([adpaciiden.Trim(), geunprcodi.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Adzonifica> CreateAsync(Adzonifica e)
    {
        const string sql = @"
            INSERT INTO Adzonifica (Adpaciiden, Geunprcodi, Estado)
            VALUES (?, ?, ?)";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Adzonifica> UpdateAsync(Adzonifica e)
    {
        const string sql = @"
            UPDATE Adzonifica SET
                Estado = ?
            WHERE ALLTRIM(Adpaciiden) = ?
            AND   ALLTRIM(Geunprcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string adpaciiden, string geunprcodi)
    {
        const string sql = @"
            DELETE FROM Adzonifica
            WHERE ALLTRIM(Adpaciiden) = ?
            AND   ALLTRIM(Geunprcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql,
            MakeParams([adpaciiden.Trim(), geunprcodi.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(AdzonificaFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Adpaciiden))
        {
            sb.Append(" AND ALLTRIM(Adpaciiden) = ?");
            values.Add(f.Adpaciiden.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Geunprcodi))
        {
            sb.Append(" AND ALLTRIM(Geunprcodi) = ?");
            values.Add(f.Geunprcodi.Trim());
        }
        if (f.Estado.HasValue)
        {
            sb.Append(" AND Estado = ?");
            values.Add(f.Estado.Value);
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Adzonifica e) =>
    [
        e.Adpaciiden,
        e.Geunprcodi,
        e.Estado.HasValue ? e.Estado.Value : (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Adzonifica e) =>
    [
        // SET
        e.Estado.HasValue ? e.Estado.Value : (object)DBNull.Value,
        // WHERE (clave compuesta)
        e.Adpaciiden,
        e.Geunprcodi,
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

    private static async Task<List<Adzonifica>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Adzonifica>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    // ── Safe helpers por ordinal ──────────────────────────────────────────

    private static double? SafeDouble(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? null : (double?)Convert.ToDouble(r.GetValue(i)); }
        catch { return null; }
    }

    private static string SafeString(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? string.Empty : r.GetString(i).TrimEnd(); }
        catch { return string.Empty; }
    }

    private static Adzonifica MapRow(IDataRecord r)
    {
        // 0:Adpaciiden  1:Geunprcodi  2:Estado
        return new Adzonifica
        {
            Adpaciiden = SafeString(r, 0),
            Geunprcodi = SafeString(r, 1),
            Estado = SafeDouble(r, 2),
        };
    }

    private static string SelectColumns() => "Adpaciiden, Geunprcodi, Estado";
}