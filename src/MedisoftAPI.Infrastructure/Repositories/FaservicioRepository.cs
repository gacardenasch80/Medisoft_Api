using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories;

/// <summary>
/// Repositorio de FASERVICIO — base de datos: facturacion.dbc (FoxPro_Fac)
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Cada OleDbCommand recibe una copia nueva de los parámetros
///    porque un OleDbParameter no puede pertenecer a dos colecciones.
/// </summary>
public class FaservicioRepository : IFaservicioRepository
{
    private readonly string _conn;

    public FaservicioRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Fac")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Fac' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Faservicio> Items, int Total)> GetAllAsync(FaservicioFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        // ── Conteo total ──────────────────────────────────────────
        var sqlCount = $"SELECT COUNT(*) FROM FASERVICIO {where}";
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
                FROM   FASERVICIO
                {where}
                ORDER BY FASERVCODI";
            queryValues = paramValues;
        }
        else
        {
            // La subquery repite el mismo WHERE → duplicar los valores
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   FASERVICIO
                {where}
                AND    FASERVCODI NOT IN (
                           SELECT TOP {offset} FASERVCODI
                           FROM   FASERVICIO
                           {where}
                           ORDER BY FASERVCODI
                       )
                ORDER BY FASERVCODI";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Faservicio?> GetByCodeAsync(string codserv)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   FASERVICIO
            WHERE  ALLTRIM(FASERVCODI) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([codserv.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Faservicio> CreateAsync(Faservicio e)
    {
        const string sql = @"
            INSERT INTO FASERVICIO (
                CTCLMACODI, FACLSECODI, FASUBCLCODI, FASERVCODI, FASERVNOMB,
                FASERVPROG, FASERVCONS, FASERVPART,  FAFISECODI, FASERVTIPO,
                FASERVOBS,  FASERVPAQU, FAAGSECODI,  FASERVDUCI, FASERVADICI,
                FASERVPRIV, FASERVINTE, FASERVENFE,  FASERVFREC, FASERVTRAN,
                FAESVAC,    FASERVTRAP, FASERVTRAS,  FASERVRX,   FAESTERAPI,
                FASERVESTA, FASERV2175, FAINCAPCID
            ) VALUES (
                ?,?,?,?,?,
                ?,?,?,?,?,
                ?,?,?,?,?,
                ?,?,?,?,?,
                ?,?,?,?,?,
                ?,?,?
            )";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Faservicio> UpdateAsync(Faservicio e)
    {
        const string sql = @"
            UPDATE FASERVICIO SET
                CTCLMACODI  = ?, FACLSECODI  = ?,
                FASUBCLCODI = ?, FASERVNOMB  = ?,
                FASERVPROG  = ?, FASERVCONS  = ?,
                FASERVPART  = ?, FAFISECODI  = ?,
                FASERVTIPO  = ?, FASERVOBS   = ?,
                FASERVPAQU  = ?, FAAGSECODI  = ?,
                FASERVDUCI  = ?, FASERVADICI = ?,
                FASERVPRIV  = ?, FASERVINTE  = ?,
                FASERVENFE  = ?, FASERVFREC  = ?,
                FASERVTRAN  = ?, FAESVAC     = ?,
                FASERVTRAP  = ?, FASERVTRAS  = ?,
                FASERVRX    = ?, FAESTERAPI  = ?,
                FASERVESTA  = ?, FASERV2175  = ?,
                FAINCAPCID  = ?
            WHERE ALLTRIM(FASERVCODI) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string codserv)
    {
        const string sql = "DELETE FROM FASERVICIO WHERE ALLTRIM(FASERVCODI) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([codserv.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(FaservicioFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.FASERVCODI))
        {
            sb.Append(" AND ALLTRIM(FASERVCODI) = ?");
            values.Add(f.FASERVCODI.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.FASERVNOMB))
        {
            sb.Append(" AND UPPER(ALLTRIM(FASERVNOMB)) LIKE ?");
            values.Add($"%{f.FASERVNOMB.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.FASERVESTA))
        {
            sb.Append(" AND ALLTRIM(FASERVESTA) = ?");
            values.Add(f.FASERVESTA.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.CTCLMACODI))
        {
            sb.Append(" AND ALLTRIM(CTCLMACODI) = ?");
            values.Add(f.CTCLMACODI.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.FACLSECODI))
        {
            sb.Append(" AND ALLTRIM(FACLSECODI) = ?");
            values.Add(f.FACLSECODI.Trim());
        }
        if (f.FASERVTIPO.HasValue)
        {
            sb.Append(" AND FASERVTIPO = ?");
            values.Add(f.FASERVTIPO.Value);
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Faservicio e) =>
    [
        e.CTCLMACODI  ?? (object)DBNull.Value,
        e.FACLSECODI  ?? (object)DBNull.Value,
        e.FASUBCLCODI ?? (object)DBNull.Value,
        e.FASERVCODI  ?? (object)DBNull.Value,
        e.FASERVNOMB  ?? (object)DBNull.Value,
        e.FASERVPROG.HasValue ? e.FASERVPROG.Value : (object)DBNull.Value,
        e.FASERVCONS.HasValue ? e.FASERVCONS.Value : (object)DBNull.Value,
        e.FASERVPART.HasValue ? e.FASERVPART.Value : (object)DBNull.Value,
        e.FAFISECODI  ?? (object)DBNull.Value,
        e.FASERVTIPO.HasValue ? e.FASERVTIPO.Value : (object)DBNull.Value,
        e.FASERVOBS.HasValue  ? e.FASERVOBS.Value  : (object)DBNull.Value,
        e.FASERVPAQU.HasValue ? e.FASERVPAQU.Value : (object)DBNull.Value,
        e.FAAGSECODI  ?? (object)DBNull.Value,
        e.FASERVDUCI.HasValue ? e.FASERVDUCI.Value : (object)DBNull.Value,
        e.FASERVADICI ?? (object)DBNull.Value,
        e.FASERVPRIV.HasValue ? e.FASERVPRIV.Value : (object)DBNull.Value,
        e.FASERVINTE.HasValue ? e.FASERVINTE.Value : (object)DBNull.Value,
        e.FASERVENFE.HasValue ? e.FASERVENFE.Value : (object)DBNull.Value,
        e.FASERVFREC.HasValue ? e.FASERVFREC.Value : (object)DBNull.Value,
        e.FASERVTRAN.HasValue ? e.FASERVTRAN.Value : (object)DBNull.Value,
        e.FAESVAC.HasValue    ? e.FAESVAC.Value    : (object)DBNull.Value,
        e.FASERVTRAP.HasValue ? e.FASERVTRAP.Value : (object)DBNull.Value,
        e.FASERVTRAS.HasValue ? e.FASERVTRAS.Value : (object)DBNull.Value,
        e.FASERVRX.HasValue   ? e.FASERVRX.Value   : (object)DBNull.Value,
        e.FAESTERAPI.HasValue ? e.FAESTERAPI.Value : (object)DBNull.Value,
        e.FASERVESTA  ?? (object)DBNull.Value,
        e.FASERV2175.HasValue ? e.FASERV2175.Value : (object)DBNull.Value,
        e.FAINCAPCID.HasValue ? e.FAINCAPCID.Value : (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Faservicio e) =>
    [
        // SET (sin FASERVCODI — es la clave, no se actualiza)
        e.CTCLMACODI  ?? (object)DBNull.Value,
        e.FACLSECODI  ?? (object)DBNull.Value,
        e.FASUBCLCODI ?? (object)DBNull.Value,
        e.FASERVNOMB  ?? (object)DBNull.Value,
        e.FASERVPROG.HasValue ? e.FASERVPROG.Value : (object)DBNull.Value,
        e.FASERVCONS.HasValue ? e.FASERVCONS.Value : (object)DBNull.Value,
        e.FASERVPART.HasValue ? e.FASERVPART.Value : (object)DBNull.Value,
        e.FAFISECODI  ?? (object)DBNull.Value,
        e.FASERVTIPO.HasValue ? e.FASERVTIPO.Value : (object)DBNull.Value,
        e.FASERVOBS.HasValue  ? e.FASERVOBS.Value  : (object)DBNull.Value,
        e.FASERVPAQU.HasValue ? e.FASERVPAQU.Value : (object)DBNull.Value,
        e.FAAGSECODI  ?? (object)DBNull.Value,
        e.FASERVDUCI.HasValue ? e.FASERVDUCI.Value : (object)DBNull.Value,
        e.FASERVADICI ?? (object)DBNull.Value,
        e.FASERVPRIV.HasValue ? e.FASERVPRIV.Value : (object)DBNull.Value,
        e.FASERVINTE.HasValue ? e.FASERVINTE.Value : (object)DBNull.Value,
        e.FASERVENFE.HasValue ? e.FASERVENFE.Value : (object)DBNull.Value,
        e.FASERVFREC.HasValue ? e.FASERVFREC.Value : (object)DBNull.Value,
        e.FASERVTRAN.HasValue ? e.FASERVTRAN.Value : (object)DBNull.Value,
        e.FAESVAC.HasValue    ? e.FAESVAC.Value    : (object)DBNull.Value,
        e.FASERVTRAP.HasValue ? e.FASERVTRAP.Value : (object)DBNull.Value,
        e.FASERVTRAS.HasValue ? e.FASERVTRAS.Value : (object)DBNull.Value,
        e.FASERVRX.HasValue   ? e.FASERVRX.Value   : (object)DBNull.Value,
        e.FAESTERAPI.HasValue ? e.FAESTERAPI.Value : (object)DBNull.Value,
        e.FASERVESTA  ?? (object)DBNull.Value,
        e.FASERV2175.HasValue ? e.FASERV2175.Value : (object)DBNull.Value,
        e.FAINCAPCID.HasValue ? e.FAINCAPCID.Value : (object)DBNull.Value,
        // WHERE
        e.FASERVCODI  ?? (object)DBNull.Value,
    ];

    // ── ADO.NET HELPERS ───────────────────────────────────────────────────

    /// <summary>
    /// Crea OleDbParameter[] NUEVOS a partir de los valores.
    /// Nunca reutiliza instancias — evita "already contained" exception.
    /// </summary>
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

    private static async Task<List<Faservicio>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Faservicio>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    private static Faservicio MapRow(System.Data.IDataRecord r) => new()
    {
        CTCLMACODI = r["CTCLMACODI"] as string,
        FACLSECODI = r["FACLSECODI"] as string,
        FASUBCLCODI = r["FASUBCLCODI"] as string,
        FASERVCODI = r["FASERVCODI"] as string,
        FASERVNOMB = r["FASERVNOMB"] as string,
        FASERVPROG = r["FASERVPROG"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVPROG"]),
        FASERVCONS = r["FASERVCONS"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVCONS"]),
        FASERVPART = r["FASERVPART"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVPART"]),
        FAFISECODI = r["FAFISECODI"] as string,
        FASERVTIPO = r["FASERVTIPO"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVTIPO"]),
        FASERVOBS = r["FASERVOBS"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVOBS"]),
        FASERVPAQU = r["FASERVPAQU"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVPAQU"]),
        FAAGSECODI = r["FAAGSECODI"] as string,
        FASERVDUCI = r["FASERVDUCI"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVDUCI"]),
        FASERVADICI = r["FASERVADICI"] as string,
        FASERVPRIV = r["FASERVPRIV"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVPRIV"]),
        FASERVINTE = r["FASERVINTE"] == DBNull.Value ? null : Convert.ToBoolean(r["FASERVINTE"]),
        FASERVENFE = r["FASERVENFE"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVENFE"]),
        FASERVFREC = r["FASERVFREC"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVFREC"]),
        FASERVTRAN = r["FASERVTRAN"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVTRAN"]),
        FAESVAC = r["FAESVAC"] == DBNull.Value ? null : Convert.ToInt32(r["FAESVAC"]),
        FASERVTRAP = r["FASERVTRAP"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVTRAP"]),
        FASERVTRAS = r["FASERVTRAS"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVTRAS"]),
        FASERVRX = r["FASERVRX"] == DBNull.Value ? null : Convert.ToInt32(r["FASERVRX"]),
        FAESTERAPI = r["FAESTERAPI"] == DBNull.Value ? null : Convert.ToInt32(r["FAESTERAPI"]),
        FASERVESTA = r["FASERVESTA"] as string,
        FASERV2175 = r["FASERV2175"] == DBNull.Value ? null : Convert.ToInt32(r["FASERV2175"]),
        FAINCAPCID = r["FAINCAPCID"] == DBNull.Value ? null : Convert.ToInt32(r["FAINCAPCID"]),
    };

    // ── Sin CAST — VFP/OleDb no soporta CAST(x AS INT)
    // Dapper/ADO.NET mapea los Numeric de VFP como double,
    // Convert.ToInt32 en MapRow hace la conversión correctamente.
    private static string SelectColumns() => @"
        CTCLMACODI, FACLSECODI, FASUBCLCODI, FASERVCODI, FASERVNOMB,
        FASERVPROG, FASERVCONS, FASERVPART,  FAFISECODI, FASERVTIPO,
        FASERVOBS,  FASERVPAQU, FAAGSECODI,  FASERVDUCI, FASERVADICI,
        FASERVPRIV, FASERVINTE, FASERVENFE,  FASERVFREC, FASERVTRAN,
        FAESVAC,    FASERVTRAP, FASERVTRAS,  FASERVRX,   FAESTERAPI,
        FASERVESTA, FASERV2175, FAINCAPCID";
}