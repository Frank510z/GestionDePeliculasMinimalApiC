using Microsoft.AspNetCore.Identity;

namespace AnimalApiPeliculas.Entidades {
    public class Comentario {
        public int Id { get; set; }
        public String Cuerpo { get; set; } = null!;
        public int PeliculaId { get; set; }

        //**********Para Relacion Con Usuarios*************//
        public string UsuarioId { get; set; } = null!;
        public IdentityUser Usuario { get; set; } = null!;
    }
}
