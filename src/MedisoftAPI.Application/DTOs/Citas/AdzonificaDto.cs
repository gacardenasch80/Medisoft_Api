namespace MedisoftAPI.Application.DTOs.Citas;

public class AdzonificaDto
{
    public string? Adpaciiden { get; set; }
    public string? Geunprcodi { get; set; }
    /// <summary>1 = Activo, 2 = Inactivo</summary>
    public double? Estado { get; set; }
}

public class AdzonificaQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Adpaciiden { get; set; }
    public string? Geunprcodi { get; set; }
    /// <summary>1 = Activo, 2 = Inactivo</summary>
    public double? Estado { get; set; }
}

public class CreateAdzonificaDto
{
    public required string Adpaciiden { get; set; }
    public required string Geunprcodi { get; set; }
    /// <summary>1 = Activo, 2 = Inactivo</summary>
    public double? Estado { get; set; } = 1;
}

public class UpdateAdzonificaDto
{
    /// <summary>1 = Activo, 2 = Inactivo</summary>
    public required double Estado { get; set; }
}