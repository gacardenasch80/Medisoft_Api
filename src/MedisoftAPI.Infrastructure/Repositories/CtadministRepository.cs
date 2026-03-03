using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories;

/// <summary>
/// Repositorio de Ctadminist — tabla: Ctadminist
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Cada OleDbCommand recibe una copia nueva de los parámetros
///    porque un OleDbParameter no puede pertenecer a dos colecciones.
/// </summary>
public class CtadministRepository : ICtadministRepository
{
    private readonly string _conn;

    public CtadministRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Con")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Con' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Ctadminist> Items, int Total)> GetAllAsync(CtadministFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        // ── Conteo total ──────────────────────────────────────────
        var sqlCount = $"SELECT COUNT(*) FROM Ctadminist {where}";
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
                FROM   Ctadminist
                {where}
                ORDER BY CTADMICODI";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Ctadminist
                {where}
                AND    CTADMICODI NOT IN (
                           SELECT TOP {offset} CTADMICODI
                           FROM   Ctadminist
                           {where}
                           ORDER BY CTADMICODI
                       )
                ORDER BY CTADMICODI";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Ctadminist?> GetByCodeAsync(string ctadmicodi)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Ctadminist
            WHERE  ALLTRIM(CTADMICODI) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([ctadmicodi.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Ctadminist> CreateAsync(Ctadminist e)
    {
        const string sql = @"
            INSERT INTO Ctadminist (
                CTADMICODI, CTADMINOMB, CTADMINIT,  CTADMISGSS,
                CTADMIDIRE, CTADMITELE, CTADMIRELE, CTADMIEMAI,
                CTADMIPAGI, GEDEPACODI, GEMUNICODI, CTCONCOPYP,
                CTADMIGIDI, FAFERECODI, FAFETIFCOD,
                CTADMTYPER, CTADMRERUT, CTADMIESTA, CTADMSGFAR
            ) VALUES (
                ?,?,?,?,
                ?,?,?,?,
                ?,?,?,?,
                ?,?,?,
                ?,?,?,?
            )";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Ctadminist> UpdateAsync(Ctadminist e)
    {
        const string sql = @"
            UPDATE Ctadminist SET
                CTADMINOMB = ?, CTADMINIT  = ?, CTADMISGSS = ?,
                CTADMIDIRE = ?, CTADMITELE = ?, CTADMIRELE = ?,
                CTADMIEMAI = ?, CTADMIPAGI = ?,
                GEDEPACODI = ?, GEMUNICODI = ?, CTCONCOPYP = ?,
                CTADMIGIDI = ?, FAFERECODI = ?, FAFETIFCOD = ?,
                CTADMTYPER = ?, CTADMRERUT = ?, CTADMIESTA = ?,
                CTADMSGFAR = ?
            WHERE ALLTRIM(CTADMICODI) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string ctadmicodi)
    {
        const string sql = "DELETE FROM Ctadminist WHERE ALLTRIM(CTADMICODI) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([ctadmicodi.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(CtadministFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.CTADMICODI))
        {
            sb.Append(" AND ALLTRIM(CTADMICODI) = ?");
            values.Add(f.CTADMICODI.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.CTADMINOMB))
        {
            sb.Append(" AND UPPER(ALLTRIM(CTADMINOMB)) LIKE ?");
            values.Add($"%{f.CTADMINOMB.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.CTADMISGSS))
        {
            sb.Append(" AND ALLTRIM(CTADMISGSS) = ?");
            values.Add(f.CTADMISGSS.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.GEDEPACODI))
        {
            sb.Append(" AND ALLTRIM(GEDEPACODI) = ?");
            values.Add(f.GEDEPACODI.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.GEMUNICODI))
        {
            sb.Append(" AND ALLTRIM(GEMUNICODI) = ?");
            values.Add(f.GEMUNICODI.Trim());
        }
        if (f.CTADMIESTA.HasValue)
        {
            sb.Append(" AND CTADMIESTA = ?");
            values.Add(f.CTADMIESTA.Value);
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Ctadminist e) =>
    [
        e.CTADMICODI ?? (object)DBNull.Value,
        e.CTADMINOMB ?? (object)DBNull.Value,
        e.CTADMINIT  ?? (object)DBNull.Value,
        e.CTADMISGSS ?? (object)DBNull.Value,
        e.CTADMIDIRE ?? (object)DBNull.Value,
        e.CTADMITELE ?? (object)DBNull.Value,
        e.CTADMIRELE ?? (object)DBNull.Value,
        e.CTADMIEMAI ?? (object)DBNull.Value,
        e.CTADMIPAGI ?? (object)DBNull.Value,
        e.GEDEPACODI ?? (object)DBNull.Value,
        e.GEMUNICODI ?? (object)DBNull.Value,
        e.CTCONCOPYP ?? (object)DBNull.Value,
        e.CTADMIGIDI.HasValue ? e.CTADMIGIDI.Value : (object)DBNull.Value,
        e.FAFERECODI ?? (object)DBNull.Value,
        e.FAFETIFCOD ?? (object)DBNull.Value,
        e.CTADMTYPER.HasValue ? e.CTADMTYPER.Value : (object)DBNull.Value,
        e.CTADMRERUT ?? (object)DBNull.Value,
        e.CTADMIESTA.HasValue ? e.CTADMIESTA.Value : (object)DBNull.Value,
        e.CTADMSGFAR ?? (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Ctadminist e) =>
    [
        // SET (sin CTADMICODI — es la clave, no se actualiza)
        e.CTADMINOMB ?? (object)DBNull.Value,
        e.CTADMINIT  ?? (object)DBNull.Value,
        e.CTADMISGSS ?? (object)DBNull.Value,
        e.CTADMIDIRE ?? (object)DBNull.Value,
        e.CTADMITELE ?? (object)DBNull.Value,
        e.CTADMIRELE ?? (object)DBNull.Value,
        e.CTADMIEMAI ?? (object)DBNull.Value,
        e.CTADMIPAGI ?? (object)DBNull.Value,
        e.GEDEPACODI ?? (object)DBNull.Value,
        e.GEMUNICODI ?? (object)DBNull.Value,
        e.CTCONCOPYP ?? (object)DBNull.Value,
        e.CTADMIGIDI.HasValue ? e.CTADMIGIDI.Value : (object)DBNull.Value,
        e.FAFERECODI ?? (object)DBNull.Value,
        e.FAFETIFCOD ?? (object)DBNull.Value,
        e.CTADMTYPER.HasValue ? e.CTADMTYPER.Value : (object)DBNull.Value,
        e.CTADMRERUT ?? (object)DBNull.Value,
        e.CTADMIESTA.HasValue ? e.CTADMIESTA.Value : (object)DBNull.Value,
        e.CTADMSGFAR ?? (object)DBNull.Value,
        // WHERE
        e.CTADMICODI ?? (object)DBNull.Value,
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

    private static async Task<List<Ctadminist>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Ctadminist>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    private static Ctadminist MapRow(System.Data.IDataRecord r) => new()
    {
        CTADMICODI = r["CTADMICODI"] as string,
        CTADMINOMB = r["CTADMINOMB"] as string,
        CTADMINIT = r["CTADMINIT"] as string,
        CTADMISGSS = r["CTADMISGSS"] as string,
        CTADMIDIRE = r["CTADMIDIRE"] as string,
        CTADMITELE = r["CTADMITELE"] as string,
        CTADMIRELE = r["CTADMIRELE"] as string,
        CTADMIEMAI = r["CTADMIEMAI"] as string,
        CTADMIPAGI = r["CTADMIPAGI"] as string,
        GEDEPACODI = r["GEDEPACODI"] as string,
        GEMUNICODI = r["GEMUNICODI"] as string,
        CTCONCOPYP = r["CTCONCOPYP"] as string,
        CTADMIGIDI = r["CTADMIGIDI"] == DBNull.Value ? null : Convert.ToInt32(r["CTADMIGIDI"]),
        FAFERECODI = r["FAFERECODI"] as string,
        FAFETIFCOD = r["FAFETIFCOD"] as string,
        CTADMTYPER = r["CTADMTYPER"] == DBNull.Value ? null : Convert.ToInt32(r["CTADMTYPER"]),
        CTADMRERUT = r["CTADMRERUT"] as string,
        CTADMIESTA = r["CTADMIESTA"] == DBNull.Value ? null : Convert.ToInt32(r["CTADMIESTA"]),
        CTADMSGFAR = r["CTADMSGFAR"] as string,
    };

    private static string SelectColumns() => @"
        CTADMICODI, CTADMINOMB, CTADMINIT,  CTADMISGSS,
        CTADMIDIRE, CTADMITELE, CTADMIRELE, CTADMIEMAI,
        CTADMIPAGI, GEDEPACODI, GEMUNICODI, CTCONCOPYP,
        CTADMIGIDI, FAFERECODI, FAFETIFCOD,
        CTADMTYPER, CTADMRERUT, CTADMIESTA, CTADMSGFAR";
}