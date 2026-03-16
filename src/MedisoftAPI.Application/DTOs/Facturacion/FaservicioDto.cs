namespace MedisoftAPI.Application.DTOs.Facturacion;

public class FaservicioDto
{
    public string CTCLMACODI  { get; set; } = string.Empty;
    public string FACLSECODI  { get; set; } = string.Empty;
    public string FASUBCLCODI { get; set; } = string.Empty;
    public string FASERVCODI  { get; set; } = string.Empty;
    public string FASERVNOMB  { get; set; } = string.Empty;
    public int?   FASERVPROG  { get; set; }
    public int?   FASERVCONS  { get; set; }
    public int?   FASERVPART  { get; set; }
    public string FAFISECODI  { get; set; } = string.Empty;
    public int?   FASERVTIPO  { get; set; }
    public int?   FASERVOBS   { get; set; }
    public int?   FASERVPAQU  { get; set; }
    public string FAAGSECODI  { get; set; } = string.Empty;
    public int?   FASERVDUCI  { get; set; }
    public string FASERVADICI { get; set; } = string.Empty;
    public int?   FASERVPRIV  { get; set; }
    public bool?  FASERVINTE  { get; set; }
    public int?   FASERVENFE  { get; set; }
    public int?   FASERVFREC  { get; set; }
    public int?   FASERVTRAN  { get; set; }
    public int?   FAESVAC     { get; set; }
    public int?   FASERVTRAP  { get; set; }
    public int?   FASERVTRAS  { get; set; }
    public int?   FASERVRX    { get; set; }
    public int?   FAESTERAPI  { get; set; }
    public string FASERVESTA  { get; set; } = string.Empty;
    public int?   FASERV2175  { get; set; }
    public int?   FAINCAPCID  { get; set; }
}

public class CreateFaservicioDto
{
    public string CTCLMACODI  { get; set; } = string.Empty;
    public string FACLSECODI  { get; set; } = string.Empty;
    public string FASUBCLCODI { get; set; } = string.Empty;
    public string FASERVCODI  { get; set; } = string.Empty;
    public string FASERVNOMB  { get; set; } = string.Empty;
    public int?   FASERVPROG  { get; set; }
    public int?   FASERVCONS  { get; set; }
    public int?   FASERVPART  { get; set; }
    public string FAFISECODI  { get; set; } = string.Empty;
    public int?   FASERVTIPO  { get; set; }
    public int?   FASERVOBS   { get; set; }
    public int?   FASERVPAQU  { get; set; }
    public string FAAGSECODI  { get; set; } = string.Empty;
    public int?   FASERVDUCI  { get; set; }
    public string FASERVADICI { get; set; } = string.Empty;
    public int?   FASERVPRIV  { get; set; }
    public bool?  FASERVINTE  { get; set; }
    public int?   FASERVENFE  { get; set; }
    public int?   FASERVFREC  { get; set; }
    public int?   FASERVTRAN  { get; set; }
    public int?   FAESVAC     { get; set; }
    public int?   FASERVTRAP  { get; set; }
    public int?   FASERVTRAS  { get; set; }
    public int?   FASERVRX    { get; set; }
    public int?   FAESTERAPI  { get; set; }
    public string FASERVESTA  { get; set; } = "A";
    public int?   FASERV2175  { get; set; }
    public int?   FAINCAPCID  { get; set; }
}

public class UpdateFaservicioDto
{
    public string CTCLMACODI  { get; set; } = string.Empty;
    public string FACLSECODI  { get; set; } = string.Empty;
    public string FASUBCLCODI { get; set; } = string.Empty;
    public string FASERVNOMB  { get; set; } = string.Empty;
    public int?   FASERVPROG  { get; set; }
    public int?   FASERVCONS  { get; set; }
    public int?   FASERVPART  { get; set; }
    public string FAFISECODI  { get; set; } = string.Empty;
    public int?   FASERVTIPO  { get; set; }
    public int?   FASERVOBS   { get; set; }
    public int?   FASERVPAQU  { get; set; }
    public string FAAGSECODI  { get; set; } = string.Empty;
    public int?   FASERVDUCI  { get; set; }
    public string FASERVADICI { get; set; } = string.Empty;
    public int?   FASERVPRIV  { get; set; }
    public bool?  FASERVINTE  { get; set; }
    public int?   FASERVENFE  { get; set; }
    public int?   FASERVFREC  { get; set; }
    public int?   FASERVTRAN  { get; set; }
    public int?   FAESVAC     { get; set; }
    public int?   FASERVTRAP  { get; set; }
    public int?   FASERVTRAS  { get; set; }
    public int?   FASERVRX    { get; set; }
    public int?   FAESTERAPI  { get; set; }
    public string FASERVESTA  { get; set; } = string.Empty;
    public int?   FASERV2175  { get; set; }
    public int?   FAINCAPCID  { get; set; }
}

/// <summary>Filtros + paginación para la consulta de servicios</summary>
public class FaservicioQueryDto
{
    public string? FASERVCODI { get; set; }
    public string? FASERVNOMB { get; set; }
    public string? FASERVESTA { get; set; }
    public string? CTCLMACODI { get; set; }
    public string? FACLSECODI { get; set; }
    public int?    FASERVTIPO { get; set; }

    // Paginación
    public int Pagina    { get; set; } = 1;
    public int TamPagina { get; set; } = 50;   // 50 registros por defecto
}

/// <summary>Respuesta paginada genérica</summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items      { get; set; } = [];
    public int            Pagina     { get; set; }
    public int            TamPagina  { get; set; }
    public int            TotalItems { get; set; }
    public int            TotalPaginas => (int)Math.Ceiling((double)TotalItems / TamPagina);
}
