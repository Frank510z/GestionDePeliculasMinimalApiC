using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AnimalApiPeliculas.Repositorios {
    public class RepositorioPeliculas : IRepositorioPeliculas {
        private readonly string? connectionString;
        private readonly HttpContext httpContext;

        public RepositorioPeliculas(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO) {
            using (var conexion = new SqlConnection(connectionString)) {

                var peliculas = await conexion.QueryAsync<Pelicula>("Peliculas_ObtenerTodos", new { paginacionDTO.Pagina, paginacionDTO.RecordsPorPagina }, commandType: CommandType.StoredProcedure);

                var cantidadPeliculas = await conexion.QuerySingleAsync<int>("Peliculas_Cantidad", commandType: CommandType.StoredProcedure);
                httpContext.Response.Headers.Append("CantidadTotalPeliculas", cantidadPeliculas.ToString()); //Mostrara en la cabecera la cantidad de peliculas total

                return peliculas.ToList(); //Retorna las Peliculas
            }
        }


        public async Task<Pelicula?> ObtenerPorId(int id) {
            using (var conexion = new SqlConnection(connectionString)) {

                using (var multi = await conexion.QueryMultipleAsync("Peliculas_ObtenerPorId", new { id }, commandType: CommandType.StoredProcedure)) {
                    var pelicula = await multi.ReadFirstAsync<Pelicula>(); //ReadFirstAsync ya que solo traera una sola pelicula
                    var comentarios = await multi.ReadAsync<Comentario>();
                    var Generos = await multi.ReadAsync<Genero>();
                    var Actores = await multi.ReadAsync<ActorPeliculaDTO>();

                    foreach (var genero in Generos) {
                        pelicula.GenerosPeliculas.Add(new GeneroPelicula {
                            GeneroId = genero.Id,  //Id genero
                            Genero = genero  //Nombre de genero
                        });
                    }

                    foreach (var actor in Actores) {
                        pelicula.ActoresPeliculas.Add(new ActorPelicula {
                            ActorId = actor.Id,
                            Personaje = actor.Personaje,
                            Actor = new Actor { Nombre = actor.Nombre }
                        });
                    }

                    pelicula.Comentarios = comentarios.ToList(); // Se la pasa los comentarios recibidos a la variable de entidad pelicula
                    return pelicula;
                }
            }
        }


        public async Task<List<Pelicula>> ObtenerPorTitulo(string titulo) {
            using (var conexion = new SqlConnection(connectionString)) {
                var pelicula = await conexion.QueryAsync<Pelicula>("Peliculas_ObtenerPorTitulo", new { titulo }, commandType: CommandType.StoredProcedure);
                return pelicula.ToList();
            }
        }


        public async Task<int> Crear(Pelicula pelicula) {
            using (var conexion = new SqlConnection(connectionString)) {
                var id = await conexion.QuerySingleAsync<int>("Peliculas_Crear", new { pelicula.Titulo, pelicula.EnCines, pelicula.FechaDeLanzamiento, pelicula.Poster }, commandType: CommandType.StoredProcedure);
                pelicula.Id = id;
                return id;
            }
        }


        public async Task<bool> Existe(int id) {
            using (var conexion = new SqlConnection(connectionString)) {

                var existe = await conexion.QuerySingleAsync<bool>("Peliculas_ExistePorId", new { id }, commandType: CommandType.StoredProcedure);
                return existe;
            }
        }


        public async Task Actualizar(Pelicula pelicula) {
            using (var conexion = new SqlConnection(connectionString)) {

                await conexion.ExecuteAsync("Peliculas_Actualizar", new { pelicula.Titulo, pelicula.EnCines, pelicula.FechaDeLanzamiento, pelicula.Poster, pelicula.Id }, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task Borrar(int id) {
            using (var conexion = new SqlConnection(connectionString)) {
                await conexion.ExecuteAsync("Peliculas_Borrar", new { id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AsignarGeneros(int id, List<int> generosIds) {
            var dt = new DataTable();
            dt.Columns.Add("id", typeof(int));

            foreach (var generoIds in generosIds) {
                dt.Rows.Add(generoIds);
            }

            using (var conexion = new SqlConnection(connectionString)) {
                await conexion.ExecuteAsync("Peliculas_AsignarGeneros", new { peliculaId = id, generosIds = dt }, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task AsignarActores(int id, List<ActorPelicula> actores) {

            for (int i = 1; i <= actores.Count; i++) {
                actores[i - 1].Orden = i;
            }

            var dt = new DataTable();
            dt.Columns.Add("ActorId", typeof(int));
            dt.Columns.Add("Personaje", typeof(string));
            dt.Columns.Add("Orden", typeof(int));

            foreach (var actorPelicula in actores) {
                dt.Rows.Add(actorPelicula.ActorId, actorPelicula.Personaje, actorPelicula.Orden);
            }

            using (var conexion = new SqlConnection(connectionString)) {
                await conexion.ExecuteAsync("Peliculas_AsignarActores", new { peliculaId = id, actores = dt }, commandType: CommandType.StoredProcedure);
            }
        }



    }
}