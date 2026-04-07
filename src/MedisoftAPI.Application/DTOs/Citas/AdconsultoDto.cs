using MedisoftAPI.Application.DTOs.Contratacion;

namespace MedisoftAPI.Application.DTOs.Citas;

public class AdconsultoDto
{
    public string Adconscodi { get; set; }
    public string Adconsnomb { get; set; }
    public string Adconsdire { get; set; }
    public string Adconstele { get; set; }
    public int? Adconsesur { get; set; }
    public string Geunprcodi { get; set; }
}

public class AdconsultoQueryDto
{
    public string? Adconscodi { get; set; }
    public string? Adconsnomb { get; set; }
    public string? Adconsdire { get; set; }
    public string? Adconstele { get; set; }
    public string? Geunprcodi { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
}

public class CreateAdconsultoDto
{
    public required string Adconscodi { get; set; }
    public string Adconsnomb { get; set; }
    public string Adconsdire { get; set; }
    public string Adconstele { get; set; }
    public int? Adconsesur { get; set; }
    public string Geunprcodi { get; set; }
}

public class UpdateAdconsultoDto
{
    public string Adconsnomb { get; set; }
    public string Adconsdire { get; set; }
    public string Adconstele { get; set; }
    public int? Adconsesur { get; set; }
    public string Geunprcodi { get; set; }
}