namespace MedisoftAPI.Application.DTOs.Citas;

public class AdparametrosDto
{
    public string? Adparametro { get; set; }
    public string? Advalorpara { get; set; }
}

public class AdparametrosQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Adparametro { get; set; }
    public string? Advalorpara { get; set; }
}

public class CreateAdparametrosDto
{
    public required string Adparametro { get; set; }
    public string? Advalorpara { get; set; }
}

public class UpdateAdparametrosDto
{
    public required string Advalorpara { get; set; }
}