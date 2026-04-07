using System.ComponentModel.DataAnnotations.Schema;

namespace MedisoftAPI.Domain.Entities.Citas;

[Table("Adparametros")]
public class Adparametros
{
    public string Adparametro { get; set; } = string.Empty;  // C(10) — Código del parámetro
    public string Advalorpara { get; set; } = string.Empty;  // C(10) — Valor del parámetro
}