using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;

namespace AnimalApiPeliculas.Repositorios {
    public interface IRepositorioPeliculas {
        Task Actualizar(Pelicula pelicula);
        Task AsignarActores(int id, List<ActorPelicula> actores);
        Task AsignarGeneros(int id, List<int> generosIds);
        Task Borrar(int id);
        Task<int> Crear(Pelicula pelicula);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPorId(int id);
        Task<List<Pelicula>> ObtenerPorTitulo(string nombre);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO);
    }
}