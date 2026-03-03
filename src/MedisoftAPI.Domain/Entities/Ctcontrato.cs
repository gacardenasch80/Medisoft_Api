using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedisoftAPI.Domain.Entities;

/// <summary>
/// Espejo de la tabla FoxPro Ctcontrato
/// </summary>
[Table("Ctcontrato")]
public class Ctcontrato
{
    // ── Identificación ────────────────────────────────────────────────────
    [MaxLength(5)] public string? CTCONTCODI { get; set; }
    [MaxLength(25)] public string? CTCONTNUME { get; set; }
    [MaxLength(3)] public string? CTADMICODI { get; set; }
    [MaxLength(2)] public string? CTREGICODI { get; set; }
    [MaxLength(2)] public string? CTPLBECODI { get; set; }
    [MaxLength(2)] public string? CTMANUCODI { get; set; }
    [MaxLength(2)] public string? CTCONTLEGA { get; set; }

    // ── Fechas ────────────────────────────────────────────────────────────
    public DateTime? CTCONTFELE { get; set; }   // Date
    public DateTime? CTCONTDESD { get; set; }   // Date
    public DateTime? CTCONTHAST { get; set; }   // Date

    // ── Valores y estado ──────────────────────────────────────────────────
    public double? CTCONTVALO { get; set; }   // Numeric 12
    [MaxLength(1)] public string? CTCONTESTA { get; set; }
    [MaxLength(1)] public string? CTTICOCODI { get; set; }
    public double? CTCONTASIG { get; set; }   // Numeric 12
    public double? CTCONTFACT { get; set; }   // Numeric 12
    public int? FAOBSHORA { get; set; }   // Numeric 2
    [MaxLength(40)] public string? CTCTAACRE { get; set; }
    public int? CTCITXDIA { get; set; }   // Numeric 4
    public int? CTCONTCONS { get; set; }   // Numeric 3

    // ── Auditoría ─────────────────────────────────────────────────────────
    public DateTime? CTCONTFECR { get; set; }   // DateTime
    [MaxLength(4)] public string? CTCONTUSCR { get; set; }
    public DateTime? CTCONTFEED { get; set; }   // DateTime
    [MaxLength(4)] public string? CTCONTUSED { get; set; }

    // ── Configuración ─────────────────────────────────────────────────────
    [MaxLength(2)] public string? CTCONTVADE { get; set; }
    public int? CTCONTPYP { get; set; }   // Numeric 1
    [MaxLength(2)] public string? CTMANUPROD { get; set; }

    // ── Cuentas contables — débitos/créditos ──────────────────────────────
    [MaxLength(40)] public string? PUCDEBRADI { get; set; }
    [MaxLength(40)] public string? FAPRESUDEB { get; set; }
    [MaxLength(40)] public string? FAPRESUCRE { get; set; }
    [MaxLength(40)] public string? PUCDEBCOPA { get; set; }
    [MaxLength(40)] public string? PREDEBCOPA { get; set; }
    [MaxLength(40)] public string? PRECRECOPA { get; set; }
    [MaxLength(40)] public string? PRCRREVACT { get; set; }
    [MaxLength(40)] public string? PRDEREVAFA { get; set; }
    [MaxLength(40)] public string? PRCRREVAFA { get; set; }
    [MaxLength(40)] public string? PRDEREVARE { get; set; }
    [MaxLength(40)] public string? PRCRREVARE { get; set; }
    [MaxLength(40)] public string? PUCDEVA360 { get; set; }
    [MaxLength(40)] public string? PRDEREVEFA { get; set; }
    [MaxLength(40)] public string? PRCRREVEFA { get; set; }
    [MaxLength(40)] public string? PRDEREVERE { get; set; }
    [MaxLength(40)] public string? PRCRREVERE { get; set; }
    [MaxLength(40)] public string? PUCDEGLOSA { get; set; }
    [MaxLength(40)] public string? PUCCRGLOSA { get; set; }
    [MaxLength(40)] public string? PUCDEGLOVA { get; set; }
    [MaxLength(40)] public string? PUCCRGLOVA { get; set; }
    [MaxLength(40)] public string? PCRGLVA360 { get; set; }
    [MaxLength(40)] public string? PUCBANCO { get; set; }
    public int? CTCONTCANFA { get; set; }   // Numeric 3
    [MaxLength(40)] public string? PUCDEBCOUR { get; set; }
    [MaxLength(40)] public string? PUCDEBCOHO { get; set; }

    // ── Facturación y empresa ─────────────────────────────────────────────
    public int? CTCONTFOPA { get; set; }   // Numeric 1
    [MaxLength(3)] public string? FAFEMPCODI { get; set; }
    [MaxLength(150)] public string? CTCONTEMSU { get; set; }
    public int? CTCONTSOAT { get; set; }   // Numeric 1
    [MaxLength(3)] public string? CTCATACODI { get; set; }
}