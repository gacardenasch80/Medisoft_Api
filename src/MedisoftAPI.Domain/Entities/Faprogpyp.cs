namespace MedisoftAPI.Domain.Entities;

public class Faprogpyp
{
    public string Faprogcodi { get; set; } = string.Empty;
    public string Faprogcod1 { get; set; } = string.Empty;
    public string Faprognomb { get; set; } = string.Empty;
    public string Faprogclas { get; set; } = string.Empty;
    public double? Faprogdesd { get; set; }  // Numeric → double?
    public double? Faproghast { get; set; }  // Numeric → double?
    public int? Faproggene { get; set; }  // Numeric pequeño → int?
    public int? Faprogfrec { get; set; }  // Numeric pequeño → int?
    public string Faficocodi { get; set; } = string.Empty;
    public string Fafisecodi { get; set; } = string.Empty;
    public string Hcformular { get; set; } = string.Empty;
    public string Hcforprive { get; set; } = string.Empty;
    public string Hcenfeprve { get; set; } = string.Empty;
    public string Hcenfectrl { get; set; } = string.Empty;
    public string Faservcodi { get; set; } = string.Empty;
    public string Famesecoco { get; set; } = string.Empty;
    public string Famesecopr { get; set; } = string.Empty;
    public int? Faprogestad { get; set; }  // Numeric pequeño → int?
}