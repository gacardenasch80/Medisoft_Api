using System.Data;
using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories;

/// <summary>
/// Repositorio de Faprogpyp — tabla: Faprogpyp (facturacion.dbc)
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Los campos Numeric se leen con IsDBNull(ordinal) para evitar
///    InvalidOperationException del proveedor VFP/OleDb.
/// </summary>
public class FaprogpypRepository : IFaprogpypRepository
{
    private readonly string _conn;

    public FaprogpypRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Fac")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Fac' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Faprogpyp> Items, int Total)> GetAllAsync(FaprogpypFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Faprogpyp {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Faprogpyp
                {where}
                ORDER BY Faprogcodi";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Faprogpyp
                {where}
                AND    Faprogcodi NOT IN (
                           SELECT TOP {offset} Faprogcodi
                           FROM   Faprogpyp
                           {where}
                           ORDER BY Faprogcodi
                       )
                ORDER BY Faprogcodi";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Faprogpyp?> GetByCodeAsync(string faprogcodi)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Faprogpyp
            WHERE  ALLTRIM(Faprogcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([faprogcodi.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Faprogpyp> CreateAsync(Faprogpyp e)
    {
        const string sql = @"
            INSERT INTO Faprogpyp (
                Faprogcodi, Faprogcod1, Faprognomb, Faprogclas,
                Faprogdesd, Faproghast, Faproggene, Faprogfrec,
                Faficocodi, Fafisecodi, Hcformular, Hcforprive,
                Hcenfeprve, Hcenfectrl, Faservcodi, Famesecoco,
                Famesecopr, Faprogestad
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Faprogpyp> UpdateAsync(Faprogpyp e)
    {
        const string sql = @"
            UPDATE Faprogpyp SET
                Faprogcod1 = ?, Faprognomb = ?, Faprogclas = ?,
                Faprogdesd = ?, Faproghast = ?, Faproggene = ?, Faprogfrec = ?,
                Faficocodi = ?, Fafisecodi = ?, Hcformular = ?, Hcforprive = ?,
                Hcenfeprve = ?, Hcenfectrl = ?, Faservcodi = ?, Famesecoco = ?,
                Famesecopr = ?, Faprogestad = ?
            WHERE ALLTRIM(Faprogcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string faprogcodi)
    {
        const string sql = "DELETE FROM Faprogpyp WHERE ALLTRIM(Faprogcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([faprogcodi.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(FaprogpypFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Faprogcodi))
        {
            sb.Append(" AND ALLTRIM(Faprogcodi) = ?");
            values.Add(f.Faprogcodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Faprogcod1))
        {
            sb.Append(" AND ALLTRIM(Faprogcod1) = ?");
            values.Add(f.Faprogcod1.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Faprognomb))
        {
            sb.Append(" AND UPPER(ALLTRIM(Faprognomb)) LIKE ?");
            values.Add($"%{f.Faprognomb.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.Faprogclas))
        {
            sb.Append(" AND ALLTRIM(Faprogclas) = ?");
            values.Add(f.Faprogclas.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Faservcodi))
        {
            sb.Append(" AND ALLTRIM(Faservcodi) = ?");
            values.Add(f.Faservcodi.Trim());
        }
        if (f.Faprogestad.HasValue)
        {
            sb.Append(" AND Faprogestad = ?");
            values.Add(f.Faprogestad.Value);
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Faprogpyp e) =>
    [
        e.Faprogcodi,
        e.Faprogcod1,
        e.Faprognomb,
        e.Faprogclas,
        e.Faprogdesd.HasValue  ? e.Faprogdesd.Value  : (object)DBNull.Value,
        e.Faproghast.HasValue  ? e.Faproghast.Value  : (object)DBNull.Value,
        e.Faproggene.HasValue  ? e.Faproggene.Value  : (object)DBNull.Value,
        e.Faprogfrec.HasValue  ? e.Faprogfrec.Value  : (object)DBNull.Value,
        e.Faficocodi,
        e.Fafisecodi,
        e.Hcformular,
        e.Hcforprive,
        e.Hcenfeprve,
        e.Hcenfectrl,
        e.Faservcodi,
        e.Famesecoco,
        e.Famesecopr,
        e.Faprogestad.HasValue ? e.Faprogestad.Value : (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Faprogpyp e) =>
    [
        // SET (sin Faprogcodi — es la clave)
        e.Faprogcod1,
        e.Faprognomb,
        e.Faprogclas,
        e.Faprogdesd.HasValue  ? e.Faprogdesd.Value  : (object)DBNull.Value,
        e.Faproghast.HasValue  ? e.Faproghast.Value  : (object)DBNull.Value,
        e.Faproggene.HasValue  ? e.Faproggene.Value  : (object)DBNull.Value,
        e.Faprogfrec.HasValue  ? e.Faprogfrec.Value  : (object)DBNull.Value,
        e.Faficocodi,
        e.Fafisecodi,
        e.Hcformular,
        e.Hcforprive,
        e.Hcenfeprve,
        e.Hcenfectrl,
        e.Faservcodi,
        e.Famesecoco,
        e.Famesecopr,
        e.Faprogestad.HasValue ? e.Faprogestad.Value : (object)DBNull.Value,
        // WHERE
        e.Faprogcodi,
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

    private static async Task<List<Faprogpyp>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Faprogpyp>();
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

    private static int? SafeInt(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? null : (int?)Convert.ToInt32(r.GetValue(i)); }
        catch { return null; }
    }

    private static string SafeString(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? string.Empty : r.GetString(i).TrimEnd(); }
        catch { return string.Empty; }
    }

    private static Faprogpyp MapRow(IDataRecord r)
    {
        //  0:Faprogcodi   1:Faprogcod1   2:Faprognomb   3:Faprogclas
        //  4:Faprogdesd   5:Faproghast   6:Faproggene   7:Faprogfrec
        //  8:Faficocodi   9:Fafisecodi  10:Hcformular  11:Hcforprive
        // 12:Hcenfeprve  13:Hcenfectrl  14:Faservcodi  15:Famesecoco
        // 16:Famesecopr  17:Faprogestad
        return new Faprogpyp
        {
            Faprogcodi = SafeString(r, 0),
            Faprogcod1 = SafeString(r, 1),
            Faprognomb = SafeString(r, 2),
            Faprogclas = SafeString(r, 3),
            Faprogdesd = SafeDouble(r, 4),
            Faproghast = SafeDouble(r, 5),
            Faproggene = SafeInt(r, 6),
            Faprogfrec = SafeInt(r, 7),
            Faficocodi = SafeString(r, 8),
            Fafisecodi = SafeString(r, 9),
            Hcformular = SafeString(r, 10),
            Hcforprive = SafeString(r, 11),
            Hcenfeprve = SafeString(r, 12),
            Hcenfectrl = SafeString(r, 13),
            Faservcodi = SafeString(r, 14),
            Famesecoco = SafeString(r, 15),
            Famesecopr = SafeString(r, 16),
            Faprogestad = SafeInt(r, 17),
        };
    }

    private static string SelectColumns() => @"
        Faprogcodi, Faprogcod1, Faprognomb, Faprogclas,
        Faprogdesd, Faproghast, Faproggene, Faprogfrec,
        Faficocodi, Fafisecodi, Hcformular, Hcforprive,
        Hcenfeprve, Hcenfectrl, Faservcodi, Famesecoco,
        Famesecopr, Faprogestad";
}