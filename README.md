# Medisoft API — .NET 8 | Arquitectura Limpia + VFP/OleDb

## 📐 Arquitectura

```
MedisoftAPI.sln
└── src/
    ├── MedisoftAPI.Domain/          → Entidades + Interfaces de repositorio
    ├── MedisoftAPI.Application/     → DTOs + Interfaces de servicio + UseCases
    ├── MedisoftAPI.Infrastructure/  → Repositorios ADO.NET (FoxPro) + EF Core (SQLite)
    └── MedisoftAPI.WebAPI/          → Controllers + Middleware + Program.cs
```

> ⚠️ **CRÍTICO:** `Infrastructure` y `WebAPI` tienen `<PlatformTarget>x86</PlatformTarget>`.
> VFPOLEDB es un proveedor **32-bit** — sin esto la API no arranca.

---

## ✅ Paso 1 — Configurar proyecto de inicio

1. Abre `MedisoftAPI.sln` en **Visual Studio 2022**
2. Clic derecho sobre **`MedisoftAPI.WebAPI`** → **"Establecer como proyecto de inicio"**
3. En la barra de herramientas selecciona el perfil **`http`** o **`https`**
4. Presiona **F5** → Swagger abre en `http://localhost:5050`

---

## ✅ Paso 2 — Bases de datos

El archivo `appsettings.json` ya tiene las rutas configuradas:

| Clave          | Base de datos     | Archivo                                       |
|----------------|-------------------|-----------------------------------------------|
| `FoxPro_Fac`   | Facturación       | `C:\Medisoft\DATOS\facturacion.dbc`           |
| `FoxPro_Gen`   | Generales         | `C:\Medisoft\DATOS\generales.dbc`             |
| `FoxPro_Con`   | Contratación      | `C:\Medisoft\DATOS\contratacion.dbc`          |
| `FoxPro_Cit`   | Citas             | `C:\Medisoft\DATOS\citas.dbc`                 |
| `FoxPro_Adm`   | Admisión          | `C:\Medisoft\DATOS\admision.dbc`              |
| `SQLite`       | Usuarios sistema  | `Database/medisoft_users.db` (auto-creado)    |

> Si alguna ruta es diferente, edítala directamente en `appsettings.json`.

---

## ✅ Paso 3 — Primer arranque

Al presionar **F5** por primera vez:
- Se crea automáticamente `medisoft_users.db` en `Database/`
- Se inserta el usuario **Admin / 123456**
- Swagger abre en `http://localhost:5050`

**No necesitas ejecutar migraciones.**

---

## ⚠️ Requisito VFPOLEDB

El proveedor **Visual FoxPro OLE DB** debe estar instalado en el equipo donde corre la API.

- Descarga: https://www.microsoft.com/en-us/download/details.aspx?id=14839
- Es un instalador de **32 bits** — instálalo aunque el sistema sea 64 bits
- Verifica con `regedit`: `HKLM\SOFTWARE\Classes\VFPOLEDB.1`

---

## 🔐 Autenticación con Swagger

1. Haz clic en **POST /api/auth/login**
2. Ingresa `{ "username": "Admin", "password": "123456" }`
3. Copia el valor del campo `token` de la respuesta
4. Haz clic en **Authorize 🔒** (arriba a la derecha en Swagger)
5. Pega el token y haz clic en **Authorize** (sin escribir "Bearer")
6. Ya puedes usar todos los endpoints protegidos

> El token tiene validez de **8 horas**.

---

## 📋 Endpoints disponibles

### Auth — `/api/Auth`
| Método   | Ruta              | Acceso   | Descripción                    |
|----------|-------------------|----------|-------------------------------|
| `POST`   | `/login`          | Público  | Login → retorna JWT            |
| `GET`    | `/users`          | Admin    | Listar usuarios                |
| `GET`    | `/users/{id}`     | Admin    | Obtener usuario por Id         |
| `POST`   | `/users`          | Admin    | Crear usuario                  |
| `PUT`    | `/users/{id}`     | Admin    | Actualizar usuario             |
| `DELETE` | `/users/{id}`     | Admin    | Eliminar usuario               |

