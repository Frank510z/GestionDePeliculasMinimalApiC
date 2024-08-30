using AnimalApiPeliculas.Repositorios;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AnimalApiPeliculas.Servicios {
    public class UsuarioStore : IUserStore<IdentityUser>, IUserEmailStore<IdentityUser>, IUserPasswordStore<IdentityUser>, IUserClaimStore<IdentityUser> {
        private readonly IRepositorioUsuarios repositorioUsuarios;

        public UsuarioStore(IRepositorioUsuarios repositorioUsuarios) {
            this.repositorioUsuarios = repositorioUsuarios;
        }

        public async Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            await repositorioUsuarios.AsignarClaims(user, claims);
        }

        //Crear Usuario
        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken) {
            user.Id = await repositorioUsuarios.Crear(user);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public void Dispose() {

        }

        //Buscar por Email
        public async Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
            return await repositorioUsuarios.BuscarUsuarioPorEmail(normalizedEmail);
        }

        public Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        //Buscar por UserName
        public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            return await repositorioUsuarios.BuscarUsuarioPorEmail(normalizedUserName);
        }

        //Obtener claims
        public async Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken cancellationToken) {
            return await repositorioUsuarios.ObtenerClaims(user);
        }

        // Obtener Email:
        public Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.NormalizedEmail);

        }

        public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.NormalizedUserName);
        }

        //Obtener el HashPasword
        public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.PasswordHash);
        }

        //Obtener el Id
        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Id);
        }

        //Obtener el UserName
        public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email);
        }

        public Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        // Remover Claims 
        public async Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken) {
            await repositorioUsuarios.RemoverClaims(user, claims);
        }

        public Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        //Asignar normalizedEmail a user
        public Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken) {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        //Asignar normalizedName a user
        public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken) {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        //Asignar passwordHash a user
        public Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken) {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        // Actualizar usuario: pero nosotros no aremos nada solo retornar el resultado 
        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken) {
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
