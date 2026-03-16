namespace MedisoftAPI.Application.DTOs.Generales;

public class GemedicosDto
{
    public string Gemedicodi { get; set; } = string.Empty;
    public string Gemedinomb { get; set; } = string.Empty;
    public string Gemedireme { get; set; } = string.Empty;
    public string Gereincodi { get; set; } = string.Empty;
    public string Gefirmesca { get; set; } = string.Empty;
    public string Gemeditele { get; set; } = string.Empty;
    public string Gemedicelu { get; set; } = string.Empty;
    public int? Gemedact { get; set; }
    public int? Geesjefeen { get; set; }
}

public class CreateGemedicosDto
{
    public string Gemedicodi { get; set; } = string.Empty;
    public string Gemedinomb { get; set; } = string.Empty;
    public string Gemedireme { get; set; } = string.Empty;
    public string Gereincodi { get; set; } = string.Empty;
    public string Gefirmesca { get; set; } = string.Empty;
    public string Gemeditele { get; set; } = string.Empty;
    public string Gemedicelu { get; set; } = string.Empty;
    public int? Gemedact { get; set; }
    public int? Geesjefeen { get; set; }
}

public class UpdateGemedicosDto
{
    public string Gemedicodi { get; set; } = string.Empty;
    public string Gemedinomb { get; set; } = string.Empty;
    public string Gemedireme { get; set; } = string.Empty;
    public string Gereincodi { get; set; } = string.Empty;
    public string Gefirmesca { get; set; } = string.Empty;
    public string Gemeditele { get; set; } = string.Empty;
    public string Gemedicelu { get; set; } = string.Empty;
    public int? Gemedact { get; set; }
    public int? Geesjefeen { get; set; }
}

public class GemedicosQueryDto
{
    // Paginación
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;   // 50 registros por defecto
}