---

### Pacientes — `/api/Pacientes` (BD: `FoxPro_Adm` → admision.dbc)
| Método | Ruta                    | Descripción                    |
|--------|-------------------------|-------------------------------|
| `GET`  | `/`                     | Listar pacientes (paginado)    |
| `GET`  | `/{identificacion}`     | Obtener por número de cédula   |

**Filtros GET `/api/Pacientes`:**
```
?ADTIIDCODI=CC
?ADPACIIDEN=12345678
?ADPACIAPE1=GARCIA
?ADPACIAPE2=LOPEZ
?ADPACINOM1=JUAN
?ADPACINOM2=CARLOS
?Pagina=1&TamPagina=50
```

---

### Admisión Pacientes — `/api/AdmisionPacientes` (BD: `FoxPro_Adm` → admision.dbc)
| Método | Ruta                       | Descripción                                          |
|--------|----------------------------|-----------------------------------------------------|
| `GET`  | `/`                        | Listar admisiones (paginado)                         |
| `GET`  | `/{adadpacons}`            | Obtener por consecutivo                              |
| `GET`  | `/paciente/{adpaciiden}`   | Admisiones enriquecidas (con Contrato + Administ.)   |

El endpoint `/paciente/{id}` retorna `AdadmipaciDetalleDto[]` con los objetos
`Ctcontrato` y `Ctadminist` incluidos en paralelo (`Task.WhenAll`).

**Filtros GET `/api/AdmisionPacientes`:**
```
?ADADPACONS=00000001
?CTADMICODI=001
?ADPACIIDEN=12345678
?CTCONTCODI=A0001
?ADADPAESTA=1
?CTNIVECODI=01
?FechaInicio=2024-01-01&FechaFin=2024-12-31
?Pagina=1&TamPagina=50
```

---

### Citas Pacientes — `/api/CitasPacientes` (BD: `FoxPro_Cit` → citas.dbc)
| Método | Ruta                            | Descripción                                           |
|--------|---------------------------------|------------------------------------------------------|
| `GET`  | `/`                             | Listar citas (paginado)                               |
| `GET`  | `/{adcitacons}`                 | Obtener cita por consecutivo                          |
| `GET`  | `/paciente/{adpaciiden}`        | Citas enriquecidas del paciente (Contrato + Administ.)|
| `POST` | `/desde-disponibilidad`         | Crear cita desde una disponibilidad médica            |

El endpoint `/paciente/{id}` retorna `AdcitasDetalleDto[]` con los objetos
`Ctcontrato` y `Ctadminist` incluidos en paralelo (`Task.WhenAll`).

El endpoint `POST /desde-disponibilidad` replica la lógica del sistema legado:
1. Busca el `Addispmed` por `Addispcons` → toma médico, especialidad, servicio y horario
2. Busca el `Adadmipaci` por `Adadpacons` → toma `Ctadmicodi` y `Ctcontcodi`
3. Genera el consecutivo automáticamente desde la tabla `Consecutivos`
4. Inserta la cita y marca `Addispcita = true` en la disponibilidad

**Filtros GET `/api/CitasPacientes`:**
```
?Adcitacons=00000001
?Geespecodi=01
?Gemedicodi=0001
?Faservcodi=0000001
?Adpaciiden=12345678
?Adconscodi=001
?Adfechcita=2024-06-15
?Ctadmicodi=001
?Ctcontcodi=A0001
?Adingrcons=001
?Pagina=1&TamPagina=50
```

**Body POST `/api/CitasPacientes/desde-disponibilidad`:**
```json
{
  "addispcons": "00001234",
  "adadpacons": "00005678",
  "adpaciiden": "1234567890",
  "faprogcodi": "PYP",
  "tabla": "ADCITAS"
}
```

> **Nota VFP:** Los campos `FECHALLEG` y `ADCITAFEAN` son `Date NOT NULL` en FoxPro.
> Cuando no tienen valor se insertan como `{ / / }` (fecha vacía nativa de VFP), nunca como `NULL`.

---

