using System.Data;
using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities.Citas;
using MedisoftAPI.Domain.Interfaces.Citas;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories.Citas;

/// <summary>
/// Repositorio de Addispmed — tabla: Addispmed (citas.dbc)
///
/// ⚠️ VFP/OleDb NO soporta parámetros nombrados (@param).
///    Se usan ? posicionales y ADO.NET directo.
///    Las fechas usan CTOD('MM/dd/yyyy') que es la función nativa de VFP.
///    Filtros base fijos: addispplan = .T. AND addispcita = .F.
/// </summary>
public class AddispmedRepository : IAddispmedRepository
{
    private readonly string _conn;

    public AddispmedRepository(IConfiguration cfg)
    {
        // ✅ citas.dbc → FoxPro_Cit  (NO FoxPro_Adm)
        _conn = cfg.GetConnectionString("FoxPro_Cit")
            ?? throw new InvalidOperationException(
                "La cadena 'FoxPro_Cit' no está configurada en appsettings.json.");
    }

    // ── GET ALL (paginado) ────────────────────────────────────────────────

    public async Task<(IEnumerable<Addispmed> Items, int Total)> GetAllAsync(AddispmedFilter filter)
    {
        var (where, paramValues) = BuildWhere(filter);

        int pagina = Math.Max(1, filter.Pagina);
        int tamPagina = Math.Clamp(filter.TamPagina, 1, 200);
        int offset = (pagina - 1) * tamPagina;

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var sqlCount = $"SELECT COUNT(*) FROM Addispmed {where}";
        var total = await ExecuteScalarAsync(conn, sqlCount, MakeParams(paramValues));

        if (total == 0)
            return ([], 0);

        string sqlData;
        object[] queryValues;

        if (offset == 0)
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Addispmed
                {where}
                ORDER BY Addispfech, Adhoraini ASC";
            queryValues = paramValues;
        }
        else
        {
            sqlData = $@"
                SELECT TOP {tamPagina} {SelectColumns()}
                FROM   Addispmed
                {where}
                AND    Addispcons NOT IN (
                           SELECT TOP {offset} Addispcons
                           FROM   Addispmed
                           {where}
                           ORDER BY Addispfech, Adhoraini ASC
                       )
                ORDER BY Addispfech, Adhoraini ASC";
            queryValues = [.. paramValues, .. paramValues];
        }

