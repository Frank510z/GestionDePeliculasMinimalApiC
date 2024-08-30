using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Filtros;
using AnimalApiPeliculas.Servicios;
using AnimalApiPeliculas.Utilidades;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AnimalApiPeliculas.Endpoints {
    public static class UsuariosEndpoints {
        public static RouteGroupBuilder MapUsuarios(this RouteGroupBuilder group) {
            group.MapPost("/registrar", Registrar).AddEndpointFilter<FiltroValidaciones<CredencialesUsuarioDTO>>();
            group.MapPost("/login", Login).AddEndpointFilter<FiltroValidaciones<CredencialesUsuarioDTO>>();

            //agregamos los endpoints
            group.MapPost("/haceradmin", HacerAdmin).AddEndpointFilter<FiltroValidaciones<EditarClaimDTO>>();//.RequireAuthorization("esadmin");
            group.MapPost("/removeradmin", RemoverAdmin).AddEndpointFilter<FiltroValidaciones<EditarClaimDTO>>();//.RequireAuthorization("esadmin");

            group.MapGet("/renovartoken", RenovarToken).RequireAuthorization();

            return group;
        }

        // Registrar
        static async Task<Results<Ok<RespuestaAutenticacionDTO>, BadRequest<IEnumerable<IdentityError>>>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO, [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration) {

            var usuario = new IdentityUser { UserName = credencialesUsuarioDTO.Email, Email = credencialesUsuarioDTO.Email };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);

            if (resultado.Succeeded) {

                var credencialesRespuesta = await ConstruirToken(credencialesUsuarioDTO, configuration, userManager);
                return TypedResults.Ok(credencialesRespuesta);

            } else {
                return TypedResults.BadRequest(resultado.Errors);
            }
        }


        //Metodo para construir Token
        private async static Task<RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDTO, IConfiguration configuration, UserManager<IdentityUser> userManager) {

            var claims = new List<Claim> {
                new Claim("email", credencialesUsuarioDTO.Email),
                new Claim("Lo que sea", "Cualquier otro valor")
                };

            // Cargamos los claims de la BD y los anexamos al listado de claims que creamos arriba 
            var usuario = await userManager.FindByNameAsync(credencialesUsuarioDTO.Email);
            var claimBD = await userManager.GetClaimsAsync(usuario!);
            claims.AddRange(claimBD);


            var llave = Llaves.ObtenerLlave(configuration);
            var creds = new SigningCredentials(llave.First(), SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);

            return new RespuestaAutenticacionDTO {
                Token = token,
                Expiracion = expiracion,
            };
        }


        static async Task<Results<Ok<RespuestaAutenticacionDTO>, BadRequest<string>>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO, [FromServices] SignInManager<IdentityUser> signInManager,
            [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration) {

            var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if (usuario is null) {
                return TypedResults.BadRequest("Login incorrecto");
            }

            var resultado = await signInManager.CheckPasswordSignInAsync(usuario, credencialesUsuarioDTO.Password, lockoutOnFailure: false);
            //lockoutOnFailure: si se logea muchas veces con contraseña equivocada se bloqueara por un tiempo 

            if (resultado.Succeeded) {
                var respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTO, configuration, userManager);
                return TypedResults.Ok(respuestaAutenticacion);
            } else {
                return TypedResults.BadRequest("Login incorrecto");
            }
        }


        // Hacer usuario admin
        static async Task<Results<NoContent, NotFound>> HacerAdmin(EditarClaimDTO editarClaimDTO, [FromServices] UserManager<IdentityUser> userManager) {

            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (usuario is null) {
                return TypedResults.NotFound();
            }

            await userManager.AddClaimAsync(usuario, new Claim("esadmin", "true"));
            return TypedResults.NoContent();
        }


        // remover admin a usuario
        static async Task<Results<NoContent, NotFound>> RemoverAdmin(EditarClaimDTO editarClaimDTO, [FromServices] UserManager<IdentityUser> userManager) {

            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (usuario is null) {
                return TypedResults.NotFound();
            }

            await userManager.RemoveClaimAsync(usuario, new Claim("esadmin", "true"));
            return TypedResults.NoContent();
        }

        //Renovar Token 
        static async Task<Results<Ok<RespuestaAutenticacionDTO>, NotFound>> RenovarToken(IServicioUsuarios servicioUsuarios, IConfiguration configuration, [FromServices] UserManager<IdentityUser> userManager) {

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null) {
                return TypedResults.NotFound();
            }

            var credencialesUsuariosDTO = new CredencialesUsuarioDTO { Email = usuario.Email! };

            var repuestaAutenticacionDTO = await ConstruirToken(credencialesUsuariosDTO, configuration, userManager);

            return TypedResults.Ok(repuestaAutenticacionDTO);
        }



    }
}