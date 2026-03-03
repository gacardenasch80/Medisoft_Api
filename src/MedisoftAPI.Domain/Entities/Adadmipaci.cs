using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedisoftAPI.Domain.Entities;

/// <summary>Espejo de la tabla FoxPro ADADMIPACI.DBF — base de datos: admision</summary>
[Table("Adadmipaci")]
public class Adadmipaci
{
    [MaxLength(8)] public string? ADADPACONS { get; set; }
    [MaxLength(3)] public string? CTADMICODI { get; set; }
    [MaxLength(20)] public string? ADPACIIDEN { get; set; }
    [MaxLength(2)] public string? ADTIAFCODI { get; set; }
    [MaxLength(10)] public string? ADADPADOCU { get; set; }
    [MaxLength(5)] public string? CTCONTCODI { get; set; }
    public int? ADADPAESTA { get; set; }   // Numeric 1
    [MaxLength(2)] public string? CTNIVECODI { get; set; }
    public DateTime? ADADPAFEIN { get; set; }   // Date
    public DateTime? ADADPAFEFI { get; set; }   // Date
}