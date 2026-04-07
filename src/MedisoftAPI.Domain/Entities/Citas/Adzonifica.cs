using System.ComponentModel.DataAnnotations.Schema;

namespace MedisoftAPI.Domain.Entities.Citas;

[Table("Adzonifica")]
public class Adzonifica
{
    public string Adpaciiden { get; set; } = string.Empty;  // C(20) — Identificación del paciente
    public string Geunprcodi { get; set; } = string.Empty;  // C(2)  — Código unidad de producción
    public double? Estado { get; set; }                  // N(1)  — 1=Activo, 2=Inactivo
}