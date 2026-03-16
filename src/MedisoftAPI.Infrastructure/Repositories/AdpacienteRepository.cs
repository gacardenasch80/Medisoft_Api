using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories;

/// <summary>
/// Repositorio de Adpaciente — base de datos: FoxPro_Adm
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Cada OleDbCommand recibe una copia nueva de los parámetros
///    porque un OleDbParameter no puede pertenecer a dos colecciones.
/// </summary>
public class AdpacienteRepository : IAdpacienteRepository
{
    private readonly string _conn;

    public AdpacienteRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Adm")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Adm' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Adpaciente> Items, int Total)> GetAllAsync(AdpacienteFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        // ── Conteo ────────────────────────────────────────────────
        var sqlCount = $"SELECT COUNT(*) FROM Adpaciente {where}";
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
                FROM   Adpaciente
                {where}
                ORDER BY ADPACIIDEN";
            queryValues = paramValues;
        }
        else
        {
            // La subquery repite el mismo WHERE → duplicar los valores
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adpaciente
                {where}
                AND    ADPACIIDEN NOT IN (
                           SELECT TOP {offset} ADPACIIDEN
                           FROM   Adpaciente
                           {where}
                           ORDER BY ADPACIIDEN
                       )
                ORDER BY ADPACIIDEN";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Adpaciente?> GetByCodeAsync(string identificacion)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Adpaciente
            WHERE  ALLTRIM(ADPACIIDEN) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([identificacion.Trim()]));
        return items.FirstOrDefault();
    }

    public async Task<Adpaciente?> GetByCelularAsync(string adpacicelu)
    {
        var sql = $@"
        SELECT {SelectColumns()}
        FROM   Adpaciente
        WHERE  (ALLTRIM(adpacicelu) = ? OR ALLTRIM(ADPACITELE) = ?) ";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([adpacicelu.Trim(), adpacicelu.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Adpaciente> CreateAsync(Adpaciente e)
    {
        const string sql = @"
            INSERT INTO Adpaciente (
                ADTIIDCODI, ADPACIIDEN, ADTIIDCAFA, ADCAFAIDEN,
                ADPACIAPE1, ADPACIAPE2, ADPACINOM1, ADPACINOM2,
                ADPACIFENA, ADPACISEXO,
                GEBARRZONA, GEDEPACODI, GEMUNICODI, GEBARRCODI,
                GEDEPACOD1, GEMUNICOD1,
                CTREGICODI, ADPACIDIRE, ADPACITELE, CTNIVECODI,
                ADPACIHIST, ADFEINSGSS, ADPACIFEIN,
                ADTIAFCODI, ADESTACODI,
                ADPACIPADR, ADPACIMADR,
                ADPACICELU, ADPACIMAIL, ADPACIHECL,
                GECODIESC,  GECODIPECT, GECODIOCGG, GECODIOSGP,
                GECODIOCSG, GECODIOCGP,
                CODIGOPOB,  CODIGONUT,
                ADPACIOBSE, ADPACIIDRE,
                FAFERECODI, FAFETIFCOD,
                ADPACTYPER, ADPACRERUT, ADCLDICODI
            ) VALUES (
                ?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?,?,?,?,?,
                ?,?,?,?,?,?
            )";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Adpaciente> UpdateAsync(Adpaciente e)
    {
        const string sql = @"
            UPDATE Adpaciente SET
                ADTIIDCODI=?, ADTIIDCAFA=?, ADCAFAIDEN=?,
                ADPACIAPE1=?, ADPACIAPE2=?, ADPACINOM1=?, ADPACINOM2=?,
                ADPACIFENA=?, ADPACISEXO=?,
                GEBARRZONA=?, GEDEPACODI=?, GEMUNICODI=?, GEBARRCODI=?,
                GEDEPACOD1=?, GEMUNICOD1=?,
                CTREGICODI=?, ADPACIDIRE=?, ADPACITELE=?, CTNIVECODI=?,
                ADPACIHIST=?, ADFEINSGSS=?, ADPACIFEIN=?,
                ADTIAFCODI=?, ADESTACODI=?,
                ADPACIPADR=?, ADPACIMADR=?,
                ADPACICELU=?, ADPACIMAIL=?, ADPACIHECL=?,
                GECODIESC=?,  GECODIPECT=?, GECODIOCGG=?, GECODIOSGP=?,
                GECODIOCSG=?, GECODIOCGP=?,
                CODIGOPOB=?,  CODIGONUT=?,
                ADPACIOBSE=?, ADPACIIDRE=?,
                FAFERECODI=?, FAFETIFCOD=?,
                ADPACTYPER=?, ADPACRERUT=?, ADCLDICODI=?
            WHERE ALLTRIM(ADPACIIDEN) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string identificacion)
    {
        const string sql = "DELETE FROM Adpaciente WHERE ALLTRIM(ADPACIIDEN) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([identificacion.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    /// <summary>
    /// Retorna el WHERE con ? y un array de valores en el mismo orden.
    /// Los valores se convierten a OleDbParameter SOLO al momento de
    /// crear el comando (MakeParams), nunca se reutiliza el mismo objeto.
    /// </summary>
    private static (string Where, object[] Values) BuildWhere(AdpacienteFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.ADPACIIDEN))
        {
            sb.Append(" AND ALLTRIM(ADPACIIDEN) = ?");
            values.Add(f.ADPACIIDEN.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.ADPACIAPE1))
        {
            sb.Append(" AND UPPER(ALLTRIM(ADPACIAPE1)) LIKE ?");
            values.Add($"%{f.ADPACIAPE1.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.ADPACIAPE2))
        {
            sb.Append(" AND UPPER(ALLTRIM(ADPACIAPE2)) LIKE ?");
            values.Add($"%{f.ADPACIAPE2.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.ADPACINOM1))
        {
            sb.Append(" AND UPPER(ALLTRIM(ADPACINOM1)) LIKE ?");
            values.Add($"%{f.ADPACINOM1.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.ADPACINOM2))
        {
            sb.Append(" AND UPPER(ALLTRIM(ADPACINOM2)) LIKE ?");
            values.Add($"%{f.ADPACINOM2.ToUpper().Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(f.ADPACICELU))
        {
            sb.Append(" AND ALLTRIM(ADPACICELU) = ?");
            values.Add(f.ADPACICELU.Trim());
        }
        //if (!string.IsNullOrWhiteSpace(f.ADESTACODI))
        //{
        //    sb.Append(" AND ALLTRIM(ADESTACODI) = ?");
        //    values.Add(f.ADESTACODI.Trim());
        //}
        if (!string.IsNullOrWhiteSpace(f.ADTIIDCODI))
        {
            sb.Append(" AND ALLTRIM(ADTIIDCODI) = ?");
            values.Add(f.ADTIIDCODI.Trim());
        }
        //if (!string.IsNullOrWhiteSpace(f.CTREGICODI))
        //{
        //    sb.Append(" AND ALLTRIM(CTREGICODI) = ?");
        //    values.Add(f.CTREGICODI.Trim());
        //}
        //if (!string.IsNullOrWhiteSpace(f.ADPACISEXO))
        //{
        //    sb.Append(" AND ALLTRIM(ADPACISEXO) = ?");
        //    values.Add(f.ADPACISEXO.Trim());
        //}
        //if (!string.IsNullOrWhiteSpace(f.GEDEPACODI))
        //{
        //    sb.Append(" AND ALLTRIM(GEDEPACODI) = ?");
        //    values.Add(f.GEDEPACODI.Trim());
        //}
        //if (!string.IsNullOrWhiteSpace(f.GEMUNICODI))
        //{
        //    sb.Append(" AND ALLTRIM(GEMUNICODI) = ?");
        //    values.Add(f.GEMUNICODI.Trim());
        //}
        //if (f.ADPACIFENA.HasValue)
        //{
        //    sb.Append(" AND ADPACIFENA = ?");
        //    values.Add(f.ADPACIFENA.Value.Date);
        //}

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Adpaciente e) =>
    [
        e.ADTIIDCODI ?? (object)DBNull.Value,
        e.ADPACIIDEN ?? (object)DBNull.Value,
        e.ADTIIDCAFA ?? (object)DBNull.Value,
        e.ADCAFAIDEN ?? (object)DBNull.Value,
        e.ADPACIAPE1 ?? (object)DBNull.Value,
        e.ADPACIAPE2 ?? (object)DBNull.Value,
        e.ADPACINOM1 ?? (object)DBNull.Value,
        e.ADPACINOM2 ?? (object)DBNull.Value,
        e.ADPACIFENA.HasValue ? e.ADPACIFENA.Value : (object)DBNull.Value,
        e.ADPACISEXO ?? (object)DBNull.Value,
        e.GEBARRZONA ?? (object)DBNull.Value,
        e.GEDEPACODI ?? (object)DBNull.Value,
        e.GEMUNICODI ?? (object)DBNull.Value,
        e.GEBARRCODI ?? (object)DBNull.Value,
        e.GEDEPACOD1 ?? (object)DBNull.Value,
        e.GEMUNICOD1 ?? (object)DBNull.Value,
        e.CTREGICODI ?? (object)DBNull.Value,
        e.ADPACIDIRE ?? (object)DBNull.Value,
        e.ADPACITELE ?? (object)DBNull.Value,
        e.CTNIVECODI ?? (object)DBNull.Value,
        e.ADPACIHIST ?? (object)DBNull.Value,
        e.ADFEINSGSS.HasValue ? e.ADFEINSGSS.Value : (object)DBNull.Value,
        e.ADPACIFEIN.HasValue ? e.ADPACIFEIN.Value : (object)DBNull.Value,
        e.ADTIAFCODI ?? (object)DBNull.Value,
        e.ADESTACODI ?? (object)DBNull.Value,
        e.ADPACIPADR ?? (object)DBNull.Value,
        e.ADPACIMADR ?? (object)DBNull.Value,
        e.ADPACICELU ?? (object)DBNull.Value,
        e.ADPACIMAIL ?? (object)DBNull.Value,
        e.ADPACIHECL ?? (object)DBNull.Value,
        e.GECODIESC  ?? (object)DBNull.Value,
        e.GECODIPECT ?? (object)DBNull.Value,
        e.GECODIOCGG ?? (object)DBNull.Value,
        e.GECODIOSGP ?? (object)DBNull.Value,
        e.GECODIOCSG ?? (object)DBNull.Value,
        e.GECODIOCGP ?? (object)DBNull.Value,
        e.CODIGOPOB  ?? (object)DBNull.Value,
        e.CODIGONUT  ?? (object)DBNull.Value,
        e.ADPACIOBSE ?? (object)DBNull.Value,
        e.ADPACIIDRE ?? (object)DBNull.Value,
        e.FAFERECODI ?? (object)DBNull.Value,
        e.FAFETIFCOD ?? (object)DBNull.Value,
        e.ADPACTYPER.HasValue ? e.ADPACTYPER.Value : (object)DBNull.Value,
        e.ADPACRERUT ?? (object)DBNull.Value,
        e.ADCLDICODI ?? (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Adpaciente e) =>
    [
        // SET (sin ADPACIIDEN)
        e.ADTIIDCODI ?? (object)DBNull.Value,
        e.ADTIIDCAFA ?? (object)DBNull.Value,
        e.ADCAFAIDEN ?? (object)DBNull.Value,
        e.ADPACIAPE1 ?? (object)DBNull.Value,
        e.ADPACIAPE2 ?? (object)DBNull.Value,
        e.ADPACINOM1 ?? (object)DBNull.Value,
        e.ADPACINOM2 ?? (object)DBNull.Value,
        e.ADPACIFENA.HasValue ? e.ADPACIFENA.Value : (object)DBNull.Value,
        e.ADPACISEXO ?? (object)DBNull.Value,
        e.GEBARRZONA ?? (object)DBNull.Value,
        e.GEDEPACODI ?? (object)DBNull.Value,
        e.GEMUNICODI ?? (object)DBNull.Value,
        e.GEBARRCODI ?? (object)DBNull.Value,
        e.GEDEPACOD1 ?? (object)DBNull.Value,
        e.GEMUNICOD1 ?? (object)DBNull.Value,
        e.CTREGICODI ?? (object)DBNull.Value,
        e.ADPACIDIRE ?? (object)DBNull.Value,
        e.ADPACITELE ?? (object)DBNull.Value,
        e.CTNIVECODI ?? (object)DBNull.Value,
        e.ADPACIHIST ?? (object)DBNull.Value,
        e.ADFEINSGSS.HasValue ? e.ADFEINSGSS.Value : (object)DBNull.Value,
        e.ADPACIFEIN.HasValue ? e.ADPACIFEIN.Value : (object)DBNull.Value,
        e.ADTIAFCODI ?? (object)DBNull.Value,
        e.ADESTACODI ?? (object)DBNull.Value,
        e.ADPACIPADR ?? (object)DBNull.Value,
        e.ADPACIMADR ?? (object)DBNull.Value,
        e.ADPACICELU ?? (object)DBNull.Value,
        e.ADPACIMAIL ?? (object)DBNull.Value,
        e.ADPACIHECL ?? (object)DBNull.Value,
        e.GECODIESC  ?? (object)DBNull.Value,
        e.GECODIPECT ?? (object)DBNull.Value,
        e.GECODIOCGG ?? (object)DBNull.Value,
        e.GECODIOSGP ?? (object)DBNull.Value,
        e.GECODIOCSG ?? (object)DBNull.Value,
        e.GECODIOCGP ?? (object)DBNull.Value,
        e.CODIGOPOB  ?? (object)DBNull.Value,
        e.CODIGONUT  ?? (object)DBNull.Value,
        e.ADPACIOBSE ?? (object)DBNull.Value,
        e.ADPACIIDRE ?? (object)DBNull.Value,
        e.FAFERECODI ?? (object)DBNull.Value,
        e.FAFETIFCOD ?? (object)DBNull.Value,
        e.ADPACTYPER.HasValue ? e.ADPACTYPER.Value : (object)DBNull.Value,
        e.ADPACRERUT ?? (object)DBNull.Value,
        e.ADCLDICODI ?? (object)DBNull.Value,
        // WHERE
        e.ADPACIIDEN ?? (object)DBNull.Value,
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

    private static async Task<List<Adpaciente>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Adpaciente>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    private static Adpaciente MapRow(System.Data.IDataRecord r) => new()
    {
        ADTIIDCODI = r["ADTIIDCODI"] as string,
        ADPACIIDEN = r["ADPACIIDEN"] as string,
        ADTIIDCAFA = r["ADTIIDCAFA"] as string,
        ADCAFAIDEN = r["ADCAFAIDEN"] as string,
        ADPACIAPE1 = r["ADPACIAPE1"] as string,
        ADPACIAPE2 = r["ADPACIAPE2"] as string,
        ADPACINOM1 = r["ADPACINOM1"] as string,
        ADPACINOM2 = r["ADPACINOM2"] as string,
        ADPACIFENA = r["ADPACIFENA"] == DBNull.Value ? null : Convert.ToDateTime(r["ADPACIFENA"]),
        ADPACISEXO = r["ADPACISEXO"] as string,
        GEBARRZONA = r["GEBARRZONA"] as string,
        GEDEPACODI = r["GEDEPACODI"] as string,
        GEMUNICODI = r["GEMUNICODI"] as string,
        GEBARRCODI = r["GEBARRCODI"] as string,
        GEDEPACOD1 = r["GEDEPACOD1"] as string,
        GEMUNICOD1 = r["GEMUNICOD1"] as string,
        CTREGICODI = r["CTREGICODI"] as string,
        ADPACIDIRE = r["ADPACIDIRE"] as string,
        ADPACITELE = r["ADPACITELE"] as string,
        CTNIVECODI = r["CTNIVECODI"] as string,
        ADPACIHIST = r["ADPACIHIST"] as string,
        ADFEINSGSS = r["ADFEINSGSS"] == DBNull.Value ? null : Convert.ToDateTime(r["ADFEINSGSS"]),
        ADPACIFEIN = r["ADPACIFEIN"] == DBNull.Value ? null : Convert.ToDateTime(r["ADPACIFEIN"]),
        ADTIAFCODI = r["ADTIAFCODI"] as string,
        ADESTACODI = r["ADESTACODI"] as string,
        ADPACIPADR = r["ADPACIPADR"] as string,
        ADPACIMADR = r["ADPACIMADR"] as string,
        ADPACICELU = r["ADPACICELU"] as string,
        ADPACIMAIL = r["ADPACIMAIL"] as string,
        ADPACIHECL = r["ADPACIHECL"] as string,
        GECODIESC = r["GECODIESC"] as string,
        GECODIPECT = r["GECODIPECT"] as string,
        GECODIOCGG = r["GECODIOCGG"] as string,
        GECODIOSGP = r["GECODIOSGP"] as string,
        GECODIOCSG = r["GECODIOCSG"] as string,
        GECODIOCGP = r["GECODIOCGP"] as string,
        CODIGOPOB = r["CODIGOPOB"] as string,
        CODIGONUT = r["CODIGONUT"] as string,
        ADPACIOBSE = r["ADPACIOBSE"] as string,
        ADPACIIDRE = r["ADPACIIDRE"] as string,
        FAFERECODI = r["FAFERECODI"] as string,
        FAFETIFCOD = r["FAFETIFCOD"] as string,
        ADPACTYPER = r["ADPACTYPER"] == DBNull.Value ? null : Convert.ToInt32(r["ADPACTYPER"]),
        ADPACRERUT = r["ADPACRERUT"] as string,
        ADCLDICODI = r["ADCLDICODI"] as string,
    };

    private static string SelectColumns() => @"
        ADTIIDCODI, ADPACIIDEN, ADTIIDCAFA, ADCAFAIDEN,
        ADPACIAPE1, ADPACIAPE2, ADPACINOM1, ADPACINOM2,
        ADPACIFENA, ADPACISEXO,
        GEBARRZONA, GEDEPACODI, GEMUNICODI, GEBARRCODI,
        GEDEPACOD1, GEMUNICOD1,
        CTREGICODI, ADPACIDIRE, ADPACITELE, CTNIVECODI,
        ADPACIHIST, ADFEINSGSS, ADPACIFEIN,
        ADTIAFCODI, ADESTACODI,
        ADPACIPADR, ADPACIMADR,
        ADPACICELU, ADPACIMAIL, ADPACIHECL,
        GECODIESC,  GECODIPECT, GECODIOCGG, GECODIOSGP,
        GECODIOCSG, GECODIOCGP,
        CODIGOPOB,  CODIGONUT,
        ADPACIOBSE, ADPACIIDRE,
        FAFERECODI, FAFETIFCOD,
        ADPACTYPER, ADPACRERUT, ADCLDICODI";
}
