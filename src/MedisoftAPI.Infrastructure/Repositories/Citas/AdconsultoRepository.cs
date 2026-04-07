using System.Data;
using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Interfaces.Citas;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Citas;

/// <summary>
/// Repositorio de Adconsulto — tabla: Adconsulto (citas.dbc)
/// </summary>
public class AdconsultoRepository : IAdconsultoRepository
{
    private readonly string _conn;

    public AdconsultoRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Cit")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Cit' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Adconsulto> Items, int Total)> GetAllAsync(AdconsultoFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Adconsulto {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adconsulto
                {where}
                ORDER BY Adconscodi";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adconsulto
                {where}
                AND    Adconscodi NOT IN (
                           SELECT TOP {offset} Adconscodi
                           FROM   Adconsulto
                           {where}
                           ORDER BY Adconscodi
                       )
                ORDER BY Adconscodi";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Adconsulto?> GetByCodeAsync(string adconscodi)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Adconsulto
            WHERE  ALLTRIM(Adconscodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([adconscodi.Trim()]));
        return items.FirstOrDefault();
    }

    // ── GET BY UNIDADES (para filtro de zonificación) ─────────────────────

    public async Task<IEnumerable<Adconsulto>> GetByUnidadesAsync(IEnumerable<string> geunprcodi)
    {
        var zonas = geunprcodi
            .Where(z => !string.IsNullOrWhiteSpace(z))
            .Select(z => z.Trim())
            .Distinct()
            .ToList();

        if (!zonas.Any())
            return [];

        // VFP soporta IN (...) con literales — se inyectan con ? posicionales
        var placeholders = string.Join(",", zonas.Select(_ => "?"));
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Adconsulto
            WHERE  ALLTRIM(Geunprcodi) IN ({placeholders})
            ORDER BY Adconscodi";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        return await QueryAsync(conn, sql,
            MakeParams(zonas.Cast<object>().ToArray()));
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Adconsulto> CreateAsync(Adconsulto e)
    {
        const string sql = @"
            INSERT INTO Adconsulto (
                Adconscodi, Adconsnomb, Adconsdire, Adconstele, Adconsesur, Geunprcodi
            ) VALUES (?,?,?,?,?,?)";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Adconsulto> UpdateAsync(Adconsulto e)
    {
        const string sql = @"
            UPDATE Adconsulto SET
                Adconsnomb = ?, Adconsdire = ?, Adconstele = ?,
                Adconsesur = ?, Geunprcodi = ?
            WHERE ALLTRIM(Adconscodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string adconscodi)
    {
        const string sql = "DELETE FROM Adconsulto WHERE ALLTRIM(Adconscodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([adconscodi.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(AdconsultoFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Adconscodi))
        {
            sb.Append(" AND ALLTRIM(Adconscodi) = ?");
            values.Add(f.Adconscodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Adconsnomb))
        {
            sb.Append(" AND UPPER(ALLTRIM(Adconsnomb)) LIKE ?");
            values.Add($"%{f.Adconsnomb.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.Adconsdire))
        {
            sb.Append(" AND UPPER(ALLTRIM(Adconsdire)) LIKE ?");
            values.Add($"%{f.Adconsdire.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.Adconstele))
        {
            sb.Append(" AND ALLTRIM(Adconstele) = ?");
            values.Add(f.Adconstele.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Geunprcodi))
        {
            sb.Append(" AND ALLTRIM(Geunprcodi) = ?");
            values.Add(f.Geunprcodi.Trim());
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Adconsulto e) =>
    [
        e.Adconscodi,
        e.Adconsnomb,
        e.Adconsdire,
        e.Adconstele,
        e.Adconsesur.HasValue ? e.Adconsesur.Value : (object)DBNull.Value,
        e.Geunprcodi,
    ];

    private static object[] UpdateValues(Adconsulto e) =>
    [
        e.Adconsnomb,
        e.Adconsdire,
        e.Adconstele,
        e.Adconsesur.HasValue ? e.Adconsesur.Value : (object)DBNull.Value,
        e.Geunprcodi,
        e.Adconscodi,
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

    private static async Task<List<Adconsulto>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Adconsulto>();
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

    private static int? SafeInt(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? null : (int?)Convert.ToInt32(r.GetValue(i)); }
        catch { return null; }
    }

    private static Adconsulto MapRow(IDataRecord r)
    {
        // 0:Adconscodi  1:Adconsnomb  2:Adconsdire
        // 3:Adconstele  4:Adconsesur  5:Geunprcodi
        return new Adconsulto
        {
            Adconscodi = SafeString(r, 0),
            Adconsnomb = SafeString(r, 1),
            Adconsdire = SafeString(r, 2),
            Adconstele = SafeString(r, 3),
            Adconsesur = SafeInt(r, 4),
            Geunprcodi = SafeString(r, 5),
        };
    }

    private static string SelectColumns() =>
        "Adconscodi, Adconsnomb, Adconsdire, Adconstele, Adconsesur, Geunprcodi";
}