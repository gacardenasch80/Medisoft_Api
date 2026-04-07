namespace MedisoftAPI.Application.DTOs.Generales;

public class GeunidprodDto
{
    public string? Geunprcodi { get; set; }
    public string? Geunprnomb { get; set; }
    public string? Geunprdire { get; set; }
    public string? Geunprtele { get; set; }
    public string? Geunprresp { get; set; }
    public string? Gefarmpref { get; set; }
}

public class GeunidprodQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Geunprcodi { get; set; }
    public string? Geunprnomb { get; set; }
    public string? Geunprresp { get; set; }
}

public class CreateGeunidprodDto
{
    public required string Geunprcodi { get; set; }
    public string? Geunprnomb { get; set; }
    public string? Geunprdire { get; set; }
    public string? Geunprtele { get; set; }
    public string? Geunprresp { get; set; }
    public string? Gefarmpref { get; set; }
}

public class UpdateGeunidprodDto
{
    public string? Geunprnomb { get; set; }
    public string? Geunprdire { get; set; }
    public string? Geunprtele { get; set; }
    public string? Geunprresp { get; set; }
    public string? Gefarmpref { get; set; }
}