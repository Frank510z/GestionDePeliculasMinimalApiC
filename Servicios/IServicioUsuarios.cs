using Microsoft.AspNetCore.Identity;

namespace AnimalApiPeliculas.Servicios {
    public interface IServicioUsuarios {
        Task<IdentityUser?> ObtenerUsuario();
    }
}