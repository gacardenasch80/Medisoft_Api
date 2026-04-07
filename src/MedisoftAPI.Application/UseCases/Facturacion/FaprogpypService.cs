using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.DTOs.Facturacion;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Entities.Facturacion;
using MedisoftAPI.Domain.Interfaces;
using MedisoftAPI.Domain.Interfaces.Admision;
using MedisoftAPI.Domain.Interfaces.Facturacion;

namespace MedisoftAPI.Application.UseCases;

public class FaprogpypService : IFaprogpypService
{
    private readonly IFaprogpypRepository _repo;
    private readonly IAdpacienteRepository _repoPaciente;

    public FaprogpypService(
        IFaprogpypRepository repo,
        IAdpacienteRepository repoPaciente)
    {
        _repo = repo;
        _repoPaciente = repoPaciente;
    }

    public async Task<PagedResult<FaprogpypDto>> GetAllAsync(FaprogpypQueryDto query)
    {
        var filter = new FaprogpypFilter
        {
            Faprogcodi = query.Faprogcodi,
            Faprogcod1 = query.Faprogcod1,
            Faprognomb = query.Faprognomb,
            Faprogclas = query.Faprogclas,
            Faservcodi = query.Faservcodi,
            Faprogestad = query.Faprogestad,
            Pagina = query.Pagina < 1 ? 1 : query.Pagina,
            TamPagina = query.TamPagina < 1 ? 50 :
                          query.TamPagina > 200 ? 200 : query.TamPagina,
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<FaprogpypDto>
        {
            Items = items.Select(ToDto),
            Pagina = filter.Pagina,
            TamPagina = filter.TamPagina,
            TotalItems = total,
        };
    }

    public async Task<FaprogpypDto?> GetByCodeAsync(string faprogcodi)
    {
        var e = await _repo.GetByCodeAsync(faprogcodi);
        return e is null ? null : ToDto(e);
    }

    // ── GET BY PACIENTE ───────────────────────────────────────────────────
    // Replica la lógica FoxPro:
    //   EdadAnnos = (FechaReferencia - FechaNacPaciente) / 365
    //   BETWEEN(edad, faprogdesd, faproghast)
    //   faproggene=0 → ambos sexos
    //   faproggene=1 → solo hombres (excluye sexo='F')
    //   faproggene=2 → solo mujeres (excluye sexo='M')

    public async Task<PagedResult<FaprogpypDto>> GetByPacienteAsync(
        string adpaciiden, int pagina, int tamPagina)
    {
        pagina = pagina < 1 ? 1 : pagina;
        tamPagina = tamPagina < 1 ? 50 :
                    tamPagina > 200 ? 200 : tamPagina;

        // 1. Buscar paciente
        var paciente = await _repoPaciente.GetByCodeAsync(adpaciiden.Trim())
            ?? throw new KeyNotFoundException(
                $"Paciente con identificación '{adpaciiden}' no encontrado.");

        if (!paciente.ADPACIFENA.HasValue)
            throw new InvalidOperationException(
                $"El paciente '{adpaciiden}' no tiene fecha de nacimiento registrada.");

        // 2. Calcular edad en años (igual a FoxPro: diferencia de días / 365)
        DateTime fechaRef = DateTime.Today;
        double edadAnnos = (fechaRef - paciente.ADPACIFENA.Value.Date).TotalDays / 365.0;

        // 3. Normalizar sexo (M/F)
        string sexo = (paciente.ADPACISEXO ?? string.Empty).Trim().ToUpper();

        // 4. Traer todos los programas para filtrar en memoria
        //    (el filtro de edad/género no se puede delegar a SQL fácilmente)
        var filter = new FaprogpypFilter { Pagina = 1, TamPagina = 200 };
        var (todos, _) = await _repo.GetAllAsync(filter);

        // 5. Aplicar reglas de FoxPro (edad + género)
        var habilitados = todos.Where(p => AplicaAlPaciente(p, edadAnnos, sexo)).ToList();

        // 6. Paginar en memoria sobre el resultado filtrado
        int total = habilitados.Count;
        int offset = (pagina - 1) * tamPagina;
        var paginaItems = habilitados.Skip(offset).Take(tamPagina);

        return new PagedResult<FaprogpypDto>
        {
            Items = paginaItems.Select(ToDto),
            Pagina = pagina,
            TamPagina = tamPagina,
            TotalItems = total,
        };
    }

    /// <summary>
    /// Devuelve true si el programa aplica al paciente según edad y género.
    /// </summary>
    private static bool AplicaAlPaciente(Faprogpyp p, double edadAnnos, string sexo)
    {
        // ── Validar rango de edad ─────────────────────────────────────────
        // Si el programa tiene límites definidos, la edad debe estar dentro
        if (p.Faprogdesd.HasValue && p.Faproghast.HasValue)
        {
            if (edadAnnos < p.Faprogdesd.Value || edadAnnos > p.Faproghast.Value)
                return false;
        }

        // ── Validar género ────────────────────────────────────────────────
        // faproggene = 0 → ambos sexos (sin restricción)
        // faproggene = 1 → solo hombres → excluir si sexo = 'F'
        // faproggene = 2 → solo mujeres → excluir si sexo = 'M'
        int gene = p.Faproggene ?? 0;
        if (gene == 1 && sexo == "F") return false;
        if (gene == 2 && sexo == "M") return false;

        return true;
    }

    public async Task<FaprogpypDto> CreateAsync(CreateFaprogpypDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    public async Task<FaprogpypDto> UpdateAsync(string faprogcodi, UpdateFaprogpypDto dto)
    {
        var existing = await _repo.GetByCodeAsync(faprogcodi)
            ?? throw new KeyNotFoundException($"Programa PyP '{faprogcodi}' no encontrado.");
        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string faprogcodi)
        => _repo.DeleteAsync(faprogcodi);

    // ── Mappers ───────────────────────────────────────────────────────────

    private static FaprogpypDto ToDto(Faprogpyp e) => new()
    {
        Faprogcodi = e.Faprogcodi,
        Faprogcod1 = e.Faprogcod1,
        Faprognomb = e.Faprognomb,
        Faprogclas = e.Faprogclas,
        Faprogdesd = e.Faprogdesd,
        Faproghast = e.Faproghast,
        Faproggene = e.Faproggene,
        Faprogfrec = e.Faprogfrec,
        faprogchbx = e.faprogchbx,
        Faficocodi = e.Faficocodi,
        Fafisecodi = e.Fafisecodi,
        Hcformular = e.Hcformular,
        Hcforprive = e.Hcforprive,
        Hcenfeprve = e.Hcenfeprve,
        Hcenfectrl = e.Hcenfectrl,
        Faservcodi = e.Faservcodi,
        Famesecoco = e.Famesecoco,
        Famesecopr = e.Famesecopr,
        Faprogestad = e.Faprogestad,
    };

    private static Faprogpyp FromCreate(CreateFaprogpypDto d) => new()
    {
        Faprogcodi = d.Faprogcodi,
        Faprogcod1 = d.Faprogcod1 ?? string.Empty,
        Faprognomb = d.Faprognomb ?? string.Empty,
        Faprogclas = d.Faprogclas ?? string.Empty,
        Faprogdesd = d.Faprogdesd,
        Faproghast = d.Faproghast,
        Faproggene = d.Faproggene,
        Faprogfrec = d.Faprogfrec,
        faprogchbx = d.faprogchbx,
        Faficocodi = d.Faficocodi ?? string.Empty,
        Fafisecodi = d.Fafisecodi ?? string.Empty,
        Hcformular = d.Hcformular ?? string.Empty,
        Hcforprive = d.Hcforprive ?? string.Empty,
        Hcenfeprve = d.Hcenfeprve ?? string.Empty,
        Hcenfectrl = d.Hcenfectrl ?? string.Empty,
        Faservcodi = d.Faservcodi ?? string.Empty,
        Famesecoco = d.Famesecoco ?? string.Empty,
        Famesecopr = d.Famesecopr ?? string.Empty,
        Faprogestad = d.Faprogestad,
    };

    private static void ApplyUpdate(Faprogpyp e, UpdateFaprogpypDto d)
    {
        if (d.Faprogcod1 is not null) e.Faprogcod1 = d.Faprogcod1;
        if (d.Faprognomb is not null) e.Faprognomb = d.Faprognomb;
        if (d.Faprogclas is not null) e.Faprogclas = d.Faprogclas;
        if (d.Faprogdesd is not null) e.Faprogdesd = d.Faprogdesd;
        if (d.Faproghast is not null) e.Faproghast = d.Faproghast;
        if (d.Faproggene is not null) e.Faproggene = d.Faproggene;
        if (d.Faprogfrec is not null) e.Faprogfrec = d.Faprogfrec;
        if (d.faprogchbx is not null) e.faprogchbx = d.faprogchbx;
        if (d.Faficocodi is not null) e.Faficocodi = d.Faficocodi;
        if (d.Fafisecodi is not null) e.Fafisecodi = d.Fafisecodi;
        if (d.Hcformular is not null) e.Hcformular = d.Hcformular;
        if (d.Hcforprive is not null) e.Hcforprive = d.Hcforprive;
        if (d.Hcenfeprve is not null) e.Hcenfeprve = d.Hcenfeprve;
        if (d.Hcenfectrl is not null) e.Hcenfectrl = d.Hcenfectrl;
        if (d.Faservcodi is not null) e.Faservcodi = d.Faservcodi;
        if (d.Famesecoco is not null) e.Famesecoco = d.Famesecoco;
        if (d.Famesecopr is not null) e.Famesecopr = d.Famesecopr;
        if (d.Faprogestad is not null) e.Faprogestad = d.Faprogestad;
    }
}