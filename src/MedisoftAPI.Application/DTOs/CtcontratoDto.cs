namespace MedisoftAPI.Application.DTOs;

// ════════════════════════════════════════════════════════════════
//  CtcontratoDto  —  Respuesta al cliente
// ════════════════════════════════════════════════════════════════
public class CtcontratoDto
{
    public string? CTCONTCODI { get; set; }
    public string? CTCONTNUME { get; set; }
    public string? CTADMICODI { get; set; }
    public string? CTREGICODI { get; set; }
    public string? CTPLBECODI { get; set; }
    public string? CTMANUCODI { get; set; }
    public string? CTCONTLEGA { get; set; }
    public DateTime? CTCONTFELE { get; set; }
    public DateTime? CTCONTDESD { get; set; }
    public DateTime? CTCONTHAST { get; set; }
    public double? CTCONTVALO { get; set; }
    public string? CTCONTESTA { get; set; }
    public string? CTTICOCODI { get; set; }
    public double? CTCONTASIG { get; set; }
    public double? CTCONTFACT { get; set; }
    public int? FAOBSHORA { get; set; }
    public string? CTCTAACRE { get; set; }
    public int? CTCITXDIA { get; set; }
    public int? CTCONTCONS { get; set; }
    public DateTime? CTCONTFECR { get; set; }
    public string? CTCONTUSCR { get; set; }
    public DateTime? CTCONTFEED { get; set; }
    public string? CTCONTUSED { get; set; }
    public string? CTCONTVADE { get; set; }
    public int? CTCONTPYP { get; set; }
    public string? CTMANUPROD { get; set; }
    public string? PUCDEBRADI { get; set; }
    public string? FAPRESUDEB { get; set; }
    public string? FAPRESUCRE { get; set; }
    public string? PUCDEBCOPA { get; set; }
    public string? PREDEBCOPA { get; set; }
    public string? PRECRECOPA { get; set; }
    public string? PRCRREVACT { get; set; }
    public string? PRDEREVAFA { get; set; }
    public string? PRCRREVAFA { get; set; }
    public string? PRDEREVARE { get; set; }
    public string? PRCRREVARE { get; set; }
    public string? PUCDEVA360 { get; set; }
    public string? PRDEREVEFA { get; set; }
    public string? PRCRREVEFA { get; set; }
    public string? PRDEREVERE { get; set; }
    public string? PRCRREVERE { get; set; }
    public string? PUCDEGLOSA { get; set; }
    public string? PUCCRGLOSA { get; set; }
    public string? PUCDEGLOVA { get; set; }
    public string? PUCCRGLOVA { get; set; }
    public string? PCRGLVA360 { get; set; }
    public string? PUCBANCO { get; set; }
    public int? CTCONTCANFA { get; set; }
    public string? PUCDEBCOUR { get; set; }
    public string? PUCDEBCOHO { get; set; }
    public int? CTCONTFOPA { get; set; }
    public string? FAFEMPCODI { get; set; }
    public string? CTCONTEMSU { get; set; }
    public int? CTCONTSOAT { get; set; }
    public string? CTCATACODI { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  CtcontratoQueryDto  —  Parámetros búsqueda + paginación
// ════════════════════════════════════════════════════════════════
public class CtcontratoQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    public string? CTCONTCODI { get; set; }
    public string? CTCONTNUME { get; set; }
    public string? CTADMICODI { get; set; }
    public string? CTREGICODI { get; set; }
    public string? CTCONTESTA { get; set; }
    public string? CTTICOCODI { get; set; }
    public string? FAFEMPCODI { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  CreateCtcontratoDto  —  Payload para POST
// ════════════════════════════════════════════════════════════════
public class CreateCtcontratoDto
{
    public required string CTCONTCODI { get; set; }
    public string? CTCONTNUME { get; set; }
    public string? CTADMICODI { get; set; }
    public string? CTREGICODI { get; set; }
    public string? CTPLBECODI { get; set; }
    public string? CTMANUCODI { get; set; }
    public string? CTCONTLEGA { get; set; }
    public DateTime? CTCONTFELE { get; set; }
    public DateTime? CTCONTDESD { get; set; }
    public DateTime? CTCONTHAST { get; set; }
    public double? CTCONTVALO { get; set; }
    public string? CTCONTESTA { get; set; }
    public string? CTTICOCODI { get; set; }
    public double? CTCONTASIG { get; set; }
    public double? CTCONTFACT { get; set; }
    public int? FAOBSHORA { get; set; }
    public string? CTCTAACRE { get; set; }
    public int? CTCITXDIA { get; set; }
    public int? CTCONTCONS { get; set; }
    public string? CTCONTVADE { get; set; }
    public int? CTCONTPYP { get; set; }
    public string? CTMANUPROD { get; set; }
    public string? PUCDEBRADI { get; set; }
    public string? FAPRESUDEB { get; set; }
    public string? FAPRESUCRE { get; set; }
    public string? PUCDEBCOPA { get; set; }
    public string? PREDEBCOPA { get; set; }
    public string? PRECRECOPA { get; set; }
    public string? PRCRREVACT { get; set; }
    public string? PRDEREVAFA { get; set; }
    public string? PRCRREVAFA { get; set; }
    public string? PRDEREVARE { get; set; }
    public string? PRCRREVARE { get; set; }
    public string? PUCDEVA360 { get; set; }
    public string? PRDEREVEFA { get; set; }
    public string? PRCRREVEFA { get; set; }
    public string? PRDEREVERE { get; set; }
    public string? PRCRREVERE { get; set; }
    public string? PUCDEGLOSA { get; set; }
    public string? PUCCRGLOSA { get; set; }
    public string? PUCDEGLOVA { get; set; }
    public string? PUCCRGLOVA { get; set; }
    public string? PCRGLVA360 { get; set; }
    public string? PUCBANCO { get; set; }
    public int? CTCONTCANFA { get; set; }
    public string? PUCDEBCOUR { get; set; }
    public string? PUCDEBCOHO { get; set; }
    public int? CTCONTFOPA { get; set; }
    public string? FAFEMPCODI { get; set; }
    public string? CTCONTEMSU { get; set; }
    public int? CTCONTSOAT { get; set; }
    public string? CTCATACODI { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  UpdateCtcontratoDto  —  Payload para PUT
//  (CTCONTCODI viene por ruta, no en el body)
// ════════════════════════════════════════════════════════════════
public class UpdateCtcontratoDto
{
    public string? CTCONTNUME { get; set; }
    public string? CTADMICODI { get; set; }
    public string? CTREGICODI { get; set; }
    public string? CTPLBECODI { get; set; }
    public string? CTMANUCODI { get; set; }
    public string? CTCONTLEGA { get; set; }
    public DateTime? CTCONTFELE { get; set; }
    public DateTime? CTCONTDESD { get; set; }
    public DateTime? CTCONTHAST { get; set; }
    public double? CTCONTVALO { get; set; }
    public string? CTCONTESTA { get; set; }
    public string? CTTICOCODI { get; set; }
    public double? CTCONTASIG { get; set; }
    public double? CTCONTFACT { get; set; }
    public int? FAOBSHORA { get; set; }
    public string? CTCTAACRE { get; set; }
    public int? CTCITXDIA { get; set; }
    public int? CTCONTCONS { get; set; }
    public string? CTCONTVADE { get; set; }
    public int? CTCONTPYP { get; set; }
    public string? CTMANUPROD { get; set; }
    public string? PUCDEBRADI { get; set; }
    public string? FAPRESUDEB { get; set; }
    public string? FAPRESUCRE { get; set; }
    public string? PUCDEBCOPA { get; set; }
    public string? PREDEBCOPA { get; set; }
    public string? PRECRECOPA { get; set; }
    public string? PRCRREVACT { get; set; }
    public string? PRDEREVAFA { get; set; }
    public string? PRCRREVAFA { get; set; }
    public string? PRDEREVARE { get; set; }
    public string? PRCRREVARE { get; set; }
    public string? PUCDEVA360 { get; set; }
    public string? PRDEREVEFA { get; set; }
    public string? PRCRREVEFA { get; set; }
    public string? PRDEREVERE { get; set; }
    public string? PRCRREVERE { get; set; }
    public string? PUCDEGLOSA { get; set; }
    public string? PUCCRGLOSA { get; set; }
    public string? PUCDEGLOVA { get; set; }
    public string? PUCCRGLOVA { get; set; }
    public string? PCRGLVA360 { get; set; }
    public string? PUCBANCO { get; set; }
    public int? CTCONTCANFA { get; set; }
    public string? PUCDEBCOUR { get; set; }
    public string? PUCDEBCOHO { get; set; }
    public int? CTCONTFOPA { get; set; }
    public string? FAFEMPCODI { get; set; }
    public string? CTCONTEMSU { get; set; }
    public int? CTCONTSOAT { get; set; }
    public string? CTCATACODI { get; set; }
}