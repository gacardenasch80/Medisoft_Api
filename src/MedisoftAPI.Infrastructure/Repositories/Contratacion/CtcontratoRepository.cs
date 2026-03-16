using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities.Contratacion;
using MedisoftAPI.Domain.Interfaces.Contratacion;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Contratacion;

/// <summary>
/// Repositorio de Ctcontrato — tabla: Ctcontrato
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Las fechas Date usan CTOD('MM/dd/yyyy') — función nativa VFP.
///    Los campos DateTime se pasan directamente como parámetro ?.
/// </summary>
public class CtcontratoRepository : ICtcontratoRepository
{
    private readonly string _conn;

    public CtcontratoRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Con")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Con' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Ctcontrato> Items, int Total)> GetAllAsync(CtcontratoFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Ctcontrato {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Ctcontrato
                {where}
                ORDER BY CTCONTCODI";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Ctcontrato
                {where}
                AND    CTCONTCODI NOT IN (
                           SELECT TOP {offset} CTCONTCODI
                           FROM   Ctcontrato
                           {where}
                           ORDER BY CTCONTCODI
                       )
                ORDER BY CTCONTCODI";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Ctcontrato?> GetByCodeAsync(string ctcontcodi)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Ctcontrato
            WHERE  ALLTRIM(CTCONTCODI) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([ctcontcodi.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Ctcontrato> CreateAsync(Ctcontrato e)
    {
        const string sql = @"
            INSERT INTO Ctcontrato (
                CTCONTCODI, CTCONTNUME, CTADMICODI, CTREGICODI, CTPLBECODI,
                CTMANUCODI, CTCONTLEGA, CTCONTFELE, CTCONTDESD, CTCONTHAST,
                CTCONTVALO, CTCONTESTA, CTTICOCODI, CTCONTASIG, CTCONTFACT,
                FAOBSHORA,  CTCTAACRE,  CTCITXDIA,  CTCONTCONS,
                CTCONTFECR, CTCONTUSCR, CTCONTFEED, CTCONTUSED,
                CTCONTVADE, CTCONTPYP,  CTMANUPROD,
                PUCDEBRADI, FAPRESUDEB, FAPRESUCRE, PUCDEBCOPA, PREDEBCOPA,
                PRECRECOPA, PRCRREVACT, PRDEREVAFA, PRCRREVAFA, PRDEREVARE,
                PRCRREVARE, PUCDEVA360, PRDEREVEFA, PRCRREVEFA, PRDEREVERE,
                PRCRREVERE, PUCDEGLOSA, PUCCRGLOSA, PUCDEGLOVA, PUCCRGLOVA,
                PCRGLVA360, PUCBANCO,   CTCONTCANFA,
                PUCDEBCOUR, PUCDEBCOHO, CTCONTFOPA, FAFEMPCODI,
                CTCONTEMSU, CTCONTSOAT, CTCATACODI
            ) VALUES (
                ?,?,?,?,?, ?,?,?,?,?,
                ?,?,?,?,?, ?,?,?,?,
                ?,?,?,?,
                ?,?,?,
                ?,?,?,?,?, ?,?,?,?,?,
                ?,?,?,?,?, ?,?,?,?,?,
                ?,?,?, ?,?,?,?,
                ?,?,?
            )";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Ctcontrato> UpdateAsync(Ctcontrato e)
    {
        const string sql = @"
            UPDATE Ctcontrato SET
                CTCONTNUME=?, CTADMICODI=?, CTREGICODI=?, CTPLBECODI=?,
                CTMANUCODI=?, CTCONTLEGA=?, CTCONTFELE=?, CTCONTDESD=?,
                CTCONTHAST=?, CTCONTVALO=?, CTCONTESTA=?, CTTICOCODI=?,
                CTCONTASIG=?, CTCONTFACT=?, FAOBSHORA=?,  CTCTAACRE=?,
                CTCITXDIA=?,  CTCONTCONS=?,
                CTCONTFECR=?, CTCONTUSCR=?, CTCONTFEED=?, CTCONTUSED=?,
                CTCONTVADE=?, CTCONTPYP=?,  CTMANUPROD=?,
                PUCDEBRADI=?, FAPRESUDEB=?, FAPRESUCRE=?, PUCDEBCOPA=?,
                PREDEBCOPA=?, PRECRECOPA=?, PRCRREVACT=?, PRDEREVAFA=?,
                PRCRREVAFA=?, PRDEREVARE=?, PRCRREVARE=?, PUCDEVA360=?,
                PRDEREVEFA=?, PRCRREVEFA=?, PRDEREVERE=?, PRCRREVERE=?,
                PUCDEGLOSA=?, PUCCRGLOSA=?, PUCDEGLOVA=?, PUCCRGLOVA=?,
                PCRGLVA360=?, PUCBANCO=?,   CTCONTCANFA=?,
                PUCDEBCOUR=?, PUCDEBCOHO=?, CTCONTFOPA=?, FAFEMPCODI=?,
                CTCONTEMSU=?, CTCONTSOAT=?, CTCATACODI=?
            WHERE ALLTRIM(CTCONTCODI) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string ctcontcodi)
    {
        const string sql = "DELETE FROM Ctcontrato WHERE ALLTRIM(CTCONTCODI) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([ctcontcodi.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    /// <summary>
    /// Fechas rango usan CTOD() — función nativa VFP para campos Date.
    /// Se inyectan formateadas MM/dd/yyyy (son DateTime del sistema, no input libre).
    /// </summary>
    private static (string Where, object[] Values) BuildWhere(CtcontratoFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.CTCONTCODI))
        {
            sb.Append(" AND ALLTRIM(CTCONTCODI) = ?");
            values.Add(f.CTCONTCODI.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.CTCONTNUME))
        {
            sb.Append(" AND ALLTRIM(CTCONTNUME) = ?");
            values.Add(f.CTCONTNUME.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.CTADMICODI))
        {
            sb.Append(" AND ALLTRIM(CTADMICODI) = ?");
            values.Add(f.CTADMICODI.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.CTREGICODI))
        {
            sb.Append(" AND ALLTRIM(CTREGICODI) = ?");
            values.Add(f.CTREGICODI.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.CTCONTESTA))
        {
            sb.Append(" AND ALLTRIM(CTCONTESTA) = ?");
            values.Add(f.CTCONTESTA.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.CTTICOCODI))
        {
            sb.Append(" AND ALLTRIM(CTTICOCODI) = ?");
            values.Add(f.CTTICOCODI.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.FAFEMPCODI))
        {
            sb.Append(" AND ALLTRIM(FAFEMPCODI) = ?");
            values.Add(f.FAFEMPCODI.Trim());
        }
        // Fechas Date con CTOD() — no usan ? sino formato directo VFP
        if (f.FechaDesde.HasValue)
            sb.Append($" AND CTCONTDESD >= CTOD('{f.FechaDesde.Value:MM/dd/yyyy}')");
        if (f.FechaHasta.HasValue)
            sb.Append($" AND CTCONTHAST <= CTOD('{f.FechaHasta.Value:MM/dd/yyyy}')");

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Ctcontrato e) =>
    [
        e.CTCONTCODI  ?? (object)DBNull.Value,
        e.CTCONTNUME  ?? (object)DBNull.Value,
        e.CTADMICODI  ?? (object)DBNull.Value,
        e.CTREGICODI  ?? (object)DBNull.Value,
        e.CTPLBECODI  ?? (object)DBNull.Value,
        e.CTMANUCODI  ?? (object)DBNull.Value,
        e.CTCONTLEGA  ?? (object)DBNull.Value,
        e.CTCONTFELE.HasValue  ? e.CTCONTFELE.Value  : (object)DBNull.Value,
        e.CTCONTDESD.HasValue  ? e.CTCONTDESD.Value  : (object)DBNull.Value,
        e.CTCONTHAST.HasValue  ? e.CTCONTHAST.Value  : (object)DBNull.Value,
        e.CTCONTVALO.HasValue  ? e.CTCONTVALO.Value  : (object)DBNull.Value,
        e.CTCONTESTA  ?? (object)DBNull.Value,
        e.CTTICOCODI  ?? (object)DBNull.Value,
        e.CTCONTASIG.HasValue  ? e.CTCONTASIG.Value  : (object)DBNull.Value,
        e.CTCONTFACT.HasValue  ? e.CTCONTFACT.Value  : (object)DBNull.Value,
        e.FAOBSHORA.HasValue   ? e.FAOBSHORA.Value   : (object)DBNull.Value,
        e.CTCTAACRE   ?? (object)DBNull.Value,
        e.CTCITXDIA.HasValue   ? e.CTCITXDIA.Value   : (object)DBNull.Value,
        e.CTCONTCONS.HasValue  ? e.CTCONTCONS.Value  : (object)DBNull.Value,
        e.CTCONTFECR.HasValue  ? e.CTCONTFECR.Value  : (object)DBNull.Value,
        e.CTCONTUSCR  ?? (object)DBNull.Value,
        e.CTCONTFEED.HasValue  ? e.CTCONTFEED.Value  : (object)DBNull.Value,
        e.CTCONTUSED  ?? (object)DBNull.Value,
        e.CTCONTVADE  ?? (object)DBNull.Value,
        e.CTCONTPYP.HasValue   ? e.CTCONTPYP.Value   : (object)DBNull.Value,
        e.CTMANUPROD  ?? (object)DBNull.Value,
        e.PUCDEBRADI  ?? (object)DBNull.Value,
        e.FAPRESUDEB  ?? (object)DBNull.Value,
        e.FAPRESUCRE  ?? (object)DBNull.Value,
        e.PUCDEBCOPA  ?? (object)DBNull.Value,
        e.PREDEBCOPA  ?? (object)DBNull.Value,
        e.PRECRECOPA  ?? (object)DBNull.Value,
        e.PRCRREVACT  ?? (object)DBNull.Value,
        e.PRDEREVAFA  ?? (object)DBNull.Value,
        e.PRCRREVAFA  ?? (object)DBNull.Value,
        e.PRDEREVARE  ?? (object)DBNull.Value,
        e.PRCRREVARE  ?? (object)DBNull.Value,
        e.PUCDEVA360  ?? (object)DBNull.Value,
        e.PRDEREVEFA  ?? (object)DBNull.Value,
        e.PRCRREVEFA  ?? (object)DBNull.Value,
        e.PRDEREVERE  ?? (object)DBNull.Value,
        e.PRCRREVERE  ?? (object)DBNull.Value,
        e.PUCDEGLOSA  ?? (object)DBNull.Value,
        e.PUCCRGLOSA  ?? (object)DBNull.Value,
        e.PUCDEGLOVA  ?? (object)DBNull.Value,
        e.PUCCRGLOVA  ?? (object)DBNull.Value,
        e.PCRGLVA360  ?? (object)DBNull.Value,
        e.PUCBANCO    ?? (object)DBNull.Value,
        e.CTCONTCANFA.HasValue ? e.CTCONTCANFA.Value : (object)DBNull.Value,
        e.PUCDEBCOUR  ?? (object)DBNull.Value,
        e.PUCDEBCOHO  ?? (object)DBNull.Value,
        e.CTCONTFOPA.HasValue  ? e.CTCONTFOPA.Value  : (object)DBNull.Value,
        e.FAFEMPCODI  ?? (object)DBNull.Value,
        e.CTCONTEMSU  ?? (object)DBNull.Value,
        e.CTCONTSOAT.HasValue  ? e.CTCONTSOAT.Value  : (object)DBNull.Value,
        e.CTCATACODI  ?? (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Ctcontrato e) =>
    [
        // SET (sin CTCONTCODI — es la clave, no se actualiza)
        e.CTCONTNUME  ?? (object)DBNull.Value,
        e.CTADMICODI  ?? (object)DBNull.Value,
        e.CTREGICODI  ?? (object)DBNull.Value,
        e.CTPLBECODI  ?? (object)DBNull.Value,
        e.CTMANUCODI  ?? (object)DBNull.Value,
        e.CTCONTLEGA  ?? (object)DBNull.Value,
        e.CTCONTFELE.HasValue  ? e.CTCONTFELE.Value  : (object)DBNull.Value,
        e.CTCONTDESD.HasValue  ? e.CTCONTDESD.Value  : (object)DBNull.Value,
        e.CTCONTHAST.HasValue  ? e.CTCONTHAST.Value  : (object)DBNull.Value,
        e.CTCONTVALO.HasValue  ? e.CTCONTVALO.Value  : (object)DBNull.Value,
        e.CTCONTESTA  ?? (object)DBNull.Value,
        e.CTTICOCODI  ?? (object)DBNull.Value,
        e.CTCONTASIG.HasValue  ? e.CTCONTASIG.Value  : (object)DBNull.Value,
        e.CTCONTFACT.HasValue  ? e.CTCONTFACT.Value  : (object)DBNull.Value,
        e.FAOBSHORA.HasValue   ? e.FAOBSHORA.Value   : (object)DBNull.Value,
        e.CTCTAACRE   ?? (object)DBNull.Value,
        e.CTCITXDIA.HasValue   ? e.CTCITXDIA.Value   : (object)DBNull.Value,
        e.CTCONTCONS.HasValue  ? e.CTCONTCONS.Value  : (object)DBNull.Value,
        e.CTCONTFECR.HasValue  ? e.CTCONTFECR.Value  : (object)DBNull.Value,
        e.CTCONTUSCR  ?? (object)DBNull.Value,
        e.CTCONTFEED.HasValue  ? e.CTCONTFEED.Value  : (object)DBNull.Value,
        e.CTCONTUSED  ?? (object)DBNull.Value,
        e.CTCONTVADE  ?? (object)DBNull.Value,
        e.CTCONTPYP.HasValue   ? e.CTCONTPYP.Value   : (object)DBNull.Value,
        e.CTMANUPROD  ?? (object)DBNull.Value,
        e.PUCDEBRADI  ?? (object)DBNull.Value,
        e.FAPRESUDEB  ?? (object)DBNull.Value,
        e.FAPRESUCRE  ?? (object)DBNull.Value,
        e.PUCDEBCOPA  ?? (object)DBNull.Value,
        e.PREDEBCOPA  ?? (object)DBNull.Value,
        e.PRECRECOPA  ?? (object)DBNull.Value,
        e.PRCRREVACT  ?? (object)DBNull.Value,
        e.PRDEREVAFA  ?? (object)DBNull.Value,
        e.PRCRREVAFA  ?? (object)DBNull.Value,
        e.PRDEREVARE  ?? (object)DBNull.Value,
        e.PRCRREVARE  ?? (object)DBNull.Value,
        e.PUCDEVA360  ?? (object)DBNull.Value,
        e.PRDEREVEFA  ?? (object)DBNull.Value,
        e.PRCRREVEFA  ?? (object)DBNull.Value,
        e.PRDEREVERE  ?? (object)DBNull.Value,
        e.PRCRREVERE  ?? (object)DBNull.Value,
        e.PUCDEGLOSA  ?? (object)DBNull.Value,
        e.PUCCRGLOSA  ?? (object)DBNull.Value,
        e.PUCDEGLOVA  ?? (object)DBNull.Value,
        e.PUCCRGLOVA  ?? (object)DBNull.Value,
        e.PCRGLVA360  ?? (object)DBNull.Value,
        e.PUCBANCO    ?? (object)DBNull.Value,
        e.CTCONTCANFA.HasValue ? e.CTCONTCANFA.Value : (object)DBNull.Value,
        e.PUCDEBCOUR  ?? (object)DBNull.Value,
        e.PUCDEBCOHO  ?? (object)DBNull.Value,
        e.CTCONTFOPA.HasValue  ? e.CTCONTFOPA.Value  : (object)DBNull.Value,
        e.FAFEMPCODI  ?? (object)DBNull.Value,
        e.CTCONTEMSU  ?? (object)DBNull.Value,
        e.CTCONTSOAT.HasValue  ? e.CTCONTSOAT.Value  : (object)DBNull.Value,
        e.CTCATACODI  ?? (object)DBNull.Value,
        // WHERE
        e.CTCONTCODI  ?? (object)DBNull.Value,
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

    private static async Task<List<Ctcontrato>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Ctcontrato>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    /// <summary>
    /// Lee un campo Numeric de VFP de forma segura usando IsDBNull por ordinal.
    /// El proveedor VFP/OleDb lanza InvalidOperationException al acceder con ["campo"]
    /// en campos Numeric vacíos, incluso antes de la comparación con DBNull.
    /// La única solución es usar IsDBNull(ordinal) + GetDouble(ordinal).
    /// </summary>
    private static double? SafeDouble(System.Data.IDataRecord r, int ordinal)
    {
        try { return r.IsDBNull(ordinal) ? null : r.GetDouble(ordinal); }
        catch { return null; }
    }

    private static int? SafeInt(System.Data.IDataRecord r, int ordinal)
    {
        try { return r.IsDBNull(ordinal) ? null : (int?)Convert.ToInt32(r.GetValue(ordinal)); }
        catch { return null; }
    }

    private static DateTime? SafeDate(System.Data.IDataRecord r, int ordinal)
    {
        try { return r.IsDBNull(ordinal) ? null : (DateTime?)Convert.ToDateTime(r.GetValue(ordinal)); }
        catch { return null; }
    }

    private static string? SafeString(System.Data.IDataRecord r, int ordinal)
    {
        try { return r.IsDBNull(ordinal) ? null : r.GetString(ordinal); }
        catch { return null; }
    }

    private static Ctcontrato MapRow(System.Data.IDataRecord r)
    {
        // Índices según el orden exacto de SelectColumns()
        // 0:CTCONTCODI  1:CTCONTNUME  2:CTADMICODI  3:CTREGICODI  4:CTPLBECODI
        // 5:CTMANUCODI  6:CTCONTLEGA  7:CTCONTFELE  8:CTCONTDESD  9:CTCONTHAST
        // 10:CTCONTVALO 11:CTCONTESTA 12:CTTICOCODI 13:CTCONTASIG 14:CTCONTFACT
        // 15:FAOBSHORA  16:CTCTAACRE  17:CTCITXDIA  18:CTCONTCONS
        // 19:CTCONTFECR 20:CTCONTUSCR 21:CTCONTFEED 22:CTCONTUSED
        // 23:CTCONTVADE 24:CTCONTPYP  25:CTMANUPROD
        // 26:PUCDEBRADI 27:FAPRESUDEB 28:FAPRESUCRE 29:PUCDEBCOPA 30:PREDEBCOPA
        // 31:PRECRECOPA 32:PRCRREVACT 33:PRDEREVAFA 34:PRCRREVAFA 35:PRDEREVARE
        // 36:PRCRREVARE 37:PUCDEVA360 38:PRDEREVEFA 39:PRCRREVEFA 40:PRDEREVERE
        // 41:PRCRREVERE 42:PUCDEGLOSA 43:PUCCRGLOSA 44:PUCDEGLOVA 45:PUCCRGLOVA
        // 46:PCRGLVA360 47:PUCBANCO   48:CTCONTCANFA
        // 49:PUCDEBCOUR 50:PUCDEBCOHO 51:CTCONTFOPA 52:FAFEMPCODI
        // 53:CTCONTEMSU 54:CTCONTSOAT 55:CTCATACODI
        return new Ctcontrato
        {
            CTCONTCODI = SafeString(r, 0),
            CTCONTNUME = SafeString(r, 1),
            CTADMICODI = SafeString(r, 2),
            CTREGICODI = SafeString(r, 3),
            CTPLBECODI = SafeString(r, 4),
            CTMANUCODI = SafeString(r, 5),
            CTCONTLEGA = SafeString(r, 6),
            CTCONTFELE = SafeDate(r, 7),
            CTCONTDESD = SafeDate(r, 8),
            CTCONTHAST = SafeDate(r, 9),
            CTCONTVALO = SafeDouble(r, 10),
            CTCONTESTA = SafeString(r, 11),
            CTTICOCODI = SafeString(r, 12),
            CTCONTASIG = SafeDouble(r, 13),
            CTCONTFACT = SafeDouble(r, 14),
            FAOBSHORA = SafeInt(r, 15),
            CTCTAACRE = SafeString(r, 16),
            CTCITXDIA = SafeInt(r, 17),
            CTCONTCONS = SafeInt(r, 18),
            CTCONTFECR = SafeDate(r, 19),
            CTCONTUSCR = SafeString(r, 20),
            CTCONTFEED = SafeDate(r, 21),
            CTCONTUSED = SafeString(r, 22),
            CTCONTVADE = SafeString(r, 23),
            CTCONTPYP = SafeInt(r, 24),
            CTMANUPROD = SafeString(r, 25),
            PUCDEBRADI = SafeString(r, 26),
            FAPRESUDEB = SafeString(r, 27),
            FAPRESUCRE = SafeString(r, 28),
            PUCDEBCOPA = SafeString(r, 29),
            PREDEBCOPA = SafeString(r, 30),
            PRECRECOPA = SafeString(r, 31),
            PRCRREVACT = SafeString(r, 32),
            PRDEREVAFA = SafeString(r, 33),
            PRCRREVAFA = SafeString(r, 34),
            PRDEREVARE = SafeString(r, 35),
            PRCRREVARE = SafeString(r, 36),
            PUCDEVA360 = SafeString(r, 37),
            PRDEREVEFA = SafeString(r, 38),
            PRCRREVEFA = SafeString(r, 39),
            PRDEREVERE = SafeString(r, 40),
            PRCRREVERE = SafeString(r, 41),
            PUCDEGLOSA = SafeString(r, 42),
            PUCCRGLOSA = SafeString(r, 43),
            PUCDEGLOVA = SafeString(r, 44),
            PUCCRGLOVA = SafeString(r, 45),
            PCRGLVA360 = SafeString(r, 46),
            PUCBANCO = SafeString(r, 47),
            CTCONTCANFA = SafeInt(r, 48),
            PUCDEBCOUR = SafeString(r, 49),
            PUCDEBCOHO = SafeString(r, 50),
            CTCONTFOPA = SafeInt(r, 51),
            FAFEMPCODI = SafeString(r, 52),
            CTCONTEMSU = SafeString(r, 53),
            CTCONTSOAT = SafeInt(r, 54),
            CTCATACODI = SafeString(r, 55),
        };
    }

    private static string SelectColumns() => @"
        CTCONTCODI, CTCONTNUME, CTADMICODI, CTREGICODI, CTPLBECODI,
        CTMANUCODI, CTCONTLEGA, CTCONTFELE, CTCONTDESD, CTCONTHAST,
        CTCONTVALO, CTCONTESTA, CTTICOCODI, CTCONTASIG, CTCONTFACT,
        FAOBSHORA,  CTCTAACRE,  CTCITXDIA,  CTCONTCONS,
        CTCONTFECR, CTCONTUSCR, CTCONTFEED, CTCONTUSED,
        CTCONTVADE, CTCONTPYP,  CTMANUPROD,
        PUCDEBRADI, FAPRESUDEB, FAPRESUCRE, PUCDEBCOPA, PREDEBCOPA,
        PRECRECOPA, PRCRREVACT, PRDEREVAFA, PRCRREVAFA, PRDEREVARE,
        PRCRREVARE, PUCDEVA360, PRDEREVEFA, PRCRREVEFA, PRDEREVERE,
        PRCRREVERE, PUCDEGLOSA, PUCCRGLOSA, PUCDEGLOVA, PUCCRGLOVA,
        PCRGLVA360, PUCBANCO,   CTCONTCANFA,
        PUCDEBCOUR, PUCDEBCOHO, CTCONTFOPA, FAFEMPCODI,
        CTCONTEMSU, CTCONTSOAT, CTCATACODI";
}