using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;

namespace AnimalApiPeliculas.Repositorios {
    public interface IRepositorioActores {
        Task Actualizar(Actor actor);
        Task Borrar(int id);
        Task<int> Crear(Actor actor);
        Task<bool> Existe(int id);
        Task<List<int>> ExistenGeneros(List<int> ids);
        Task<Actor?> ObtenerPorId(int id);
        Task<List<Actor>> ObtenerPorNombre(string nombre);
        Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDTO);
    }
}