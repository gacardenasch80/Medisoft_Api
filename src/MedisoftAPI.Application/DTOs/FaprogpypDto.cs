namespace MedisoftAPI.Application.DTOs;

public class FaprogpypDto
{
    public string? Faprogcodi { get; set; }
    public string? Faprogcod1 { get; set; }
    public string? Faprognomb { get; set; }
    public string? Faprogclas { get; set; }
    public double? Faprogdesd { get; set; }
    public double? Faproghast { get; set; }
    public int? Faproggene { get; set; }
    public int? Faprogfrec { get; set; }
    public string? Faficocodi { get; set; }
    public string? Fafisecodi { get; set; }
    public string? Hcformular { get; set; }
    public string? Hcforprive { get; set; }
    public string? Hcenfeprve { get; set; }
    public string? Hcenfectrl { get; set; }
    public string? Faservcodi { get; set; }
    public string? Famesecoco { get; set; }
    public string? Famesecopr { get; set; }
    public int? Faprogestad { get; set; }
}

public class FaprogpypQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Faprogcodi { get; set; }
    public string? Faprogcod1 { get; set; }
    public string? Faprognomb { get; set; }
    public string? Faprogclas { get; set; }
    public string? Faservcodi { get; set; }
    public int? Faprogestad { get; set; }
}

public class CreateFaprogpypDto
{
    public required string Faprogcodi { get; set; }
    public string? Faprogcod1 { get; set; }
    public string? Faprognomb { get; set; }
    public string? Faprogclas { get; set; }
    public double? Faprogdesd { get; set; }
    public double? Faproghast { get; set; }
    public int? Faproggene { get; set; }
    public int? Faprogfrec { get; set; }
    public string? Faficocodi { get; set; }
    public string? Fafisecodi { get; set; }
    public string? Hcformular { get; set; }
    public string? Hcforprive { get; set; }
    public string? Hcenfeprve { get; set; }
    public string? Hcenfectrl { get; set; }
    public string? Faservcodi { get; set; }
    public string? Famesecoco { get; set; }
    public string? Famesecopr { get; set; }
    public int? Faprogestad { get; set; }
}

public class UpdateFaprogpypDto
{
    public string? Faprogcod1 { get; set; }
    public string? Faprognomb { get; set; }
    public string? Faprogclas { get; set; }
    public double? Faprogdesd { get; set; }
    public double? Faproghast { get; set; }
    public int? Faproggene { get; set; }
    public int? Faprogfrec { get; set; }
    public string? Faficocodi { get; set; }
    public string? Fafisecodi { get; set; }
    public string? Hcformular { get; set; }
    public string? Hcforprive { get; set; }
    public string? Hcenfeprve { get; set; }
    public string? Hcenfectrl { get; set; }
    public string? Faservcodi { get; set; }
    public string? Famesecoco { get; set; }
    public string? Famesecopr { get; set; }
    public int? Faprogestad { get; set; }
}