### Administradoras — `/api/Administradoras` (BD: `FoxPro_Con` → contratacion.dbc)
| Método | Ruta              | Descripción                        |
|--------|-------------------|------------------------------------|
| `GET`  | `/`               | Listar administradoras (paginado)  |
| `GET`  | `/{consecutivo}`  | Obtener por código (CTADMICODI)    |

**Filtros GET `/api/Administradoras`:**
```
?CTADMICODI=001
?CTADMINOMB=SALUD
?CTADMISGSS=123456
?GEDEPACODI=68
?GEMUNICODI=001
?CTADMIESTA=1
?Pagina=1&TamPagina=50
```

---

### Contratos — `/api/Contratos` (BD: `FoxPro_Con` → contratacion.dbc)
| Método | Ruta             | Descripción                    |
|--------|------------------|-------------------------------|
| `GET`  | `/`              | Listar contratos (paginado)    |
| `GET`  | `/{ctcontcodi}`  | Obtener por código de contrato |

**Filtros GET `/api/Contratos`:**
```
?CTCONTCODI=A0001
?CTCONTNUME=2024-001
?CTADMICODI=001
?CTREGICODI=01
?CTCONTESTA=A
?CTTICOCODI=C
?FAFEMPCODI=001
?FechaDesde=2024-01-01&FechaHasta=2024-12-31
?Pagina=1&TamPagina=50
```

---

### Disponibilidad Médica — `/api/Disponibilidad` (BD: `FoxPro_Cit` → citas.dbc)
| Método | Ruta              | Descripción                          |
|--------|-------------------|-------------------------------------|
| `GET`  | `/`               | Listar disponibilidad (paginado)     |
| `GET`  | `/{consecutivo}`  | Obtener por consecutivo (Addispcons) |

> Solo devuelve registros con `addispplan = .T. AND addispcita = .F.`
> Una vez creada la cita desde este registro, `addispcita` pasa a `.T.` automáticamente.

**Filtros GET `/api/Disponibilidad`:**
```
?Addispcons=00001
?Geespecodi=01
?Gemedicodi=0001
?Faservcodi=0000001
?Adconscodi=001
?FechaInicio=2024-01-01&FechaFin=2024-12-31
?Pagina=1&TamPagina=50
```

---

### Especialidades — `/api/Especialidad` (BD: `FoxPro_Gen` → generales.dbc)
| Método | Ruta              | Descripción                      |
|--------|-------------------|----------------------------------|
| `GET`  | `/`               | Listar especialidades (paginado) |
| `GET`  | `/{consecutivo}`  | Obtener por código               |

**Filtros GET `/api/Especialidad`:**
```
?Geespecodi=01
?Geespenomb=MEDICINA
?Pagina=1&TamPagina=50
```

---

### Médicos — `/api/Medicos` (BD: `FoxPro_Gen` → generales.dbc)
| Método | Ruta | Descripción                  |
|--------|------|------------------------------|
| `GET`  | `/`  | Listar médicos (paginado)    |

**Filtros GET `/api/Medicos`:**
```
?Pagina=1&TamPagina=50
```

---

### Servicios Médicos — `/api/Servicios` (BD: `FoxPro_Fac` → facturacion.dbc)
| Método | Ruta | Descripción                    |
|--------|------|-------------------------------|
| `GET`  | `/`  | Listar servicios (paginado)    |

**Filtros GET `/api/Servicios`:**
```
?FASERVCODI=00000001
?FASERVNOMB=CONSULTA
?FASERVESTA=A
?CTCLMACODI=01
?FACLSECODI=02
?FASERVTIPO=1
?Pagina=1&TamPagina=50
```

---

## 🗂️ Mapa completo de la solución

