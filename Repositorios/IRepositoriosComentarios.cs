using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;

namespace AnimalApiPeliculas.Repositorios {
    public interface IRepositoriosComentarios {
        Task Actualizar(Comentario comentario);
        Task Borrar(int id);
        Task<int> Crear(Comentario comentario);
        Task<bool> Existe(int id);
        Task<Comentario?> ObtenerPorId(int Id);
        Task<List<Comentario>> ObtenerTodos(int PeliculaId);
    }
}
