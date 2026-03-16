using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities.Generales;
using MedisoftAPI.Domain.Interfaces.Generales;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Generales;

/// <summary>
/// Repositorio de Geespecial — tabla: Geespecial
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Cada OleDbCommand recibe una copia nueva de los parámetros
///    porque un OleDbParameter no puede pertenecer a dos colecciones.
/// </summary>
public class GeespecialRepository : IGeespecialRepository
{
    private readonly string _conn;

    public GeespecialRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Gen")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Gen' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Geespecial> Items, int Total)> GetAllAsync(GeespecialFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        // ── Conteo total ──────────────────────────────────────────
        var sqlCount = $"SELECT COUNT(*) FROM Geespecial {where}";
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
                FROM   Geespecial
                {where}
                ORDER BY Geespecodi";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Geespecial
                {where}
                AND    Geespecodi NOT IN (
                           SELECT TOP {offset} Geespecodi
                           FROM   Geespecial
                           {where}
                           ORDER BY Geespecodi
                       )
                ORDER BY Geespecodi";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Geespecial?> GetByCodeAsync(string geespecodi)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Geespecial
            WHERE  ALLTRIM(Geespecodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([geespecodi.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Geespecial> CreateAsync(Geespecial e)
    {
        const string sql = @"
            INSERT INTO Geespecial (
                Geespecodi, Geespenomb,
                Geespesv18, Geespeodon, Hcrevartip
            ) VALUES (
                ?,?,
                ?,?,?
            )";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Geespecial> UpdateAsync(Geespecial e)
    {
        const string sql = @"
            UPDATE Geespecial SET
                Geespenomb = ?,
                Geespesv18 = ?, Geespeodon = ?, Hcrevartip = ?
            WHERE ALLTRIM(Geespecodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string geespecodi)
    {
        const string sql = "DELETE FROM Geespecial WHERE ALLTRIM(Geespecodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([geespecodi.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(GeespecialFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Geespecodi))
        {
            sb.Append(" AND ALLTRIM(Geespecodi) = ?");
            values.Add(f.Geespecodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Geespenomb))
        {
            sb.Append(" AND UPPER(ALLTRIM(Geespenomb)) LIKE ?");
            values.Add($"%{f.Geespenomb.ToUpper().Trim()}%");
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Geespecial e) =>
    [
        e.Geespecodi ?? (object)DBNull.Value,
        e.Geespenomb ?? (object)DBNull.Value,
        e.Geespesv18.HasValue  ? e.Geespesv18.Value  : (object)DBNull.Value,
        e.Geespeodon.HasValue  ? e.Geespeodon.Value  : (object)DBNull.Value,
        e.Hcrevartip.HasValue  ? e.Hcrevartip.Value  : (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Geespecial e) =>
    [
        // SET (sin Geespecodi — es la clave, no se actualiza)
        e.Geespenomb ?? (object)DBNull.Value,
        e.Geespesv18.HasValue  ? e.Geespesv18.Value  : (object)DBNull.Value,
        e.Geespeodon.HasValue  ? e.Geespeodon.Value  : (object)DBNull.Value,
        e.Hcrevartip.HasValue  ? e.Hcrevartip.Value  : (object)DBNull.Value,
        // WHERE
        e.Geespecodi ?? (object)DBNull.Value,
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

    private static async Task<List<Geespecial>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Geespecial>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    private static Geespecial MapRow(System.Data.IDataRecord r) => new()
    {
        Geespecodi = r["Geespecodi"] as string,
        Geespenomb = r["Geespenomb"] as string,
        Geespesv18 = r["Geespesv18"] == DBNull.Value ? null : Convert.ToInt32(r["Geespesv18"]),
        Geespeodon = r["Geespeodon"] == DBNull.Value ? null : Convert.ToInt32(r["Geespeodon"]),
        Hcrevartip = r["Hcrevartip"] == DBNull.Value ? null : Convert.ToInt32(r["Hcrevartip"]),
    };

    private static string SelectColumns() => @"
        Geespecodi, Geespenomb,
        Geespesv18, Geespeodon, Hcrevartip";
}