using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedisoftAPI.Domain.Entities
{
    /// <summary>
    /// Entidad generada desde estructura de tabla FoxPro.
    /// Longitud total del registro: 743 bytes.
    /// </summary>
    [Table("Adpaciente")] // Ajusta el nombre de la tabla según corresponda
    public class Adpaciente
    {
        // ── IDENTIFICACIÓN TIPO DOCUMENTO ─────────────────────────────────

        /// <summary>Código tipo de identificación (2)</summary>
        [MaxLength(2)]
        public string ADTIIDCODI { get; set; }

        /// <summary>Identificación del paciente (20)</summary>
        [MaxLength(20)]
        public string ADPACIIDEN { get; set; }

        /// <summary>Código tipo de identificación CAFA (2)</summary>
        [MaxLength(2)]
        public string ADTIIDCAFA { get; set; }

        /// <summary>Identificación CAFA (20)</summary>
        [MaxLength(20)]
        public string ADCAFAIDEN { get; set; }

        // ── DATOS PERSONALES ──────────────────────────────────────────────

        /// <summary>Primer apellido del paciente (30)</summary>
        [MaxLength(30)]
        public string ADPACIAPE1 { get; set; }

        /// <summary>Segundo apellido del paciente (30)</summary>
        [MaxLength(30)]
        public string ADPACIAPE2 { get; set; }

        /// <summary>Primer nombre del paciente (20)</summary>
        [MaxLength(20)]
        public string ADPACINOM1 { get; set; }

        /// <summary>Segundo nombre del paciente (20)</summary>
        [MaxLength(20)]
        public string ADPACINOM2 { get; set; }

        /// <summary>Fecha de nacimiento (Date 8)</summary>
        public DateTime? ADPACIFENA { get; set; }

        /// <summary>Sexo del paciente (1) — M/F</summary>
        [MaxLength(1)]
        public string ADPACISEXO { get; set; }

        // ── UBICACIÓN GEOGRÁFICA ──────────────────────────────────────────

        /// <summary>Zona del barrio (1)</summary>
        [MaxLength(1)]
        public string GEBARRZONA { get; set; }

        /// <summary>Código departamento (2)</summary>
        [MaxLength(2)]
        public string GEDEPACODI { get; set; }

        /// <summary>Código municipio (3)</summary>
        [MaxLength(3)]
        public string GEMUNICODI { get; set; }

        /// <summary>Código barrio (3)</summary>
        [MaxLength(3)]
        public string GEBARRCODI { get; set; }

        /// <summary>Código departamento alterno (2)</summary>
        [MaxLength(2)]
        public string GEDEPACOD1 { get; set; }

        /// <summary>Código municipio alterno (3)</summary>
        [MaxLength(3)]
        public string GEMUNICOD1 { get; set; }

        // ── RÉGIMEN Y NIVEL ───────────────────────────────────────────────

        /// <summary>Código régimen (2)</summary>
        [MaxLength(2)]
        public string CTREGICODI { get; set; }

        /// <summary>Dirección del paciente (40)</summary>
        [MaxLength(40)]
        public string ADPACIDIRE { get; set; }

        /// <summary>Teléfono del paciente (25)</summary>
        [MaxLength(25)]
        public string ADPACITELE { get; set; }

        /// <summary>Código nivel de atención (2)</summary>
        [MaxLength(2)]
        public string CTNIVECODI { get; set; }

        // ── HISTORIA CLÍNICA Y AFILIACIÓN ─────────────────────────────────

        /// <summary>Número de historia clínica (20)</summary>
        [MaxLength(20)]
        public string ADPACIHIST { get; set; }

        /// <summary>Fecha de ingreso a SSS (Date 8)</summary>
        public DateTime? ADFEINSGSS { get; set; }

        /// <summary>Fecha de ingreso del paciente (Date 8)</summary>
        public DateTime? ADPACIFEIN { get; set; }

        /// <summary>Código tipo de afiliación (2)</summary>
        [MaxLength(2)]
        public string ADTIAFCODI { get; set; }

        /// <summary>Estado del paciente (1) — A/I</summary>
        [MaxLength(1)]
        public string ADESTACODI { get; set; }

        // ── DATOS FAMILIARES ──────────────────────────────────────────────

        /// <summary>Nombre del padre (30)</summary>
        [MaxLength(30)]
        public string ADPACIPADR { get; set; }

        /// <summary>Nombre de la madre (30)</summary>
        [MaxLength(30)]
        public string ADPACIMADR { get; set; }

        // ── CONTACTO ─────────────────────────────────────────────────────

        /// <summary>Celular del paciente (10)</summary>
        [MaxLength(10)]
        public string ADPACICELU { get; set; }

        /// <summary>Correo electrónico del paciente (100)</summary>
        [MaxLength(100)]
        public string ADPACIMAIL { get; set; }

        /// <summary>Hectáreas/código especial (3)</summary>
        [MaxLength(3)]
        public string ADPACIHECL { get; set; }

        // ── CÓDIGOS GEOGRÁFICOS ADICIONALES ──────────────────────────────

        /// <summary>Código escuela (2)</summary>
        [MaxLength(2)]
        public string GECODIESC { get; set; }

        /// <summary>Código PECT (2)</summary>
        [MaxLength(2)]
        public string GECODIPECT { get; set; }

        /// <summary>Código OCGG (1)</summary>
        [MaxLength(1)]
        public string GECODIOCGG { get; set; }

        /// <summary>Código OSGP (2)</summary>
        [MaxLength(2)]
        public string GECODIOSGP { get; set; }

        /// <summary>Código OCSG (3)</summary>
        [MaxLength(3)]
        public string GECODIOCSG { get; set; }

        /// <summary>Código OCGP (4)</summary>
        [MaxLength(4)]
        public string GECODIOCGP { get; set; }

        // ── CÓDIGOS DE CLASIFICACIÓN ──────────────────────────────────────

        /// <summary>Código de población (2)</summary>
        [MaxLength(2)]
        public string CODIGOPOB { get; set; }

        /// <summary>Código nutricional (2)</summary>
        [MaxLength(2)]
        public string CODIGONUT { get; set; }

        // ── OBSERVACIONES Y REFERENCIAS ───────────────────────────────────

        /// <summary>Observaciones del paciente (240)</summary>
        [MaxLength(240)]
        public string ADPACIOBSE { get; set; }

        /// <summary>Identificación de referencia del paciente (20)</summary>
        [MaxLength(20)]
        public string ADPACIIDRE { get; set; }

        // ── FACTURACIÓN ───────────────────────────────────────────────────

        /// <summary>Código fecha recepción factura (8)</summary>
        [MaxLength(8)]
        public string FAFERECODI { get; set; }

        /// <summary>Código tipo de factura (2)</summary>
        [MaxLength(2)]
        public string FAFETIFCOD { get; set; }

        // ── CAMPOS NUMÉRICOS Y ADICIONALES ───────────────────────────────

        /// <summary>Tipo de paciente/rutero (Numeric 2)</summary>
        public int? ADPACTYPER { get; set; }

        /// <summary>Código de ruta del paciente (2)</summary>
        [MaxLength(2)]
        public string ADPACRERUT { get; set; }

        /// <summary>Código clasificación clínica (2)</summary>
        [MaxLength(2)]
        public string ADCLDICODI { get; set; }
    }
}
