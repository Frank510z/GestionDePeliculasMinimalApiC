using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Identity;

namespace AnimalApiPeliculas.Servicios {
    public class ServicioUsuarios : IServicioUsuarios {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager) {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }


        public async Task<IdentityUser?> ObtenerUsuario() {

            var emailClaim = httpContextAccessor.HttpContext!.User.Claims.Where(x => x.Type == "email").FirstOrDefault(); // Obtiene el email del usuario de la funcion Construirtoken

            if (emailClaim is null) {
                return null;
            }
            var email = emailClaim.Value; //obtiene su email
            return await userManager.FindByEmailAsync(email);
        }

    }
}