        var items = await QueryAsync(conn, sqlData, MakeParams(queryValues));
        return (items, total);
    }

    // ── GET BY CODE ───────────────────────────────────────────────────────

    public async Task<Addispmed?> GetByCodeAsync(string addispcons)
    {
        var sql = $@"
            SELECT {SelectColumns()}
            FROM   Addispmed
            WHERE  ALLTRIM(Addispcons) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();

        var items = await QueryAsync(conn, sql, MakeParams([addispcons.Trim()]));
        return items.FirstOrDefault();
    }

    // ── CREATE ────────────────────────────────────────────────────────────

    public async Task<Addispmed> CreateAsync(Addispmed e)
    {
        const string sql = @"
            INSERT INTO Addispmed (
                Addispcons, Geespecodi, Gemedicodi, Faservcodi,
                Adconscodi, Addispfech, Adhoraini,  Adhorafin,
                Addispcita, Addispplan
            ) VALUES (?,?,?,?,?,?,?,?,?,?)";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(InsertValues(e)));
        return e;
    }

    // ── UPDATE ────────────────────────────────────────────────────────────

    public async Task<Addispmed> UpdateAsync(Addispmed e)
    {
        const string sql = @"
            UPDATE Addispmed SET
                Geespecodi = ?, Gemedicodi = ?, Faservcodi = ?,
                Adconscodi = ?, Addispfech = ?, Adhoraini  = ?,
                Adhorafin  = ?, Addispcita = ?, Addispplan = ?
            WHERE ALLTRIM(Addispcons) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        await ExecuteAsync(conn, sql, MakeParams(UpdateValues(e)));
        return e;
    }

    // ── DELETE ────────────────────────────────────────────────────────────

    public async Task<bool> DeleteAsync(string addispcons)
    {
        const string sql = "DELETE FROM Addispmed WHERE ALLTRIM(Addispcons) = ?";

        using var conn = new OleDbConnection(_conn);
        await conn.OpenAsync();
        return await ExecuteAsync(conn, sql, MakeParams([addispcons.Trim()])) > 0;
    }

    // ── BUILD WHERE ───────────────────────────────────────────────────────

    private static (string Where, object[] Values) BuildWhere(AddispmedFilter f)
    {
        DateTime ahora = DateTime.Now;
        string fechaHoy = ahora.ToString("MM/dd/yyyy");
        string horaActual = ahora.ToString("HH:mm");

        var sb = new StringBuilder(
            $"WHERE addispplan = .T. AND addispcita = .F." +
            $" AND (Addispfech > CTOD('{fechaHoy}')" +
            $" OR (Addispfech = CTOD('{fechaHoy}') AND ALLTRIM(Adhoraini) >= '{horaActual}'))");

        var values = new List<object>();

        if (!string.IsNullOrWhiteSpace(f.Addispcons))
        {
            sb.Append(" AND ALLTRIM(Addispcons) = ?");
            values.Add(f.Addispcons.Trim());
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
        if (!string.IsNullOrWhiteSpace(f.Adconscodi))
        {
            sb.Append(" AND ALLTRIM(Adconscodi) = ?");
            values.Add(f.Adconscodi.Trim());
        }
        if (f.FechaInicio.HasValue)
            sb.Append($" AND Addispfech >= CTOD('{f.FechaInicio.Value:MM/dd/yyyy}')");
        if (f.FechaFin.HasValue)
            sb.Append($" AND Addispfech <= CTOD('{f.FechaFin.Value:MM/dd/yyyy}')");

        return (sb.ToString(), values.ToArray());
    }

    // ── VALORES INSERT / UPDATE ───────────────────────────────────────────

    private static object[] InsertValues(Addispmed e) =>
    [
        e.Addispcons ?? (object)DBNull.Value,
        e.Geespecodi ?? (object)DBNull.Value,
        e.Gemedicodi ?? (object)DBNull.Value,
        e.Faservcodi ?? (object)DBNull.Value,
        e.Adconscodi ?? (object)DBNull.Value,
        e.Addispfech.HasValue ? e.Addispfech.Value : (object)DBNull.Value,
        e.Adhoraini  ?? (object)DBNull.Value,
        e.Adhorafin  ?? (object)DBNull.Value,
        e.Addispcita.HasValue ? e.Addispcita.Value : (object)DBNull.Value,
        e.Addispplan.HasValue ? e.Addispplan.Value : (object)DBNull.Value,
    ];

    private static object[] UpdateValues(Addispmed e) =>
    [
        e.Geespecodi ?? (object)DBNull.Value,
        e.Gemedicodi ?? (object)DBNull.Value,
        e.Faservcodi ?? (object)DBNull.Value,
        e.Adconscodi ?? (object)DBNull.Value,
        e.Addispfech.HasValue ? e.Addispfech.Value : (object)DBNull.Value,
        e.Adhoraini  ?? (object)DBNull.Value,
        e.Adhorafin  ?? (object)DBNull.Value,
        e.Addispcita.HasValue ? e.Addispcita.Value : (object)DBNull.Value,
        e.Addispplan.HasValue ? e.Addispplan.Value : (object)DBNull.Value,
        e.Addispcons ?? (object)DBNull.Value,
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

    private static async Task<List<Addispmed>> QueryAsync(
        OleDbConnection conn, string sql, OleDbParameter[] p)
    {
        using var cmd = new OleDbCommand(sql, conn);
        cmd.Parameters.AddRange(p);

        var list = new List<Addispmed>();
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

    private static Addispmed MapRow(IDataRecord r)
    {
        // 0:Addispcons  1:Geespecodi  2:Gemedicodi  3:Faservcodi
        // 4:Adconscodi  5:Addispfech  6:Adhoraini   7:Adhorafin
        // 8:Addispcita  9:Addispplan
        return new Addispmed
        {
            Addispcons = SafeString(r, 0),
            Geespecodi = SafeString(r, 1),
            Gemedicodi = SafeString(r, 2),
            Faservcodi = SafeString(r, 3),
            Adconscodi = SafeString(r, 4),
            Addispfech = r.IsDBNull(5) ? null : (DateTime?)Convert.ToDateTime(r.GetValue(5)),
            Adhoraini = SafeString(r, 6),
            Adhorafin = SafeString(r, 7),
            Addispcita = r.IsDBNull(8) ? null : (bool?)Convert.ToBoolean(r.GetValue(8)),
            Addispplan = r.IsDBNull(9) ? null : (bool?)Convert.ToBoolean(r.GetValue(9)),
        };
    }

    private static string SelectColumns() => @"
        Addispcons, Geespecodi, Gemedicodi, Faservcodi,
        Adconscodi, Addispfech, Adhoraini,  Adhorafin,
        Addispcita, Addispplan";
}