### Domain — Entidades
| Entidad       | Tabla FoxPro    | BD              | Campos clave           |
|---------------|-----------------|-----------------|------------------------|
| `Adpaciente`  | Adpaciente      | FoxPro_Adm      | ADPACIIDEN (20)        |
| `Adadmipaci`  | ADADMIPACI.DBF  | FoxPro_Adm      | ADADPACONS (8)         |
| `Adcitas`     | Adcitas         | FoxPro_Cit      | Adcitacons (8)         |
| `Addispmed`   | Addispmed       | FoxPro_Cit      | Addispcons             |
| `Ctadminist`  | Ctadminist      | FoxPro_Con      | CTADMICODI (3)         |
| `Ctcontrato`  | Ctcontrato      | FoxPro_Con      | CTCONTCODI (5)         |
| `Faservicio`  | FASERVICIO      | FoxPro_Fac      | FASERVCODI             |
| `Geespecial`  | Geespecial      | FoxPro_Gen      | Geespecodi             |
| `Gemedicos`   | Gemedicos       | FoxPro_Gen      | Gemedicodi             |
| `AppUser`     | Users           | SQLite (EF Core)| Id (int, auto)         |

### Infrastructure — Repositorios
| Repositorio              | Conexión       | Patrón lectura          |
|--------------------------|----------------|-------------------------|
| `AdpacienteRepository`   | FoxPro_Adm     | `r["campo"] as string`  |
| `AdadmipaciRepository`   | FoxPro_Adm     | SafeInt / SafeDate      |
| `AdcitasRepository`      | FoxPro_Cit     | SafeString / SafeDate / SafeInt (ordinal) |
| `AddispmedRepository`    | FoxPro_Cit     | `r["campo"] as string`  |
| `CtadministRepository`   | FoxPro_Con     | `r["campo"] as string`  |
| `CtcontratoRepository`   | FoxPro_Con     | SafeDouble / SafeInt ⚠️ |
| `FaservicioRepository`   | FoxPro_Fac     | Convert.ToInt32         |
| `GeespecialRepository`   | FoxPro_Gen     | Convert.ToInt32         |
| `GemedicosRepository`    | FoxPro_Gen     | Convert.ToInt32         |
| `ConsecutivosRepository` | FoxPro_Adm     | Genera y actualiza consecutivos |
| `UserRepository`         | SQLite         | EF Core                 |

### Application — Servicios con dependencias cruzadas
| Servicio           | Dependencias adicionales                                        | Método especial                    |
|--------------------|----------------------------------------------------------------|------------------------------------|
| `AdadmipaciService`| `ICtcontratoRepository`, `ICtadministRepository`               | `GetByPacienteAsync` (Task.WhenAll)|
| `AdcitasService`   | `ICtcontratoRepository`, `ICtadministRepository`, `IAddispmedRepository`, `IAdadmipaciRepository`, `IConsecutivosRepository` | `GetByPacienteAsync`, `CreateFromDispmedAsync` |

---

## 🔧 Guía para agregar una nueva tabla FoxPro

Seguir este orden exacto. Reemplazar `Xxtabla` con el nombre real:

### 1. Domain — Entidad
**`MedisoftAPI.Domain/Entities/Xxtabla.cs`**
```csharp
[Table("Xxtabla")]
public class Xxtabla
{
    [MaxLength(N)] public string? CAMPOCLAVE { get; set; }  // PK
    [MaxLength(N)] public string? CAMPO2     { get; set; }
    public int?      CAMPONUMERIC { get; set; }  // Numeric pequeño → int?
    public double?   CAMPOVALOR   { get; set; }  // Numeric grande  → double?
    public DateTime? CAMPOFECHA   { get; set; }  // Date o DateTime → DateTime?
}
```

> **Regla de tipos VFP → C#:**
> - `Character(n)` → `string?` con `[MaxLength(n)]`
> - `Numeric(1-4)` → `int?`
> - `Numeric(5+)` → `double?` ← **nunca `decimal?`**
> - `Date` → `DateTime?`
> - `DateTime` → `DateTime?`
> - `Logical` → `bool?`

---

### 2. Domain — Interfaz + Filtro
**`MedisoftAPI.Domain/Interfaces/IXxtablaRepository.cs`**
```csharp
public interface IXxtablaRepository
{
    Task<(IEnumerable<Xxtabla> Items, int Total)> GetAllAsync(XxtablaFilter filter);
    Task<Xxtabla?> GetByCodeAsync(string clave);
    Task<Xxtabla>  CreateAsync(Xxtabla entity);
    Task<Xxtabla>  UpdateAsync(Xxtabla entity);
    Task<bool>     DeleteAsync(string clave);
}

public class XxtablaFilter
{
    public int Pagina    { get; set; } = 1;
    public int TamPagina { get; set; } = 50;
    public string?   CAMPOCLAVE  { get; set; }
    public string?   CAMPOBUSCAR { get; set; }
    public int?      CAMPOESTADO { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin    { get; set; }
}
```

