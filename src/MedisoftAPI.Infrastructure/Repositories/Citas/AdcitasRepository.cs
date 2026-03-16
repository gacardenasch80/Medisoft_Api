using System.Data;
using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Interfaces.Citas;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Citas;

/// <summary>
/// Repositorio de Adcitas — tabla: Adcitas (citas.dbc)
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Las fechas Date usan CTOD('MM/dd/yyyy') — función nativa VFP.
///    Los campos Numeric se leen con IsDBNull(ordinal) para evitar
///    InvalidOperationException del proveedor VFP/OleDb.
/// </summary>
public class AdcitasRepository : IAdcitasRepository
{
    private readonly string _conn;

    public AdcitasRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_Cit")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Cit' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Adcitas> Items, int Total)> GetAllAsync(AdcitasFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Adcitas {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adcitas
                {where}
                ORDER BY Adcitacons";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Adcitas
                {where}
                AND    Adcitacons NOT IN (
                           SELECT TOP {offset} Adcitacons
                           FROM   Adcitas
                           {where}
                           ORDER BY Adcitacons
                       )
                ORDER BY Adcitacons";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Adcitas?> GetByCodeAsync(string adcitacons)
    {
        const string sql = @"
            SELECT {0}
            FROM   Adcitas
            WHERE  ALLTRIM(Adcitacons) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn,
            string.Format(sql, SelectColumns()),
            MakeParams([adcitacons.Trim()]));

