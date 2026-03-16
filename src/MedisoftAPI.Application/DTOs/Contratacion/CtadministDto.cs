namespace MedisoftAPI.Application.DTOs.Contratacion;

// ════════════════════════════════════════════════════════════════
//  CtadministDto  —  Respuesta al cliente
// ════════════════════════════════════════════════════════════════
public class CtadministDto
{
    public string? CTADMICODI { get; set; }
    public string? CTADMINOMB { get; set; }
    public string? CTADMINIT { get; set; }
    public string? CTADMISGSS { get; set; }
    public string? CTADMIDIRE { get; set; }
    public string? CTADMITELE { get; set; }
    public string? CTADMIRELE { get; set; }
    public string? CTADMIEMAI { get; set; }
    public string? CTADMIPAGI { get; set; }
    public string? GEDEPACODI { get; set; }
    public string? GEMUNICODI { get; set; }
    public string? CTCONCOPYP { get; set; }
    public int? CTADMIGIDI { get; set; }
    public string? FAFERECODI { get; set; }
    public string? FAFETIFCOD { get; set; }
    public int? CTADMTYPER { get; set; }
    public string? CTADMRERUT { get; set; }
    public int? CTADMIESTA { get; set; }
    public string? CTADMSGFAR { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  CtadministQueryDto  —  Parámetros de búsqueda + paginación
// ════════════════════════════════════════════════════════════════
public class CtadministQueryDto
{
    public int Pagina { get; set; } = 1;
    public int TamPagina { get; set; } = 50;

    /// <summary>Código administradora exacto</summary>
    public string? CTADMICODI { get; set; }

    /// <summary>Nombre administradora (búsqueda parcial)</summary>
    public string? CTADMINOMB { get; set; }

    /// <summary>Código SGSS exacto</summary>
    public string? CTADMISGSS { get; set; }

    /// <summary>Código departamento exacto</summary>
    public string? GEDEPACODI { get; set; }

    /// <summary>Código municipio exacto</summary>
    public string? GEMUNICODI { get; set; }

    /// <summary>Estado: 1 = activo, 0 = inactivo</summary>
    public int? CTADMIESTA { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  CreateCtadministDto  —  Payload para POST
// ════════════════════════════════════════════════════════════════
public class CreateCtadministDto
{
    public required string CTADMICODI { get; set; }
    public required string CTADMINOMB { get; set; }
    public string? CTADMINIT { get; set; }
    public string? CTADMISGSS { get; set; }
    public string? CTADMIDIRE { get; set; }
    public string? CTADMITELE { get; set; }
    public string? CTADMIRELE { get; set; }
    public string? CTADMIEMAI { get; set; }
    public string? CTADMIPAGI { get; set; }
    public string? GEDEPACODI { get; set; }
    public string? GEMUNICODI { get; set; }
    public string? CTCONCOPYP { get; set; }
    public int? CTADMIGIDI { get; set; }
    public string? FAFERECODI { get; set; }
    public string? FAFETIFCOD { get; set; }
    public int? CTADMTYPER { get; set; }
    public string? CTADMRERUT { get; set; }
    public int? CTADMIESTA { get; set; }
    public string? CTADMSGFAR { get; set; }
}

// ════════════════════════════════════════════════════════════════
//  UpdateCtadministDto  —  Payload para PUT
//  (CTADMICODI viene por ruta, no en el body)
// ════════════════════════════════════════════════════════════════
public class UpdateCtadministDto
{
    public string? CTADMINOMB { get; set; }
    public string? CTADMINIT { get; set; }
    public string? CTADMISGSS { get; set; }
    public string? CTADMIDIRE { get; set; }
    public string? CTADMITELE { get; set; }
    public string? CTADMIRELE { get; set; }
    public string? CTADMIEMAI { get; set; }
    public string? CTADMIPAGI { get; set; }
    public string? GEDEPACODI { get; set; }
    public string? GEMUNICODI { get; set; }
    public string? CTCONCOPYP { get; set; }
    public int? CTADMIGIDI { get; set; }
    public string? FAFERECODI { get; set; }
    public string? FAFETIFCOD { get; set; }
    public int? CTADMTYPER { get; set; }
    public string? CTADMRERUT { get; set; }
    public int? CTADMIESTA { get; set; }
    public string? CTADMSGFAR { get; set; }
}