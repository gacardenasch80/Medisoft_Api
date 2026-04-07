using System.Data;
using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities.Generales;
using MedisoftAPI.Domain.Interfaces.Generales;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Generales;

/// <summary>
/// Repositorio de Geunidprod — tabla: Geunidprod (generales.dbc)
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
/// </summary>
public class GeunidprodRepository : IGeunidprodRepository
{
    private readonly string _conn;

    public GeunidprodRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Gen")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Gen' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Geunidprod> Items, int Total)> GetAllAsync(GeunidprodFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Geunidprod {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Geunidprod
                {where}
                ORDER BY Geunprcodi";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Geunidprod
                {where}
                AND    Geunprcodi NOT IN (
                           SELECT TOP {offset} Geunprcodi
                           FROM   Geunidprod
                           {where}
                           ORDER BY Geunprcodi
                       )
                ORDER BY Geunprcodi";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Geunidprod?> GetByCodeAsync(string geunprcodi)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Geunidprod
            WHERE  ALLTRIM(Geunprcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([geunprcodi.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Geunidprod> CreateAsync(Geunidprod e)
    {
        const string sql = @"
            INSERT INTO Geunidprod (
                Geunprcodi, Geunprnomb, Geunprdire,
                Geunprtele, Geunprresp,  Gefarmpref
            ) VALUES (?,?,?,?,?,?)";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Geunidprod> UpdateAsync(Geunidprod e)
    {
        const string sql = @"
            UPDATE Geunidprod SET
                Geunprnomb = ?, Geunprdire = ?,
                Geunprtele = ?, Geunprresp  = ?, Gefarmpref = ?
            WHERE ALLTRIM(Geunprcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string geunprcodi)
    {
        const string sql = "DELETE FROM Geunidprod WHERE ALLTRIM(Geunprcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([geunprcodi.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(GeunidprodFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Geunprcodi))
        {
            sb.Append(" AND ALLTRIM(Geunprcodi) = ?");
            values.Add(f.Geunprcodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Geunprnomb))
        {
            sb.Append(" AND UPPER(ALLTRIM(Geunprnomb)) LIKE ?");
            values.Add($"%{f.Geunprnomb.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.Geunprresp))
        {
            sb.Append(" AND UPPER(ALLTRIM(Geunprresp)) LIKE ?");
            values.Add($"%{f.Geunprresp.ToUpper().Trim()}%");
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Geunidprod e) =>
    [
        e.Geunprcodi, e.Geunprnomb, e.Geunprdire,
        e.Geunprtele, e.Geunprresp,  e.Gefarmpref,
    ];

    private static object[] UpdateValues(Geunidprod e) =>
    [
        // SET (sin Geunprcodi — es la clave)
        e.Geunprnomb, e.Geunprdire,
        e.Geunprtele, e.Geunprresp, e.Gefarmpref,
        // WHERE
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

    private static async Task<List<Geunidprod>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Geunidprod>();
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

    private static Geunidprod MapRow(IDataRecord r)
    {
        // 0:Geunprcodi  1:Geunprnomb  2:Geunprdire
        // 3:Geunprtele  4:Geunprresp   5:Gefarmpref
        return new Geunidprod
        {
            Geunprcodi = SafeString(r, 0),
            Geunprnomb = SafeString(r, 1),
            Geunprdire = SafeString(r, 2),
            Geunprtele = SafeString(r, 3),
            Geunprresp = SafeString(r, 4),
            Gefarmpref = SafeString(r, 5),
        };
    }

    private static string SelectColumns() =>
        "Geunprcodi, Geunprnomb, Geunprdire, Geunprtele, Geunprresp, Gefarmpref";
}