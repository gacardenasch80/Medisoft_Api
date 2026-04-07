namespace MedisoftAPI.Domain.Entities.Generales;

public class Geunidprod
{
    public string Geunprcodi { get; set; } = string.Empty;  // C(3)  — Código unidad de producción
    public string Geunprnomb { get; set; } = string.Empty;  // C(40) — Nombre
    public string Geunprdire { get; set; } = string.Empty;  // C(60) — Dirección
    public string Geunprtele { get; set; } = string.Empty;  // C(30) — Teléfono
    public string Geunprresp { get; set; } = string.Empty;  // C(40) — Responsable
    public string Gefarmpref { get; set; } = string.Empty;  // C(3)  — Farmacia preferida
}