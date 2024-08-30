using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Net.Http;

namespace AnimalApiPeliculas.Repositorios {
    public class RepositoriosComentarios : IRepositoriosComentarios {
        private readonly string connectionString;

        public RepositoriosComentarios(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<Comentario>> ObtenerTodos(int PeliculaId) {
            using (var conexion = new SqlConnection(connectionString)) {

                var comentario = await conexion.QueryAsync<Comentario>("Comentarios_ObtenerTodos", new { PeliculaId }, commandType: CommandType.StoredProcedure);
                return comentario.ToList(); //Retorna las Peliculas
            }
        }


        public async Task<Comentario?> ObtenerPorId(int Id) {
            using (var conexion = new SqlConnection(connectionString)) {
                var comentario = await conexion.QueryFirstOrDefaultAsync<Comentario>("Comentarios_ObtenerPorId", new { Id }, commandType: CommandType.StoredProcedure);
                return comentario;
            }
        }

        public async Task<int> Crear(Comentario comentario) {
            using (var conexion = new SqlConnection(connectionString)) {
                var id = await conexion.QuerySingleAsync<int>("Comentarios_Crear", new { comentario.Cuerpo, comentario.PeliculaId, comentario.UsuarioId }, commandType: CommandType.StoredProcedure);
                comentario.Id = id;
                return id;
            }
        }


        public async Task<bool> Existe(int id) {
            using (var conexion = new SqlConnection(connectionString)) {
                var existe = await conexion.QuerySingleAsync<bool>("Comentarios_ExistePorId", new { id }, commandType: CommandType.StoredProcedure);
                return existe;
            }
        }

        public async Task Actualizar(Comentario comentario) {
            using (var conexion = new SqlConnection(connectionString)) {
                await conexion.ExecuteAsync("Comentarios_Actualizar", new { comentario.Cuerpo, comentario.PeliculaId, comentario.Id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Borrar(int id) {
            using (var conexion = new SqlConnection(connectionString)) {
                await conexion.ExecuteAsync("Comentarios_Borrar", new { id }, commandType: CommandType.StoredProcedure);

            }
        }


    }
}