        return items.FirstOrDefault();
    }

    // ── GET BY PACIENTE ───────────────────────────────────────────────────

    public async Task<IEnumerable<Adcitas>> GetByPacienteAsync(string adpaciiden)
    {
        string sql = $@"
            SELECT {SelectColumns()}
            FROM   Adcitas
            WHERE  ALLTRIM(Adpaciiden) = ?
            ORDER BY Adfechcita DESC";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        return await QueryAsync(conn, sql, MakeParams([adpaciiden.Trim()]));
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Adcitas> CreateAsync(Adcitas e)
    {
        // Fechas requeridas → CTOD(); fechas opcionales nulas → { / / } (fecha vacía VFP)
        string fechacita = $"CTOD('{e.Adfechcita:MM/dd/yyyy}')";
        string fechasoli = $"CTOD('{e.Fechasoli:MM/dd/yyyy}')";
        string fechapref = $"CTOD('{e.Fechprefpa:MM/dd/yyyy}')";
        string fechalleg = e.Fechalleg.HasValue
                          ? $"CTOD('{e.Fechalleg.Value:MM/dd/yyyy}')"
                          : "{ / / }";
        string adcitafean = e.Adcitafean.HasValue
                          ? $"CTOD('{e.Adcitafean.Value:MM/dd/yyyy}')"
                          : "{ / / }";

        string sql = $@"
        INSERT INTO Adcitas (
            Adcitacons, Geespecodi, Gemedicodi, Faservcodi, Adpaciiden,
            Adconscodi, Adfechcita, Adhorainic, Adhorafina, Adduraminu,
            Adconsdisp, Adcitaest,  Adanulcodi, Ctadmicodi, Ctcontcodi,
            Fechasoli,  Geusuacreo, Adingrcons, Faorsecons, Coconscons,
            Faprogcodi, Fechalleg,  Adcodanula, Fechprefpa, Adcitaande,
            Geusuacoan, Adcitafean, Adciticodi
        ) VALUES (
            ?,?,?,?,?,
            ?,{fechacita},?,?,?,
            ?,?,?,?,?,
            {fechasoli},?,?,?,?,
            ?,{fechalleg},?,{fechapref},?,
            ?,{adcitafean},?
        )";

        // Parámetros ? en el mismo orden que los ? del SQL (las fechas van inyectadas)
        var parms = new object[]
        {
        e.Adcitacons, e.Geespecodi, e.Gemedicodi, e.Faservcodi, e.Adpaciiden,
        e.Adconscodi, /* Adfechcita → CTOD */ e.Adhorainic, e.Adhorafina,
        e.Adduraminu.HasValue ? e.Adduraminu.Value : (object)DBNull.Value,
        e.Adconsdisp, e.Adcitaest, e.Adanulcodi, e.Ctadmicodi, e.Ctcontcodi,
        /* Fechasoli → CTOD */ e.Geusuacreo, e.Adingrcons, e.Faorsecons, e.Coconscons,
        e.Faprogcodi, /* Fechalleg → {//} */ e.Adcodanula,
        /* Fechprefpa → CTOD */ e.Adcitaande,
        e.Geusuacoan, /* Adcitafean → {//} */ e.Adciticodi,
        };

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(parms));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Adcitas> UpdateAsync(Adcitas e)
    {
        const string sql = @"
            UPDATE Adcitas SET
                Geespecodi = ?, Gemedicodi = ?, Faservcodi = ?, Adpaciiden = ?,
                Adconscodi = ?, Adfechcita = ?, Adhorainic = ?, Adhorafina = ?,
                Adduraminu = ?, Adconsdisp = ?, Adcitaest  = ?, Adanulcodi = ?,
                Ctadmicodi = ?, Ctcontcodi = ?, Fechasoli  = ?, Geusuacreo = ?,
                Adingrcons = ?, Faorsecons = ?, Coconscons = ?, Faprogcodi = ?,
                Fechalleg  = ?, Adcodanula = ?, Fechprefpa = ?, Adcitaande = ?,
                Geusuacoan = ?, Adcitafean = ?, Adciticodi = ?
            WHERE ALLTRIM(Adcitacons) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string adcitacons)
    {
        const string sql = "DELETE FROM Adcitas WHERE ALLTRIM(Adcitacons) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([adcitacons.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(AdcitasFilter f)
    {
        var sb = new StringBuilder("WHERE 1=1");
        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Adcitacons))
        {
            sb.Append(" AND ALLTRIM(Adcitacons) = ?");
            values.Add(f.Adcitacons.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Geespecodi))
        {
            sb.Append(" AND ALLTRIM(Geespecodi) = ?");
            values.Add(f.Geespecodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Gemedicodi))
        {
            sb.Append(" AND ALLTRIM(Gemedicodi) = ?");
            values.Add(f.Gemedicodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Faservcodi))
        {
            sb.Append(" AND ALLTRIM(Faservcodi) = ?");
            values.Add(f.Faservcodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Adpaciiden))
        {
            sb.Append(" AND ALLTRIM(Adpaciiden) = ?");
            values.Add(f.Adpaciiden.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Adconscodi))
        {
            sb.Append(" AND ALLTRIM(Adconscodi) = ?");
            values.Add(f.Adconscodi.Trim());
        }
        // Fecha Date con CTOD() — función nativa VFP, formato MM/dd/yyyy
        if (f.Adfechcita.HasValue)
            sb.Append($" AND Adfechcita = CTOD('{f.Adfechcita.Value:MM/dd/yyyy}')");

        if (!string.IsNullOrWhiteSpace(f.Ctadmicodi))
        {
            sb.Append(" AND ALLTRIM(Ctadmicodi) = ?");
            values.Add(f.Ctadmicodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Ctcontcodi))
        {
            sb.Append(" AND ALLTRIM(Ctcontcodi) = ?");
            values.Add(f.Ctcontcodi.Trim());
        }
        if (!string.IsNullOrWhiteSpace(f.Adingrcons))
        {
            sb.Append(" AND ALLTRIM(Adingrcons) = ?");
            values.Add(f.Adingrcons.Trim());
        }

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Adcitas e) =>
    [
        e.Adcitacons,
        e.Geespecodi,
        e.Gemedicodi,
        e.Faservcodi,
        e.Adpaciiden,
        e.Adconscodi,
        e.Adfechcita,
        e.Adhorainic,
        e.Adhorafina,
        e.Adduraminu.HasValue ? e.Adduraminu.Value : (object)DBNull.Value,
        e.Adconsdisp,
        e.Adcitaest,
        e.Adanulcodi,
        e.Ctadmicodi,
        e.Ctcontcodi,
        e.Fechasoli,
        e.Geusuacreo,
        e.Adingrcons,
        e.Faorsecons,
        e.Coconscons,
        e.Faprogcodi,
        e.Fechalleg.HasValue  ? e.Fechalleg.Value  : (object)DBNull.Value,
        e.Adcodanula,
        e.Fechprefpa,
        e.Adcitaande,
        e.Geusuacoan,
        e.Adcitafean.HasValue ? e.Adcitafean.Value : (object)DBNull.Value,
        e.Adciticodi,
    ];

    private static object[] UpdateValues(Adcitas e) =>
    [
        // SET (sin Adcitacons — es la clave, no se actualiza)
        e.Geespecodi,
        e.Gemedicodi,
        e.Faservcodi,
        e.Adpaciiden,
        e.Adconscodi,
        e.Adfechcita,
        e.Adhorainic,
        e.Adhorafina,
        e.Adduraminu.HasValue ? e.Adduraminu.Value : (object)DBNull.Value,
        e.Adconsdisp,
        e.Adcitaest,
        e.Adanulcodi,
        e.Ctadmicodi,
        e.Ctcontcodi,
        e.Fechasoli,
        e.Geusuacreo,
        e.Adingrcons,
        e.Faorsecons,
        e.Coconscons,
        e.Faprogcodi,
        e.Fechalleg.HasValue  ? e.Fechalleg.Value  : (object)DBNull.Value,
        e.Adcodanula,
        e.Fechprefpa,
        e.Adcitaande,
        e.Geusuacoan,
        e.Adcitafean.HasValue ? e.Adcitafean.Value : (object)DBNull.Value,
        e.Adciticodi,
        // WHERE
        e.Adcitacons,
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

    private static async Task<List<Adcitas>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Adcitas>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(MapRow(reader));

        return list;
    }

    // ── Safe helpers por ordinal — evitan InvalidOperationException de VFP ──

    private static int? SafeInt(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? null : (int?)Convert.ToInt32(r.GetValue(i)); }
        catch { return null; }
    }

    private static DateTime? SafeDate(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? null : (DateTime?)Convert.ToDateTime(r.GetValue(i)); }
        catch { return null; }
    }

    private static DateTime SafeDateReq(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? default : Convert.ToDateTime(r.GetValue(i)); }
        catch { return default; }
    }

    private static string SafeString(IDataRecord r, int i)
    {
        try { return r.IsDBNull(i) ? string.Empty : r.GetString(i).TrimEnd(); }
        catch { return string.Empty; }
    }

    private static Adcitas MapRow(IDataRecord r)
    {
        // Índices según el orden exacto de SelectColumns():
        //  0:Adcitacons  1:Geespecodi  2:Gemedicodi  3:Faservcodi  4:Adpaciiden
        //  5:Adconscodi  6:Adfechcita  7:Adhorainic  8:Adhorafina  9:Adduraminu
        // 10:Adconsdisp 11:Adcitaest  12:Adanulcodi 13:Ctadmicodi 14:Ctcontcodi
        // 15:Fechasoli  16:Geusuacreo 17:Adingrcons 18:Faorsecons 19:Coconscons
        // 20:Faprogcodi 21:Fechalleg  22:Adcodanula 23:Fechprefpa 24:Adcitaande
        // 25:Geusuacoan 26:Adcitafean 27:Adciticodi
        return new Adcitas
        {
            Adcitacons = SafeString(r, 0),
            Geespecodi = SafeString(r, 1),
            Gemedicodi = SafeString(r, 2),
            Faservcodi = SafeString(r, 3),
            Adpaciiden = SafeString(r, 4),
            Adconscodi = SafeString(r, 5),
            Adfechcita = SafeDateReq(r, 6),
            Adhorainic = SafeString(r, 7),
            Adhorafina = SafeString(r, 8),
            Adduraminu = SafeInt(r, 9),
            Adconsdisp = SafeString(r, 10),
            Adcitaest = SafeString(r, 11),
            Adanulcodi = SafeString(r, 12),
            Ctadmicodi = SafeString(r, 13),
            Ctcontcodi = SafeString(r, 14),
            Fechasoli = SafeDateReq(r, 15),
            Geusuacreo = SafeString(r, 16),
            Adingrcons = SafeString(r, 17),
            Faorsecons = SafeString(r, 18),
            Coconscons = SafeString(r, 19),
            Faprogcodi = SafeString(r, 20),
            Fechalleg = SafeDate(r, 21),
            Adcodanula = SafeString(r, 22),
            Fechprefpa = SafeDateReq(r, 23),
            Adcitaande = SafeString(r, 24),
            Geusuacoan = SafeString(r, 25),
            Adcitafean = SafeDate(r, 26),
            Adciticodi = SafeString(r, 27),
        };
    }

    private static string SelectColumns() => @"
        Adcitacons, Geespecodi, Gemedicodi, Faservcodi, Adpaciiden,
        Adconscodi, Adfechcita, Adhorainic, Adhorafina, Adduraminu,
        Adconsdisp, Adcitaest,  Adanulcodi, Ctadmicodi, Ctcontcodi,
        Fechasoli,  Geusuacreo, Adingrcons, Faorsecons, Coconscons,
        Faprogcodi, Fechalleg,  Adcodanula, Fechprefpa, Adcitaande,
        Geusuacoan, Adcitafean, Adciticodi";
}