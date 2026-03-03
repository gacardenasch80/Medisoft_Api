using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories;

/// <summary>
/// Repositorio de Adadmipaci — tabla: ADADMIPACI.DBF (admision)
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Las fechas Date usan CTOD('MM/dd/yyyy') — función nativa VFP.
///    Los campos Numeric se leen con IsDBNull(ordinal) para evitar
///    InvalidOperationException del proveedor VFP/OleDb.
/// </summary>
public class AdadmipaciRepository : IAdadmipaciRepository
{
    private readonly string _conn;

    public AdadmipaciRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Adm")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Adm' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Adadmipaci> Items, int Total)> GetAllAsync(AdadmipaciFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Adadmipaci {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adadmipaci
                {where}
                ORDER BY ADADPACONS";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adadmipaci
                {where}
                AND    ADADPACONS NOT IN (
                           SELECT TOP {offset} ADADPACONS
                           FROM   Adadmipaci
                           {where}
                           ORDER BY ADADPACONS
                       )
                ORDER BY ADADPACONS";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Adadmipaci?> GetByCodeAsync(string adadpacons)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Adadmipaci
            WHERE  ALLTRIM(ADADPACONS) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([adadpacons.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Adadmipaci> CreateAsync(Adadmipaci e)
    {
        const string sql = @"
            INSERT INTO Adadmipaci (
                ADADPACONS, CTADMICODI, ADPACIIDEN, ADTIAFCODI,
                ADADPADOCU, CTCONTCODI, ADADPAESTA, CTNIVECODI,
                ADADPAFEIN, ADADPAFEFI
            ) VALUES (?,?,?,?,?,?,?,?,?,?)";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Adadmipaci> UpdateAsync(Adadmipaci e)
    {
        const string sql = @"
            UPDATE Adadmipaci SET
                CTADMICODI = ?, ADPACIIDEN = ?, ADTIAFCODI = ?,
                ADADPADOCU = ?, CTCONTCODI = ?, ADADPAESTA = ?,
                CTNIVECODI = ?, ADADPAFEIN = ?, ADADPAFEFI = ?
            WHERE ALLTRIM(ADADPACONS) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string adadpacons)
    {
        const string sql = "DELETE FROM Adadmipaci WHERE ALLTRIM(ADADPACONS) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([adadpacons.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(AdadmipaciFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.ADADPACONS))
        {
            sb.Append(" AND ALLTRIM(ADADPACONS) = ?");
            values.Add(f.ADADPACONS.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.CTADMICODI))
        {
            sb.Append(" AND ALLTRIM(CTADMICODI) = ?");
            values.Add(f.CTADMICODI.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.ADPACIIDEN))
        {
            sb.Append(" AND ALLTRIM(ADPACIIDEN) = ?");
            values.Add(f.ADPACIIDEN.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.CTCONTCODI))
        {
            sb.Append(" AND ALLTRIM(CTCONTCODI) = ?");
            values.Add(f.CTCONTCODI.Trim());
        }
        if (f.ADADPAESTA.HasValue)
        {
            sb.Append(" AND ADADPAESTA = ?");
            values.Add(f.ADADPAESTA.Value);
        }
        if (!string.IsNullOrWhiteSpace(f.CTNIVECODI))
        {
            sb.Append(" AND ALLTRIM(CTNIVECODI) = ?");
            values.Add(f.CTNIVECODI.Trim());
        }
        // Fechas Date con CTOD() — función nativa VFP, formato MM/dd/yyyy
        if (f.FechaInicio.HasValue)
            sb.Append($" AND ADADPAFEIN >= CTOD('{f.FechaInicio.Value:MM/dd/yyyy}')");
        if (f.FechaFin.HasValue)
            sb.Append($" AND ADADPAFEFI <= CTOD('{f.FechaFin.Value:MM/dd/yyyy}')");

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Adadmipaci e) =>
    [
        e.ADADPACONS ?? (object)DBNull.Value,
        e.CTADMICODI ?? (object)DBNull.Value,
        e.ADPACIIDEN ?? (object)DBNull.Value,
        e.ADTIAFCODI ?? (object)DBNull.Value,
        e.ADADPADOCU ?? (object)DBNull.Value,
        e.CTCONTCODI ?? (object)DBNull.Value,
        e.ADADPAESTA.HasValue ? e.ADADPAESTA.Value : (object)DBNull.Value,
        e.CTNIVECODI ?? (object)DBNull.Value,
        e.ADADPAFEIN.HasValue ? e.ADADPAFEIN.Value : (object)DBNull.Value,
        e.ADADPAFEFI.HasValue ? e.ADADPAFEFI.Value : (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Adadmipaci e) =>
    [
        // SET (sin ADADPACONS — es la clave, no se actualiza)
        e.CTADMICODI ?? (object)DBNull.Value,
        e.ADPACIIDEN ?? (object)DBNull.Value,
        e.ADTIAFCODI ?? (object)DBNull.Value,
        e.ADADPADOCU ?? (object)DBNull.Value,
        e.CTCONTCODI ?? (object)DBNull.Value,
        e.ADADPAESTA.HasValue ? e.ADADPAESTA.Value : (object)DBNull.Value,
        e.CTNIVECODI ?? (object)DBNull.Value,
        e.ADADPAFEIN.HasValue ? e.ADADPAFEIN.Value : (object)DBNull.Value,
        e.ADADPAFEFI.HasValue ? e.ADADPAFEFI.Value : (object)DBNull.Value,
        // WHERE
        e.ADADPACONS ?? (object)DBNull.Value,
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

    private static async Task<List<Adadmipaci>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Adadmipaci>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    // ── Safe helpers por ordinal — evitan InvalidOperationException de VFP ──

    private static int? SafeInt(System.Data.IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? null : (int?)Convert.ToInt32(r.GetValue(i)); }
        catch { return null; }
    }

    private static DateTime? SafeDate(System.Data.IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? null : (DateTime?)Convert.ToDateTime(r.GetValue(i)); }
        catch { return null; }
    }

    private static string? SafeString(System.Data.IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? null : r.GetString(i); }
        catch { return null; }
    }

    private static Adadmipaci MapRow(System.Data.IDataRecord r)
    {
        // Índices según el orden exacto de SelectColumns():
        // 0:ADADPACONS  1:CTADMICODI  2:ADPACIIDEN  3:ADTIAFCODI
        // 4:ADADPADOCU  5:CTCONTCODI  6:ADADPAESTA  7:CTNIVECODI
        // 8:ADADPAFEIN  9:ADADPAFEFI
        return new Adadmipaci
        {
            ADADPACONS = SafeString(r, 0),
            CTADMICODI = SafeString(r, 1),
            ADPACIIDEN = SafeString(r, 2),
            ADTIAFCODI = SafeString(r, 3),
            ADADPADOCU = SafeString(r, 4),
            CTCONTCODI = SafeString(r, 5),
            ADADPAESTA = SafeInt(r, 6),
            CTNIVECODI = SafeString(r, 7),
            ADADPAFEIN = SafeDate(r, 8),
            ADADPAFEFI = SafeDate(r, 9),
        };
    }

    private static string SelectColumns() => @"
        ADADPACONS, CTADMICODI, ADPACIIDEN, ADTIAFCODI,
        ADADPADOCU, CTCONTCODI, ADADPAESTA, CTNIVECODI,
        ADADPAFEIN, ADADPAFEFI";
}