namespace MedisoftAPI.Application.DTOs.Generales;

public class GehospitalDto
{
    public string? Gehospcodi { get; set; }
    public string? Gehospnomb { get; set; }
    public string? Gehosptiid { get; set; }
    public string? Gehospnit { get; set; }
    public string? Gehospdire { get; set; }
    public string? Gehosptele { get; set; }
    public string? Gehospreso { get; set; }
    public string? Gehospisgss { get; set; }
    public string? Gehosprele { get; set; }
    public string? Gehospemai { get; set; }
    public string? Gehosppagi { get; set; }
    public string? Gedepacodi { get; set; }
    public string? Gemunicodi { get; set; }
    public string? Gehosprefa { get; set; }
    public string? Gehospreec { get; set; }
    public string? Geresudian { get; set; }
    public string? Gemensage { get; set; }
    public string? Gehospdigi { get; set; }
    public string? Gehospindi { get; set; }
    public string? Gehospext { get; set; }
    public int? Gemoduacti { get; set; }
    public int? Gehoenviro { get; set; }
    public string? Gehosetpru { get; set; }
    public string? Gehocurren { get; set; }
    public string? Gehocertif { get; set; }
    public int? Gehosubtyp { get; set; }
    public string? Faferecodi { get; set; }
    public int? Gehofaeles { get; set; }
    public string? Gehomailsi { get; set; }
    public string? Fafetifcod { get; set; }
    public int? Gehostyper { get; set; }
    public string? Gehosrerut { get; set; }
    public string? Gehosquaid { get; set; }
    public int? Gehosquaes { get; set; }
    public string? Gehosproxy { get; set; }
    public string? Gehoslogid { get; set; }
    public string? Gehosimgid { get; set; }
    public string? Gehocerdia { get; set; }
    public string? Fafetpdcod { get; set; }
}

public class GehospitalQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string? Gehospcodi { get; set; }
    public string? Gehospnomb { get; set; }
    public string? Gehospnit { get; set; }
    public string? Gedepacodi { get; set; }
    public string? Gemunicodi { get; set; }
}

public class CreateGehospitalDto
{
    public required string Gehospcodi { get; set; }
    public string? Gehospnomb { get; set; }
    public string? Gehosptiid { get; set; }
    public string? Gehospnit { get; set; }
    public string? Gehospdire { get; set; }
    public string? Gehosptele { get; set; }
    public string? Gehospreso { get; set; }
    public string? Gehospisgss { get; set; }
    public string? Gehosprele { get; set; }
    public string? Gehospemai { get; set; }
    public string? Gehosppagi { get; set; }
    public string? Gedepacodi { get; set; }
    public string? Gemunicodi { get; set; }
    public string? Gehosprefa { get; set; }
    public string? Gehospreec { get; set; }
    public string? Geresudian { get; set; }
    public string? Gemensage { get; set; }
    public string? Gehospdigi { get; set; }
    public string? Gehospindi { get; set; }
    public string? Gehospext { get; set; }
    public int? Gemoduacti { get; set; }
    public int? Gehoenviro { get; set; }
    public string? Gehosetpru { get; set; }
    public string? Gehocurren { get; set; }
    public string? Gehocertif { get; set; }
    public int? Gehosubtyp { get; set; }
    public string? Faferecodi { get; set; }
    public int? Gehofaeles { get; set; }
    public string? Gehomailsi { get; set; }
    public string? Fafetifcod { get; set; }
    public int? Gehostyper { get; set; }
    public string? Gehosrerut { get; set; }
    public string? Gehosquaid { get; set; }
    public int? Gehosquaes { get; set; }
    public string? Gehosproxy { get; set; }
    public string? Gehoslogid { get; set; }
    public string? Gehosimgid { get; set; }
    public string? Gehocerdia { get; set; }
    public string? Fafetpdcod { get; set; }
}

public class UpdateGehospitalDto
{
    public string? Gehospnomb { get; set; }
    public string? Gehosptiid { get; set; }
    public string? Gehospnit { get; set; }
    public string? Gehospdire { get; set; }
    public string? Gehosptele { get; set; }
    public string? Gehospreso { get; set; }
    public string? Gehospisgss { get; set; }
    public string? Gehosprele { get; set; }
    public string? Gehospemai { get; set; }
    public string? Gehosppagi { get; set; }
    public string? Gedepacodi { get; set; }
    public string? Gemunicodi { get; set; }
    public string? Gehosprefa { get; set; }
    public string? Gehospreec { get; set; }
    public string? Geresudian { get; set; }
    public string? Gemensage { get; set; }
    public string? Gehospdigi { get; set; }
    public string? Gehospindi { get; set; }
    public string? Gehospext { get; set; }
    public int? Gemoduacti { get; set; }
    public int? Gehoenviro { get; set; }
    public string? Gehosetpru { get; set; }
    public string? Gehocurren { get; set; }
    public string? Gehocertif { get; set; }
    public int? Gehosubtyp { get; set; }
    public string? Faferecodi { get; set; }
    public int? Gehofaeles { get; set; }
    public string? Gehomailsi { get; set; }
    public string? Fafetifcod { get; set; }
    public int? Gehostyper { get; set; }
    public string? Gehosrerut { get; set; }
    public string? Gehosquaid { get; set; }
    public int? Gehosquaes { get; set; }
    public string? Gehosproxy { get; set; }
    public string? Gehoslogid { get; set; }
    public string? Gehosimgid { get; set; }
    public string? Gehocerdia { get; set; }
    public string? Fafetpdcod { get; set; }
}