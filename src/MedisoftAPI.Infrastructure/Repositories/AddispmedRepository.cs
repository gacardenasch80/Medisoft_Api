using System.Data.OleDb;
using System.Text;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MedisoftAPI.Infrastructure.Repositories;

/// <summary>
/// Repositorio de Addispmed — tabla: Addispmed
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

        // ── Conteo total ──────────────────────────────────────────
        var sqlCount = $"SELECT COUNT(*) FROM Addispmed {where}";
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
                FROM   Addispmed
                {where}
                ORDER BY Addispcons ASC";
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
                           ORDER BY Addispcons ASC
                       )
                ORDER BY Addispcons ASC";
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
            ) VALUES (
                ?,?,?,?,
                ?,?,?,?,
                ?,?
            )";

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

    /// <summary>
    /// Filtros base fijos: addispplan = .T. AND addispcita = .F.
    /// Las fechas usan CTOD() — función nativa VFP para convertir string a Date.
    /// Formato requerido por VFP: MM/dd/yyyy
    /// Las fechas NO usan ? porque CTOD() no acepta parámetros posicionales,
    /// se inyectan directamente formateadas (son DateTime, no input de usuario).
    /// </summary>
    private static (string Where, object[] Values) BuildWhere(AddispmedFilter f)
    {
        var sb = new StringBuilder("WHERE addispplan = .T. AND addispcita = .F.");
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

        // ── Fechas con CTOD() — VFP no acepta DateTime como parámetro ? ──
        // Se formatean como MM/dd/yyyy y se inyectan directamente.
        // Son valores DateTime del sistema, no input libre del usuario.
        if (f.FechaInicio.HasValue)
        {
            sb.Append($" AND Addispfech >= CTOD('{f.FechaInicio.Value:MM/dd/yyyy}')");
        }
        if (f.FechaFin.HasValue)
        {
            sb.Append($" AND Addispfech <= CTOD('{f.FechaFin.Value:MM/dd/yyyy}')");
        }

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
        // SET (sin Addispcons — es la clave, no se actualiza)
        e.Geespecodi ?? (object)DBNull.Value,
        e.Gemedicodi ?? (object)DBNull.Value,
        e.Faservcodi ?? (object)DBNull.Value,
        e.Adconscodi ?? (object)DBNull.Value,
        e.Addispfech.HasValue ? e.Addispfech.Value : (object)DBNull.Value,
        e.Adhoraini  ?? (object)DBNull.Value,
        e.Adhorafin  ?? (object)DBNull.Value,
        e.Addispcita.HasValue ? e.Addispcita.Value : (object)DBNull.Value,
        e.Addispplan.HasValue ? e.Addispplan.Value : (object)DBNull.Value,
        // WHERE
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

    private static Addispmed MapRow(System.Data.IDataRecord r) => new()
    {
        Addispcons = r["Addispcons"] as string,
        Geespecodi = r["Geespecodi"] as string,
        Gemedicodi = r["Gemedicodi"] as string,
        Faservcodi = r["Faservcodi"] as string,
        Adconscodi = r["Adconscodi"] as string,
        Addispfech = r["Addispfech"] == DBNull.Value ? null : Convert.ToDateTime(r["Addispfech"]),
        Adhoraini = r["Adhoraini"] as string,
        Adhorafin = r["Adhorafin"] as string,
        Addispcita = r["Addispcita"] == DBNull.Value ? null : Convert.ToBoolean(r["Addispcita"]),
        Addispplan = r["Addispplan"] == DBNull.Value ? null : Convert.ToBoolean(r["Addispplan"]),
    };

    private static string SelectColumns() => @"
        Addispcons, Geespecodi, Gemedicodi, Faservcodi,
        Adconscodi, Addispfech, Adhoraini,  Adhorafin,
        Addispcita, Addispplan";
}