---

### 3. Application — DTOs
**`MedisoftAPI.Application/DTOs/XxtablaDtos.cs`**
```csharp
public class XxtablaDto         { /* todos los campos nullable */ }
public class CreateXxtablaDto   { public required string CAMPOCLAVE { get; set; } /* resto opcional */ }
public class UpdateXxtablaDto   { /* sin CAMPOCLAVE — viene por ruta */ }
public class XxtablaQueryDto    { /* filtros + Pagina + TamPagina */ }
```

---

### 4. Application — Interfaz de Servicio
**`MedisoftAPI.Application/Interfaces/IXxtablaService.cs`**
```csharp
public interface IXxtablaService
{
    Task<PagedResult<XxtablaDto>> GetAllAsync(XxtablaQueryDto query);
    Task<XxtablaDto?>  GetByCodeAsync(string clave);
    Task<XxtablaDto>   CreateAsync(CreateXxtablaDto dto);
    Task<XxtablaDto>   UpdateAsync(string clave, UpdateXxtablaDto dto);
    Task<bool>         DeleteAsync(string clave);
}
```

---

### 5. Application — Servicio
**`MedisoftAPI.Application/UseCases/XxtablaService.cs`**
```csharp
public class XxtablaService : IXxtablaService
{
    private readonly IXxtablaRepository _repo;
    public XxtablaService(IXxtablaRepository repo) => _repo = repo;
    // ... GetAllAsync, GetByCodeAsync, CreateAsync, UpdateAsync, DeleteAsync + mappers
}
```

> Si el servicio necesita cruzar datos con otras tablas (como `AdcitasService`),
> inyectar los repositorios adicionales en el constructor y usar `Task.WhenAll` para cargas en paralelo.

---

### 6. Infrastructure — Repositorio
**`MedisoftAPI.Infrastructure/Repositories/XxtablaRepository.cs`**

Plantilla mínima con el patrón correcto para VFP:

```csharp
public class XxtablaRepository : IXxtablaRepository
{
    private readonly string _conn;

    public XxtablaRepository(IConfiguration cfg)
    {
        _conn = cfg.GetConnectionString("FoxPro_XXX")
            ?? throw new InvalidOperationException("Cadena 'FoxPro_XXX' no configurada.");
    }

    // ── Safe helpers OBLIGATORIOS para campos Numeric ─────────────────────
    private static double?   SafeDouble(IDataRecord r, int i) { try { return r.IsDBNull(i) ? null : r.GetDouble(i); }                          catch { return null; } }
    private static int?      SafeInt   (IDataRecord r, int i) { try { return r.IsDBNull(i) ? null : (int?)Convert.ToInt32(r.GetValue(i)); }    catch { return null; } }
    private static DateTime? SafeDate  (IDataRecord r, int i) { try { return r.IsDBNull(i) ? null : (DateTime?)Convert.ToDateTime(r.GetValue(i)); } catch { return null; } }
    private static string    SafeString(IDataRecord r, int i) { try { return r.IsDBNull(i) ? string.Empty : r.GetString(i).TrimEnd(); }        catch { return string.Empty; } }

    private static Xxtabla MapRow(IDataRecord r) => new()
    {
        // comentar índices: 0:CAMPOCLAVE 1:CAMPO2 ...
        CAMPOCLAVE   = SafeString(r, 0),
        CAMPO2       = SafeString(r, 1),
        CAMPONUMERIC = SafeInt(r,    2),
        CAMPOVALOR   = SafeDouble(r, 3),
        CAMPOFECHA   = SafeDate(r,   4),
    };

    private static string SelectColumns() =>
        "CAMPOCLAVE, CAMPO2, CAMPONUMERIC, CAMPOVALOR, CAMPOFECHA";
}
```

