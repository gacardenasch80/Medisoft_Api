

using System.ComponentModel.DataAnnotations;


public class GeespecialDto
{
    public string Geespecodi { get; set; }
    public string Geespenomb { get; set; }
    public int? Geespesv18 { get; set; }
    public int? Geespeodon { get; set; }
    public int? Hcrevartip { get; set; }
}
public class CreateGeespecialDto
{
    public string Geespecodi { get; set; }
    public string Geespenomb { get; set; }
    public int? Geespesv18 { get; set; }
    public int? Geespeodon { get; set; }
    public int? Hcrevartip { get; set; }
}
public class UpdateGeespecialDto
{
    public string Geespecodi { get; set; }
    public string Geespenomb { get; set; }
    public int? Geespesv18 { get; set; }
    public int? Geespeodon { get; set; }
    public int? Hcrevartip { get; set; }
}
public class GeespecialQueryDto
{
    public string? Geespecodi { get; set; }
    public string? Geespenomb { get; set; }
    // Paginación
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;   // 50 registros por defecto
}

