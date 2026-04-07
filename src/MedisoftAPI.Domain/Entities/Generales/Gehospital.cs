using System.ComponentModel.DataAnnotations.Schema;

namespace MedisoftAPI.Domain.Entities.Generales;

[Table("Gehospital")]
public class Gehospital
{
    public string Gehospcodi { get; set; } = string.Empty;
    public string Gehospnomb { get; set; } = string.Empty;
    public string Gehosptiid { get; set; } = string.Empty;
    public string Gehospnit { get; set; } = string.Empty;
    public string Gehospdire { get; set; } = string.Empty;
    public string Gehosptele { get; set; } = string.Empty;
    public string Gehospreso { get; set; } = string.Empty;
    public string Gehospisgss { get; set; } = string.Empty;
    public string Gehosprele { get; set; } = string.Empty;
    public string Gehospemai { get; set; } = string.Empty;
    public string Gehosppagi { get; set; } = string.Empty;
    public string Gedepacodi { get; set; } = string.Empty;
    public string Gemunicodi { get; set; } = string.Empty;
    public string Gehosprefa { get; set; } = string.Empty;
    public string Gehospreec { get; set; } = string.Empty;
    public string Geresudian { get; set; } = string.Empty;
    public string Gemensage { get; set; } = string.Empty;
    public string Gehospdigi { get; set; } = string.Empty;
    public string Gehospindi { get; set; } = string.Empty;
    public string Gehospext { get; set; } = string.Empty;
    public int? Gemoduacti { get; set; }  // Integer → int?
    public int? Gehoenviro { get; set; }  // Numeric(1) → int?
    public string Gehosetpru { get; set; } = string.Empty;
    public string Gehocurren { get; set; } = string.Empty;
    public string Gehocertif { get; set; } = string.Empty;
    public int? Gehosubtyp { get; set; }  // Numeric(1) → int?
    public string Faferecodi { get; set; } = string.Empty;
    public int? Gehofaeles { get; set; }  // Numeric(1) → int?
    public string Gehomailsi { get; set; } = string.Empty;
    public string Fafetifcod { get; set; } = string.Empty;
    public int? Gehostyper { get; set; }  // Numeric(2) → int?
    public string Gehosrerut { get; set; } = string.Empty;
    public string Gehosquaid { get; set; } = string.Empty;
    public int? Gehosquaes { get; set; }  // Numeric(2) → int?
    public string Gehosproxy { get; set; } = string.Empty;
    public string Gehoslogid { get; set; } = string.Empty;
    public string Gehosimgid { get; set; } = string.Empty;
    public string Gehocerdia { get; set; } = string.Empty;
    public string Fafetpdcod { get; set; } = string.Empty;
}