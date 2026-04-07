using System.Data;
using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities.Generales;
using MedisoftAPI.Domain.Interfaces.Generales;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Generales;

/// <summary>
/// Repositorio de Gehospital — tabla: Gehospital (generales.dbc)
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Los campos Numeric/Integer se leen con IsDBNull(ordinal)
///    para evitar InvalidOperationException del proveedor VFP/OleDb.
/// </summary>
public class GehospitalRepository : IGehospitalRepository
{
    private readonly string _conn;

    public GehospitalRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Gen")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Gen' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Gehospital> Items, int Total)> GetAllAsync(GehospitalFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Gehospital {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Gehospital
                {where}
                ORDER BY Gehospcodi";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Gehospital
                {where}
                AND    Gehospcodi NOT IN (
                           SELECT TOP {offset} Gehospcodi
                           FROM   Gehospital
                           {where}
                           ORDER BY Gehospcodi
                       )
                ORDER BY Gehospcodi";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Gehospital?> GetByCodeAsync(string gehospcodi)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Gehospital
            WHERE  ALLTRIM(Gehospcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([gehospcodi.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Gehospital> CreateAsync(Gehospital e)
    {
        const string sql = @"
            INSERT INTO Gehospital (
                Gehospcodi,  Gehospnomb,  Gehosptiid,  Gehospnit,   Gehospdire,
                Gehosptele,  Gehospreso,  Gehospisgss, Gehosprele,  Gehospemai,
                Gehosppagi,  Gedepacodi,  Gemunicodi,  Gehosprefa,  Gehospreec,
                Geresudian,  Gemensage,   Gehospdigi,  Gehospindi,  Gehospext,
                Gemoduacti,  Gehoenviro,  Gehosetpru,  Gehocurren,  Gehocertif,
                Gehosubtyp,  Faferecodi,  Gehofaeles,  Gehomailsi,  Fafetifcod,
                Gehostyper,  Gehosrerut,  Gehosquaid,  Gehosquaes,  Gehosproxy,
                Gehoslogid,  Gehosimgid,  Gehocerdia,  Fafetpdcod
            ) VALUES (
                ?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?
            )";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Gehospital> UpdateAsync(Gehospital e)
    {
        const string sql = @"
            UPDATE Gehospital SET
                Gehospnomb  = ?, Gehosptiid  = ?, Gehospnit   = ?, Gehospdire  = ?,
                Gehosptele  = ?, Gehospreso  = ?, Gehospisgss = ?, Gehosprele  = ?,
                Gehospemai  = ?, Gehosppagi  = ?, Gedepacodi  = ?, Gemunicodi  = ?,
                Gehosprefa  = ?, Gehospreec  = ?, Geresudian  = ?, Gemensage   = ?,
                Gehospdigi  = ?, Gehospindi  = ?, Gehospext   = ?, Gemoduacti  = ?,
                Gehoenviro  = ?, Gehosetpru  = ?, Gehocurren  = ?, Gehocertif  = ?,
                Gehosubtyp  = ?, Faferecodi  = ?, Gehofaeles  = ?, Gehomailsi  = ?,
                Fafetifcod  = ?, Gehostyper  = ?, Gehosrerut  = ?, Gehosquaid  = ?,
                Gehosquaes  = ?, Gehosproxy  = ?, Gehoslogid  = ?, Gehosimgid  = ?,
                Gehocerdia  = ?, Fafetpdcod  = ?
            WHERE ALLTRIM(Gehospcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string gehospcodi)
    {
        const string sql = "DELETE FROM Gehospital WHERE ALLTRIM(Gehospcodi) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([gehospcodi.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(GehospitalFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Gehospcodi))
        {
            sb.Append(" AND ALLTRIM(Gehospcodi) = ?");
            values.Add(f.Gehospcodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Gehospnomb))
        {
            sb.Append(" AND UPPER(ALLTRIM(Gehospnomb)) LIKE ?");
            values.Add($"%{f.Gehospnomb.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.Gehospnit))
        {
            sb.Append(" AND ALLTRIM(Gehospnit) = ?");
            values.Add(f.Gehospnit.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Gedepacodi))
        {
            sb.Append(" AND ALLTRIM(Gedepacodi) = ?");
            values.Add(f.Gedepacodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Gemunicodi))
        {
            sb.Append(" AND ALLTRIM(Gemunicodi) = ?");
            values.Add(f.Gemunicodi.Trim());
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Gehospital e) =>
    [
        e.Gehospcodi,  e.Gehospnomb,  e.Gehosptiid,  e.Gehospnit,   e.Gehospdire,
        e.Gehosptele,  e.Gehospreso,  e.Gehospisgss, e.Gehosprele,  e.Gehospemai,
        e.Gehosppagi,  e.Gedepacodi,  e.Gemunicodi,  e.Gehosprefa,  e.Gehospreec,
        e.Geresudian,  e.Gemensage,   e.Gehospdigi,  e.Gehospindi,  e.Gehospext,
        e.Gemoduacti.HasValue  ? e.Gemoduacti.Value  : (object)DBNull.Value,
        e.Gehoenviro.HasValue  ? e.Gehoenviro.Value  : (object)DBNull.Value,
        e.Gehosetpru,  e.Gehocurren,  e.Gehocertif,
        e.Gehosubtyp.HasValue  ? e.Gehosubtyp.Value  : (object)DBNull.Value,
        e.Faferecodi,
        e.Gehofaeles.HasValue  ? e.Gehofaeles.Value  : (object)DBNull.Value,
        e.Gehomailsi,  e.Fafetifcod,
        e.Gehostyper.HasValue  ? e.Gehostyper.Value  : (object)DBNull.Value,
        e.Gehosrerut,  e.Gehosquaid,
        e.Gehosquaes.HasValue  ? e.Gehosquaes.Value  : (object)DBNull.Value,
        e.Gehosproxy,  e.Gehoslogid,  e.Gehosimgid,  e.Gehocerdia,  e.Fafetpdcod,
    ];

    private static object[] UpdateValues(Gehospital e) =>
    [
        // SET (sin Gehospcodi — es la clave)
        e.Gehospnomb,  e.Gehosptiid,  e.Gehospnit,   e.Gehospdire,
        e.Gehosptele,  e.Gehospreso,  e.Gehospisgss, e.Gehosprele,
        e.Gehospemai,  e.Gehosppagi,  e.Gedepacodi,  e.Gemunicodi,
        e.Gehosprefa,  e.Gehospreec,  e.Geresudian,  e.Gemensage,
        e.Gehospdigi,  e.Gehospindi,  e.Gehospext,
        e.Gemoduacti.HasValue  ? e.Gemoduacti.Value  : (object)DBNull.Value,
        e.Gehoenviro.HasValue  ? e.Gehoenviro.Value  : (object)DBNull.Value,
        e.Gehosetpru,  e.Gehocurren,  e.Gehocertif,
        e.Gehosubtyp.HasValue  ? e.Gehosubtyp.Value  : (object)DBNull.Value,
        e.Faferecodi,
        e.Gehofaeles.HasValue  ? e.Gehofaeles.Value  : (object)DBNull.Value,
        e.Gehomailsi,  e.Fafetifcod,
        e.Gehostyper.HasValue  ? e.Gehostyper.Value  : (object)DBNull.Value,
        e.Gehosrerut,  e.Gehosquaid,
        e.Gehosquaes.HasValue  ? e.Gehosquaes.Value  : (object)DBNull.Value,
        e.Gehosproxy,  e.Gehoslogid,  e.Gehosimgid,  e.Gehocerdia,  e.Fafetpdcod,
        // WHERE
        e.Gehospcodi,
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

    private static async Task<List<Gehospital>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Gehospital>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    // ── Safe helpers por ordinal ──────────────────────────────────────────

    private static int? SafeInt(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? null : (int?)Convert.ToInt32(r.GetValue(i)); }
        catch { return null; }
    }

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

    private static Gehospital MapRow(IDataRecord r)
    {
        //  0:Gehospcodi   1:Gehospnomb   2:Gehosptiid   3:Gehospnit    4:Gehospdire
        //  5:Gehosptele   6:Gehospreso   7:Gehospisgss  8:Gehosprele   9:Gehospemai
        // 10:Gehosppagi  11:Gedepacodi  12:Gemunicodi  13:Gehosprefa  14:Gehospreec
        // 15:Geresudian  16:Gemensage   17:Gehospdigi  18:Gehospindi  19:Gehospext
        // 20:Gemoduacti  21:Gehoenviro  22:Gehosetpru  23:Gehocurren  24:Gehocertif
        // 25:Gehosubtyp  26:Faferecodi  27:Gehofaeles  28:Gehomailsi  29:Fafetifcod
        // 30:Gehostyper  31:Gehosrerut  32:Gehosquaid  33:Gehosquaes  34:Gehosproxy
        // 35:Gehoslogid  36:Gehosimgid  37:Gehocerdia  38:Fafetpdcod
        return new Gehospital
        {
            Gehospcodi = SafeString(r, 0),
            Gehospnomb = SafeString(r, 1),
            Gehosptiid = SafeString(r, 2),
            Gehospnit = SafeString(r, 3),
            Gehospdire = SafeString(r, 4),
            Gehosptele = SafeString(r, 5),
            Gehospreso = SafeString(r, 6),
            Gehospisgss = SafeString(r, 7),
            Gehosprele = SafeString(r, 8),
            Gehospemai = SafeString(r, 9),
            Gehosppagi = SafeString(r, 10),
            Gedepacodi = SafeString(r, 11),
            Gemunicodi = SafeString(r, 12),
            Gehosprefa = SafeString(r, 13),
            Gehospreec = SafeString(r, 14),
            Geresudian = SafeString(r, 15),
            Gemensage = SafeString(r, 16),
            Gehospdigi = SafeString(r, 17),
            Gehospindi = SafeString(r, 18),
            Gehospext = SafeString(r, 19),
            Gemoduacti = SafeInt(r, 20),
            Gehoenviro = SafeInt(r, 21),
            Gehosetpru = SafeString(r, 22),
            Gehocurren = SafeString(r, 23),
            Gehocertif = SafeString(r, 24),
            Gehosubtyp = SafeInt(r, 25),
            Faferecodi = SafeString(r, 26),
            Gehofaeles = SafeInt(r, 27),
            Gehomailsi = SafeString(r, 28),
            Fafetifcod = SafeString(r, 29),
            Gehostyper = SafeInt(r, 30),
            Gehosrerut = SafeString(r, 31),
            Gehosquaid = SafeString(r, 32),
            Gehosquaes = SafeInt(r, 33),
            Gehosproxy = SafeString(r, 34),
            Gehoslogid = SafeString(r, 35),
            Gehosimgid = SafeString(r, 36),
            Gehocerdia = SafeString(r, 37),
            Fafetpdcod = SafeString(r, 38),
        };
    }

    private static string SelectColumns() => @"
        Gehospcodi,  Gehospnomb,  Gehosptiid,  Gehospnit,   Gehospdire,
        Gehosptele,  Gehospreso,  Gehospisgss, Gehosprele,  Gehospemai,
        Gehosppagi,  Gedepacodi,  Gemunicodi,  Gehosprefa,  Gehospreec,
        Geresudian,  Gemensage,   Gehospdigi,  Gehospindi,  Gehospext,
        Gemoduacti,  Gehoenviro,  Gehosetpru,  Gehocurren,  Gehocertif,
        Gehosubtyp,  Faferecodi,  Gehofaeles,  Gehomailsi,  Fafetifcod,
        Gehostyper,  Gehosrerut,  Gehosquaid,  Gehosquaes,  Gehosproxy,
        Gehoslogid,  Gehosimgid,  Gehocerdia,  Fafetpdcod";
}