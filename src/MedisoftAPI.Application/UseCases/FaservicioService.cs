using MedisoftAPI.Application.DTOs;
using MedisoftAPI.Application.Interfaces;
using MedisoftAPI.Domain.Entities;
using MedisoftAPI.Domain.Interfaces;

namespace MedisoftAPI.Application.UseCases;

public class FaservicioService : IFaservicioService
{
    private readonly IFaservicioRepository _repo;

    public FaservicioService(IFaservicioRepository repo) => _repo = repo;

    public async Task<PagedResult<FaservicioDto>> GetAllAsync(FaservicioQueryDto query)
    {
        var filter = new FaservicioFilter
        {
            FASERVCODI = query.FASERVCODI,
            FASERVNOMB = query.FASERVNOMB,
            FASERVESTA = query.FASERVESTA,
            CTCLMACODI = query.CTCLMACODI,
            FACLSECODI = query.FACLSECODI,
            FASERVTIPO = query.FASERVTIPO,
            Pagina     = query.Pagina    < 1   ? 1   : query.Pagina,
            TamPagina  = query.TamPagina < 1   ? 50  :
                         query.TamPagina > 200 ? 200 : query.TamPagina
        };

        var (items, total) = await _repo.GetAllAsync(filter);

        return new PagedResult<FaservicioDto>
        {
            Items      = items.Select(ToDto),
            Pagina     = filter.Pagina,
            TamPagina  = filter.TamPagina,
            TotalItems = total
        };
    }

    public async Task<FaservicioDto?> GetByCodeAsync(string codserv)
    {
        var e = await _repo.GetByCodeAsync(codserv);
        return e is null ? null : ToDto(e);
    }

    public async Task<FaservicioDto> CreateAsync(CreateFaservicioDto dto)
        => ToDto(await _repo.CreateAsync(FromCreate(dto)));

    public async Task<FaservicioDto> UpdateAsync(string codserv, UpdateFaservicioDto dto)
    {
        var existing = await _repo.GetByCodeAsync(codserv)
            ?? throw new KeyNotFoundException($"Servicio '{codserv}' no encontrado.");
        ApplyUpdate(existing, dto);
        return ToDto(await _repo.UpdateAsync(existing));
    }

    public Task<bool> DeleteAsync(string codserv) => _repo.DeleteAsync(codserv);

    // ── Mappers ────────────────────────────────────────────────
    private static FaservicioDto ToDto(Faservicio e) => new()
    {
        CTCLMACODI  = e.CTCLMACODI,  FACLSECODI  = e.FACLSECODI,
        FASUBCLCODI = e.FASUBCLCODI, FASERVCODI  = e.FASERVCODI,
        FASERVNOMB  = e.FASERVNOMB,  FASERVPROG  = e.FASERVPROG,
        FASERVCONS  = e.FASERVCONS,  FASERVPART  = e.FASERVPART,
        FAFISECODI  = e.FAFISECODI,  FASERVTIPO  = e.FASERVTIPO,
        FASERVOBS   = e.FASERVOBS,   FASERVPAQU  = e.FASERVPAQU,
        FAAGSECODI  = e.FAAGSECODI,  FASERVDUCI  = e.FASERVDUCI,
        FASERVADICI = e.FASERVADICI, FASERVPRIV  = e.FASERVPRIV,
        FASERVINTE  = e.FASERVINTE,  FASERVENFE  = e.FASERVENFE,
        FASERVFREC  = e.FASERVFREC,  FASERVTRAN  = e.FASERVTRAN,
        FAESVAC     = e.FAESVAC,     FASERVTRAP  = e.FASERVTRAP,
        FASERVTRAS  = e.FASERVTRAS,  FASERVRX    = e.FASERVRX,
        FAESTERAPI  = e.FAESTERAPI,  FASERVESTA  = e.FASERVESTA,
        FASERV2175  = e.FASERV2175,  FAINCAPCID  = e.FAINCAPCID
    };

    private static Faservicio FromCreate(CreateFaservicioDto d) => new()
    {
        CTCLMACODI  = d.CTCLMACODI,  FACLSECODI  = d.FACLSECODI,
        FASUBCLCODI = d.FASUBCLCODI, FASERVCODI  = d.FASERVCODI,
        FASERVNOMB  = d.FASERVNOMB,  FASERVPROG  = d.FASERVPROG,
        FASERVCONS  = d.FASERVCONS,  FASERVPART  = d.FASERVPART,
        FAFISECODI  = d.FAFISECODI,  FASERVTIPO  = d.FASERVTIPO,
        FASERVOBS   = d.FASERVOBS,   FASERVPAQU  = d.FASERVPAQU,
        FAAGSECODI  = d.FAAGSECODI,  FASERVDUCI  = d.FASERVDUCI,
        FASERVADICI = d.FASERVADICI, FASERVPRIV  = d.FASERVPRIV,
        FASERVINTE  = d.FASERVINTE,  FASERVENFE  = d.FASERVENFE,
        FASERVFREC  = d.FASERVFREC,  FASERVTRAN  = d.FASERVTRAN,
        FAESVAC     = d.FAESVAC,     FASERVTRAP  = d.FASERVTRAP,
        FASERVTRAS  = d.FASERVTRAS,  FASERVRX    = d.FASERVRX,
        FAESTERAPI  = d.FAESTERAPI,  FASERVESTA  = d.FASERVESTA,
        FASERV2175  = d.FASERV2175,  FAINCAPCID  = d.FAINCAPCID
    };

    private static void ApplyUpdate(Faservicio e, UpdateFaservicioDto d)
    {
        e.CTCLMACODI  = d.CTCLMACODI;  e.FACLSECODI  = d.FACLSECODI;
        e.FASUBCLCODI = d.FASUBCLCODI; e.FASERVNOMB  = d.FASERVNOMB;
        e.FASERVPROG  = d.FASERVPROG;  e.FASERVCONS  = d.FASERVCONS;
        e.FASERVPART  = d.FASERVPART;  e.FAFISECODI  = d.FAFISECODI;
        e.FASERVTIPO  = d.FASERVTIPO;  e.FASERVOBS   = d.FASERVOBS;
        e.FASERVPAQU  = d.FASERVPAQU;  e.FAAGSECODI  = d.FAAGSECODI;
        e.FASERVDUCI  = d.FASERVDUCI;  e.FASERVADICI = d.FASERVADICI;
        e.FASERVPRIV  = d.FASERVPRIV;  e.FASERVINTE  = d.FASERVINTE;
        e.FASERVENFE  = d.FASERVENFE;  e.FASERVFREC  = d.FASERVFREC;
        e.FASERVTRAN  = d.FASERVTRAN;  e.FAESVAC     = d.FAESVAC;
        e.FASERVTRAP  = d.FASERVTRAP;  e.FASERVTRAS  = d.FASERVTRAS;
        e.FASERVRX    = d.FASERVRX;    e.FAESTERAPI  = d.FAESTERAPI;
        e.FASERVESTA  = d.FASERVESTA;  e.FASERV2175  = d.FASERV2175;
        e.FAINCAPCID  = d.FAINCAPCID;
    }
}
