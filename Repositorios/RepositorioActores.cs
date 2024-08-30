using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AnimalApiPeliculas.Repositorios {
    public class RepositorioActores : IRepositorioActores {

        //Coneccion a connectionString (Base de Datos)
        private readonly string? connectionString; //Solo lectura
        private readonly HttpContext httpContext;

        public RepositorioActores(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
            httpContext = httpContextAccessor.HttpContext!;
        }


        // Consultar Todos Los Actores
        public async Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDTO) {
            using (var conexion = new SqlConnection(connectionString)) {
                var actores = await conexion.QueryAsync<Actor>("Actores_ObtenerTodos", new { paginacionDTO.Pagina, paginacionDTO.RecordsPorPagina }, commandType: CommandType.StoredProcedure);

                var cantidadActores = await conexion.QuerySingleAsync<int>("Actores_Cantidad", commandType: CommandType.StoredProcedure);

                httpContext.Response.Headers.Append("CantidadTotal", cantidadActores.ToString()); //Mostrara en la cabecera la cantidad de actores total
                return actores.ToList();
            }
        }



        // Consultar Actor por ID
        public async Task<Actor?> ObtenerPorId(int id) {
            using (var conexion = new SqlConnection(connectionString)) {
                var actor = await conexion.QueryFirstOrDefaultAsync<Actor>("Actores_ObtenerPorId", new { id }, commandType: CommandType.StoredProcedure);
                return actor;
            }
        }

        public async Task<List<Actor>> ObtenerPorNombre(string nombre) {
            using (var conexion = new SqlConnection(connectionString)) {
                var actores = await conexion.QueryAsync<Actor>("Actores_ObtenerPorNombre", new { nombre }, commandType: CommandType.StoredProcedure);
                return actores.ToList();
            }
        }


        public async Task<int> Crear(Actor actor) {
            using (var conexion = new SqlConnection(connectionString)) {
                var id = await conexion.QuerySingleAsync<int>("Actores_Crear", new { actor.Nombre, actor.FechaNacimiento, actor.Foto }, commandType: CommandType.StoredProcedure);
                actor.Id = id;
                return id;
            }
        }

        public async Task Actualizar(Actor actor) {
            using (var conexion = new SqlConnection(connectionString)) {
                await conexion.ExecuteAsync("Actores_Actualizar", new { actor.Nombre, actor.FechaNacimiento, actor.Foto, actor.Id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> Existe(int id) {
            using (var conexion = new SqlConnection(connectionString)) {
                var existe = await conexion.QuerySingleAsync<bool>("Actores_Existe", new { id }, commandType: CommandType.StoredProcedure);
                return existe;
            }
        }

        public async Task Borrar(int id) {
            using (var conexion = new SqlConnection(connectionString)) {
                await conexion.ExecuteAsync("Actores_Borrar", new { id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<int>> ExistenGeneros(List<int> ids) {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var id in ids) {
                dt.Rows.Add(id);
            }

            using (var conexion = new SqlConnection(connectionString)) {
                var idsGenerosExistentes = await conexion.QueryAsync<int>("Actores_ObtenerVariosPorId", new { actoresIds = dt }, commandType: CommandType.StoredProcedure);
                return idsGenerosExistentes.ToList();
            }

        }




    }
}