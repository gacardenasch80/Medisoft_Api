using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories;

/// <summary>
/// Repositorio de Gemedicos — base de datos: FoxPro_Gen
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Cada OleDbCommand recibe una copia nueva de los parámetros
///    porque un OleDbParameter no puede pertenecer a dos colecciones.
/// </summary>
public class GemedicosRepository : IGemedicosRepository
{
    private readonly string _conn;

    public GemedicosRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Gen")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Gen' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Gemedicos> Items, int Total)> GetAllAsync(GemedicosFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        // ── Conteo total ──────────────────────────────────────────
        var sqlCount = $"SELECT COUNT(*) FROM Gemedicos {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        // ── Query paginada ────────────────────────────────────────
        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Gemedicos
                {where}
                ORDER BY Gemedicodi";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Gemedicos
                {where}
                AND    Gemedicodi NOT IN (
                           SELECT TOP {offset} Gemedicodi
                           FROM   Gemedicos
                           {where}
                           ORDER BY Gemedicodi
                       )
                ORDER BY Gemedicodi";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Gemedicos?> GetByCodeAsync(string gemedicodi)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Gemedicos
            WHERE  ALLTRIM(Gemedicodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([gemedicodi.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Gemedicos> CreateAsync(Gemedicos e)
    {
        const string sql = @"
            INSERT INTO Gemedicos (
                Gemedicodi, Gemedinomb, Gemedireme, Gereincodi, Gefirmesca,
                Gemeditele, Gemedicelu, Gemedact,   Geesjefeen
            ) VALUES (
                ?,?,?,?,?,
                ?,?,?,?
            )";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Gemedicos> UpdateAsync(Gemedicos e)
    {
        const string sql = @"
            UPDATE Gemedicos SET
                Gemedinomb = ?, Gemedireme = ?, Gereincodi = ?,
                Gefirmesca = ?, Gemeditele = ?, Gemedicelu = ?,
                Gemedact   = ?, Geesjefeen = ?
            WHERE ALLTRIM(Gemedicodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string gemedicodi)
    {
        const string sql = "DELETE FROM Gemedicos WHERE ALLTRIM(Gemedicodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([gemedicodi.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(GemedicosFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Gemedicodi))
        {
            sb.Append(" AND ALLTRIM(Gemedicodi) = ?");
            values.Add(f.Gemedicodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Gemedinomb))
        {
            sb.Append(" AND UPPER(ALLTRIM(Gemedinomb)) LIKE ?");
            values.Add($"%{f.Gemedinomb.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.Gereincodi))
        {
            sb.Append(" AND ALLTRIM(Gereincodi) = ?");
            values.Add(f.Gereincodi.Trim());
        }
        if (f.Gemedact.HasValue)
        {
            sb.Append(" AND Gemedact = ?");
            values.Add(f.Gemedact.Value);
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Gemedicos e) =>
    [
        e.Gemedicodi ?? (object)DBNull.Value,
        e.Gemedinomb ?? (object)DBNull.Value,
        e.Gemedireme ?? (object)DBNull.Value,
        e.Gereincodi ?? (object)DBNull.Value,
        e.Gefirmesca ?? (object)DBNull.Value,
        e.Gemeditele ?? (object)DBNull.Value,
        e.Gemedicelu ?? (object)DBNull.Value,
        e.Gemedact.HasValue   ? e.Gemedact.Value   : (object)DBNull.Value,
        e.Geesjefeen.HasValue ? e.Geesjefeen.Value : (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Gemedicos e) =>
    [
        // SET (sin Gemedicodi — es la clave, no se actualiza)
        e.Gemedinomb ?? (object)DBNull.Value,
        e.Gemedireme ?? (object)DBNull.Value,
        e.Gereincodi ?? (object)DBNull.Value,
        e.Gefirmesca ?? (object)DBNull.Value,
        e.Gemeditele ?? (object)DBNull.Value,
        e.Gemedicelu ?? (object)DBNull.Value,
        e.Gemedact.HasValue   ? e.Gemedact.Value   : (object)DBNull.Value,
        e.Geesjefeen.HasValue ? e.Geesjefeen.Value : (object)DBNull.Value,
        // WHERE
        e.Gemedicodi ?? (object)DBNull.Value,
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

    private static async Task<List<Gemedicos>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Gemedicos>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    private static Gemedicos MapRow(System.Data.IDataRecord r) => new()
    {
        Gemedicodi = r["Gemedicodi"] as string,
        Gemedinomb = r["Gemedinomb"] as string,
        Gemedireme = r["Gemedireme"] as string,
        Gereincodi = r["Gereincodi"] as string,
        Gefirmesca = r["Gefirmesca"] as string,
        Gemeditele = r["Gemeditele"] as string,
        Gemedicelu = r["Gemedicelu"] as string,
        Gemedact = r["Gemedact"] == DBNull.Value ? null : Convert.ToInt32(r["Gemedact"]),
        Geesjefeen = r["Geesjefeen"] == DBNull.Value ? null : Convert.ToInt32(r["Geesjefeen"]),
    };

    // ── Sin CAST — VFP/OleDb no soporta CAST(x AS INT)
    // Convert.ToInt32 en MapRow hace la conversión correctamente.
    private static string SelectColumns() => @"
        Gemedicodi, Gemedinomb, Gemedireme, Gereincodi,
        Gefirmesca, Gemeditele, Gemedicelu,
        Gemedact,   Geesjefeen";
}