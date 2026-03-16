using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedisoftAPI.Domain.Entities.Contratacion;

/// <summary>
/// Espejo de la tabla FoxPro Ctadminist
/// Longitud total del registro: 719 bytes aprox.
/// </summary>
[Table("Ctadminist")]
public class Ctadminist
{
    // ── Identificación ────────────────────────────────────────────────────

    /// <summary>Código administradora (3)</summary>
    [MaxLength(3)]
    public string? CTADMICODI { get; set; }

    /// <summary>Nombre administradora (100)</summary>
    [MaxLength(100)]
    public string? CTADMINOMB { get; set; }

    /// <summary>Iniciales (15)</summary>
    [MaxLength(15)]
    public string? CTADMINIT { get; set; }

    /// <summary>Código SGSS (6)</summary>
    [MaxLength(6)]
    public string? CTADMISGSS { get; set; }

    // ── Contacto ──────────────────────────────────────────────────────────

    /// <summary>Dirección (40)</summary>
    [MaxLength(40)]
    public string? CTADMIDIRE { get; set; }

    /// <summary>Teléfono (25)</summary>
    [MaxLength(25)]
    public string? CTADMITELE { get; set; }

    /// <summary>Representante legal (30)</summary>
    [MaxLength(30)]
    public string? CTADMIRELE { get; set; }

    /// <summary>Correo electrónico (150)</summary>
    [MaxLength(150)]
    public string? CTADMIEMAI { get; set; }

    /// <summary>Página web (30)</summary>
    [MaxLength(30)]
    public string? CTADMIPAGI { get; set; }

    // ── Ubicación geográfica ──────────────────────────────────────────────

    /// <summary>Código departamento (2)</summary>
    [MaxLength(2)]
    public string? GEDEPACODI { get; set; }

    /// <summary>Código municipio (3)</summary>
    [MaxLength(3)]
    public string? GEMUNICODI { get; set; }

    // ── Contrato y facturación ────────────────────────────────────────────

    /// <summary>Código contrato copia (5)</summary>
    [MaxLength(5)]
    public string? CTCONCOPYP { get; set; }

    /// <summary>Indicador GID (Numeric 1)</summary>
    public int? CTADMIGIDI { get; set; }

    /// <summary>Código fecha recepción (8)</summary>
    [MaxLength(8)]
    public string? FAFERECODI { get; set; }

    /// <summary>Código tipo factura (2)</summary>
    [MaxLength(2)]
    public string? FAFETIFCOD { get; set; }

    // ── Clasificación y estado ────────────────────────────────────────────

    /// <summary>Tipo administradora (Numeric 2)</summary>
    public int? CTADMTYPER { get; set; }

    /// <summary>Código ruta (2)</summary>
    [MaxLength(2)]
    public string? CTADMRERUT { get; set; }

    /// <summary>Estado: 1 = activo, 0 = inactivo (Numeric 1)</summary>
    public int? CTADMIESTA { get; set; }

    // ── Observaciones ─────────────────────────────────────────────────────

    /// <summary>Firma/observaciones SGSS farmacia (250)</summary>
    [MaxLength(250)]
    public string? CTADMSGFAR { get; set; }
}