> **Regla de fechas en INSERT/UPDATE:**
> - Fechas `Date NOT NULL` → inyectar con `CTOD('MM/dd/yyyy')` o `{ / / }` si es null
> - Nunca pasar `DBNull.Value` a un campo `Date NOT NULL` en VFP — lanza `OleDbException`
> - Fechas opcionales nulas → `{ / / }` (fecha vacía nativa de VFP)

---

### 7. WebAPI — Controller
**`MedisoftAPI.WebAPI/Controllers/XxtablaController.cs`**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class XxtablaController : ControllerBase
{
    private readonly IXxtablaService _service;
    public XxtablaController(IXxtablaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] XxtablaQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<XxtablaDto>>.Ok(result,
            $"Página {result.Pagina} de {result.TotalPaginas} — {result.TotalItems} registros."));
    }

    [HttpGet("{clave}")]
    public async Task<IActionResult> GetByCode(string clave)
    {
        var item = await _service.GetByCodeAsync(clave);
        if (item is null) return NotFound(ApiResponse<string>.Fail($"'{clave}' no encontrado."));
        return Ok(ApiResponse<XxtablaDto>.Ok(item));
    }
}
```

---

### 8. Program.cs — Registrar DI
```csharp
builder.Services.AddScoped<IXxtablaRepository, XxtablaRepository>();
builder.Services.AddScoped<IXxtablaService,    XxtablaService>();
```

---

## 🐛 Errores frecuentes con VFP/OleDb

| Error | Causa | Solución |
|-------|-------|---------|
| `InvalidOperationException: The provider could not determine the Decimal value` | Acceder campo Numeric con `r["campo"]` | Usar `SafeDouble(r, ordinal)` o `SafeInt(r, ordinal)` |
| `OleDbException: Multiple-step OLE DB operation generated errors` | Tipo C# no compatible con columna VFP | Verificar tipos — Numeric grande → `double?`, no `decimal?` |
| `OleDbException: Field FECHALLEG does not accept null values` | Campo `Date NOT NULL` recibe `DBNull` | Inyectar `{ / / }` para fecha vacía o `CTOD('MM/dd/yyyy')` para fecha con valor |
| `The parameter is already contained...` | Reutilizar `OleDbParameter` en 2 comandos | Llamar `MakeParams()` en cada comando — nunca reutilizar |
| `CTOD()` no funciona con `?` | VFP no acepta `DateTime` como parámetro en CTOD | Inyectar fecha como string: `CTOD('{fecha:MM/dd/yyyy}')` |
| `BadImageFormatException` o `PlatformTarget` | VFPOLEDB 32-bit en proceso 64-bit | Verificar `<PlatformTarget>x86</PlatformTarget>` en ambos `.csproj` |
| `Unable to open database file` | SQLite: carpeta `Database/` no existe | Program.cs crea la carpeta automáticamente en el arranque |
| `NullReferenceException` en servicio | Dependencia no asignada en constructor | Verificar que TODOS los parámetros del constructor se asignan a su campo `_campo` |

---

## 📦 Dependencias principales

| Paquete | Versión | Proyecto | Uso |
|---------|---------|----------|-----|
| `System.Data.OleDb` | 8.0.0 | Infrastructure | FoxPro via VFPOLEDB |
| `Microsoft.EntityFrameworkCore.Sqlite` | 8.0.8 | Infrastructure | Usuarios SQLite |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.8 | WebAPI | Auth JWT |
| `Swashbuckle.AspNetCore` | 6.7.3 | WebAPI | Swagger UI |
| `System.IdentityModel.Tokens.Jwt` | 7.3.1 | Application | Generación de tokens |
| `Dapper` | 2.1.28 | Infrastructure | (disponible, no usado aún) |

---

## 🔑 Credenciales por defecto

| Campo    | Valor  |
|----------|--------|
| Usuario  | Admin  |
| Password | 123456 |
| Role     | Admin  |

> Cambiar en producción usando `PUT /api/auth/users/1`.
> El hash es SHA-256 sobre la contraseña en texto plano.
