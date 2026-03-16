using MedisoftAPI.Application.DTOs.Contratacion;

namespace MedisoftAPI.Application.DTOs.Admision;

public class AdadmipaciDto
{
    public string? ADADPACONS { get; set; }
    public string? CTADMICODI { get; set; }
    public string? ADPACIIDEN { get; set; }
    public string? ADTIAFCODI { get; set; }
    public string? ADADPADOCU { get; set; }
    public string? CTCONTCODI { get; set; }
    public int? ADADPAESTA { get; set; }
    public string? CTNIVECODI { get; set; }
    public DateTime? ADADPAFEIN { get; set; }
    public DateTime? ADADPAFEFI { get; set; }
}

public class AdadmipaciQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    public string? ADADPACONS { get; set; }
    public string? CTADMICODI { get; set; }
    public string? ADPACIIDEN { get; set; }
    public string? CTCONTCODI { get; set; }
    public int? ADADPAESTA { get; set; }
    public string? CTNIVECODI { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}

public class CreateAdadmipaciDto
{
    public required string ADADPACONS { get; set; }
    public string? CTADMICODI { get; set; }
    public string? ADPACIIDEN { get; set; }
    public string? ADTIAFCODI { get; set; }
    public string? ADADPADOCU { get; set; }
    public string? CTCONTCODI { get; set; }
    public int? ADADPAESTA { get; set; }
    public string? CTNIVECODI { get; set; }
    public DateTime? ADADPAFEIN { get; set; }
    public DateTime? ADADPAFEFI { get; set; }
}

public class UpdateAdadmipaciDto
{
    public string? CTADMICODI { get; set; }
    public string? ADPACIIDEN { get; set; }
    public string? ADTIAFCODI { get; set; }
    public string? ADADPADOCU { get; set; }
    public string? CTCONTCODI { get; set; }
    public int? ADADPAESTA { get; set; }
    public string? CTNIVECODI { get; set; }
    public DateTime? ADADPAFEIN { get; set; }
    public DateTime? ADADPAFEFI { get; set; }
}
/// <summary>
/// Admisión enriquecida — incluye el contrato y la administradora relacionados
/// </summary>
public class AdadmipaciDetalleDto
{
    public AdadmipaciDto Admision { get; set; } = null!;
    public CtcontratoDto? Contrato { get; set; }
    public CtadministDto? Administradora { get; set; }
}