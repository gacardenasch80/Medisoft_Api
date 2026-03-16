namespace MedisoftAPI.Application.DTOs;

// ── DTO base (sin enriquecer) ─────────────────────────────────────────────
public class AddispmedDto
{
    public string? Addispcons { get; set; }
    public string? Geespecodi { get; set; }
    public string? Gemedicodi { get; set; }
    public string? Faservcodi { get; set; }
    public string? Adconscodi { get; set; }
    public DateTime? Addispfech { get; set; }
    public string? Adhoraini { get; set; }
    public string? Adhorafin { get; set; }
    public bool? Addispcita { get; set; }
    public bool? Addispplan { get; set; }
}

// ── DTO enriquecido con nombres ───────────────────────────────────────────
public class AddispmedDetalleDto
{
    public string? Addispcons { get; set; }
    public string? Geespecodi { get; set; }
    public string? Geespenomb { get; set; }   // ← nombre especialidad
    public string? Gemedicodi { get; set; }
    public string? Gemedinomb { get; set; }   // ← nombre médico
    public string? Faservcodi { get; set; }
    public string? Faservnomb { get; set; }   // ← nombre servicio
    public string? Adconscodi { get; set; }
    public DateTime? Addispfech { get; set; }
    public string? Adhoraini { get; set; }
    public string? Adhorafin { get; set; }
    public bool? Addispcita { get; set; }
    public bool? Addispplan { get; set; }
}

// ── Query ─────────────────────────────────────────────────────────────────
public class AddispmedQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Addispcons { get; set; }
    public string? Geespecodi { get; set; }
    public string? Gemedicodi { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}

// ── Create / Update (sin cambios) ─────────────────────────────────────────
public class CreateAddispmedDto
{
    public string? Geespecodi { get; set; }
    public string? Gemedicodi { get; set; }
    public string? Faservcodi { get; set; }
    public string? Adconscodi { get; set; }
    public DateTime? Addispfech { get; set; }
    public string? Adhoraini { get; set; }
    public string? Adhorafin { get; set; }
    public bool? Addispcita { get; set; }
    public bool? Addispplan { get; set; }
}

public class UpdateAddispmedDto
{
    public string? Geespecodi { get; set; }
    public string? Gemedicodi { get; set; }
    public string? Faservcodi { get; set; }
    public string? Adconscodi { get; set; }
    public DateTime? Addispfech { get; set; }
    public string? Adhoraini { get; set; }
    public string? Adhorafin { get; set; }
    public bool? Addispcita { get; set; }
    public bool? Addispplan { get; set; }
}