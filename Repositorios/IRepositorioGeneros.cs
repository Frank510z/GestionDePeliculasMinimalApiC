﻿using AnimalApiPeliculas.Entidades;

namespace AnimalApiPeliculas.Repositorios {
    public interface IRepositorioGeneros {
        Task<int> CrearGenero(Genero genero);
        Task<Genero?> ObtenerPorId(int id);
        Task<List<Genero>> ObtenerTodos();
        Task<bool> Existe(int id);
        Task Actualizar(Genero genero);
        Task Borrar(int id);
        Task<List<int>> ExistenGeneros(List<int> ids);
        Task<bool> Existe(int id, string nombre);
    }
}