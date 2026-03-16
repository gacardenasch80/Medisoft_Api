using MedisoftAPI.Application.DTOs.Contratacion;

namespace MedisoftAPI.Application.DTOs.Citas;

public class AdcitasDto
{
    public string Adcitacons { get; set; }
    public string Geespecodi { get; set; }
    public string Gemedicodi { get; set; }
    public string Faservcodi { get; set; }
    public string Adpaciiden { get; set; }
    public string Adconscodi { get; set; }
    public DateTime Adfechcita { get; set; }
    public string Adhorainic { get; set; }
    public string Adhorafina { get; set; }
    public int? Adduraminu { get; set; }
    public string Adconsdisp { get; set; }
    public string Adcitaest { get; set; }
    public string Adanulcodi { get; set; }
    public string Ctadmicodi { get; set; }
    public string Ctcontcodi { get; set; }
    public DateTime Fechasoli { get; set; }
    public string Geusuacreo { get; set; }
    public string Adingrcons { get; set; }
    public string Faorsecons { get; set; }
    public string Coconscons { get; set; }
    public string Faprogcodi { get; set; }
    public DateTime? Fechalleg { get; set; }
    public string Adcodanula { get; set; }
    public DateTime Fechprefpa { get; set; }
    public string Adcitaande { get; set; }
    public string Geusuacoan { get; set; }
    public DateTime? Adcitafean { get; set; }
    public string Adciticodi { get; set; }
}

public class AdcitasQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    public string? Adcitacons { get; set; }
    public string? Geespecodi { get; set; }
    public string? Gemedicodi { get; set; }
    public string? Faservcodi { get; set; }
    public string? Adpaciiden { get; set; }
    public string? Adconscodi { get; set; }
    public DateTime? Adfechcita { get; set; }
    public string? Ctadmicodi { get; set; }
    public string? Ctcontcodi { get; set; }
    public string? Adingrcons { get; set; }
}

public class CreateAdcitasDto
{
    public required string ADADPACONS { get; set; }
    public string Geespecodi { get; set; }
    public string Gemedicodi { get; set; }
    public string Faservcodi { get; set; }
    public string Adpaciiden { get; set; }
    public string Adconscodi { get; set; }
    public DateTime Adfechcita { get; set; }
    public string Adhorainic { get; set; }
    public string Adhorafina { get; set; }
    public int? Adduraminu { get; set; }
    public string Adconsdisp { get; set; }
    public string Adcitaest { get; set; }
    public string Adanulcodi { get; set; }
    public string Ctadmicodi { get; set; }
    public string Ctcontcodi { get; set; }
    public DateTime Fechasoli { get; set; }
    public string Geusuacreo { get; set; }
    public string Adingrcons { get; set; }
    public string Faorsecons { get; set; }
    public string Coconscons { get; set; }
    public string Faprogcodi { get; set; }
    public DateTime? Fechalleg { get; set; }
    public string Adcodanula { get; set; }
    public DateTime Fechprefpa { get; set; }
    public string Adcitaande { get; set; }
    public string Geusuacoan { get; set; }
    public DateTime? Adcitafean { get; set; }
    public string Adciticodi { get; set; }
}

public class UpdateAdcitasDto
{
    public string Geespecodi { get; set; }
    public string Gemedicodi { get; set; }
    public string Faservcodi { get; set; }
    public string Adpaciiden { get; set; }
    public string Adconscodi { get; set; }
    public DateTime Adfechcita { get; set; }
    public string Adhorainic { get; set; }
    public string Adhorafina { get; set; }
    public int? Adduraminu { get; set; }
    public string Adconsdisp { get; set; }
    public string Adcitaest { get; set; }
    public string Adanulcodi { get; set; }
    public string Ctadmicodi { get; set; }
    public string Ctcontcodi { get; set; }
    public DateTime Fechasoli { get; set; }
    public string Geusuacreo { get; set; }
    public string Adingrcons { get; set; }
    public string Faorsecons { get; set; }
    public string Coconscons { get; set; }
    public string Faprogcodi { get; set; }
    public DateTime? Fechalleg { get; set; }
    public string Adcodanula { get; set; }
    public DateTime Fechprefpa { get; set; }
    public string Adcitaande { get; set; }
    public string Geusuacoan { get; set; }
    public DateTime? Adcitafean { get; set; }
    public string Adciticodi { get; set; }
}
/// <summary>
/// Admisión enriquecida — incluye el contrato y la administradora relacionados
/// </summary>
public class AdcitasDetalleDto
{
    public AdcitasDto Admision { get; set; } = null!;
    public CtcontratoDto? Contrato { get; set; }
    public CtadministDto? Administradora { get; set; }
}

/// <summary>
/// DTO para crear una cita a partir de una disponibilidad médica (Addispmed).
/// Replica la lógica del método CreateAdcitas del sistema legado:
///   1. Busca el Addispmed por Addispcons → toma médico, especialidad, servicio, fechas y horas
///   2. Busca el Adadmipaci por Adadpacons → toma Ctadmicodi y Ctcontcodi
///   3. Genera el consecutivo desde la tabla Consecutivos usando el campo Tabla
///   4. Inserta la cita y marca Addispcita = true en el Addispmed
/// </summary>
public class CreateAdcitasFromDispmedDto
{
    /// <summary>Consecutivo de la disponibilidad médica (Addispmed.Addispcons)</summary>
    public required string Addispcons { get; set; }

    /// <summary>Consecutivo de la admisión del paciente (Adadmipaci.Adadpacons)</summary>
    public required string Adadpacons { get; set; }

    /// <summary>Identificación del paciente</summary>
    public required string Adpaciiden { get; set; }

    /// <summary>Código del programa (Faprogcodi)</summary>
    public string Faprogcodi { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la tabla en Consecutivos para generar el número de cita.
    /// Ejemplo: "ADCITAS"
    /// </summary>
    public required string Tabla { get; set; }
}