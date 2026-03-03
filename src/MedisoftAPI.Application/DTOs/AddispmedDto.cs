

using System.ComponentModel.DataAnnotations;


public class AddispmedDto
{
    public string Addispcons { get; set; }
    public string Geespecodi { get; set; }
    public string Gemedicodi { get; set; }
    public string Faservcodi { get; set; }
    public string Adconscodi { get; set; }
    public DateTime? Addispfech { get; set; }
    public string Adhoraini { get; set; }
    public string Adhorafin { get; set; }
    public bool? Addispcita { get; set; }
    public bool? Addispplan { get; set; }
}
public class CreateAddispmedDto
{
    public string Geespecodi { get; set; }
    public string Gemedicodi { get; set; }
    public string Faservcodi { get; set; }
    public string Adconscodi { get; set; }
    public DateTime? Addispfech { get; set; }
    public string Adhoraini { get; set; }
    public string Adhorafin { get; set; }
    public bool? Addispcita { get; set; }
    public bool? Addispplan { get; set; }
}
public class UpdateAddispmedDto
{
    public string Geespecodi { get; set; }
    public string Gemedicodi { get; set; }
    public string Faservcodi { get; set; }
    public string Adconscodi { get; set; }
    public DateTime? Addispfech { get; set; }
    public string Adhoraini { get; set; }
    public string Adhorafin { get; set; }
    public bool? Addispcita { get; set; }
    public bool? Addispplan { get; set; }
}
public class AddispmedQueryDto
{
    public string? Addispcons { get; set; }
    public string? Geespecodi { get; set; }
    public string? Gemedicodi { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    // Paginación
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;   // 50 registros por defecto
}

