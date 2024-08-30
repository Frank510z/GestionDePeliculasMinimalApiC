using AnimalApiPeliculas.Entidades;

namespace AnimalApiPeliculas.Repositorios {
    public interface IRepositorioErrores {
        Task Crear(Error error);
    }
}