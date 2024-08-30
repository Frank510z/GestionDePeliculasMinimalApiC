using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;
using AnimalApiPeliculas.Filtros;
using AnimalApiPeliculas.Repositorios;
using AnimalApiPeliculas.Servicios;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace AnimalApiPeliculas.Endpoints {
    public static class ComentariosEndpoints {

        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group) {
            group.MapPost("/", Crear).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>().RequireAuthorization(); //agregamos el RequireAuthorization para que solo los que estan autorizados puedan crear comentarios
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).
            Tag("comentarios-get").
            SetVaryByRouteValue(new string[] { "peliculaId" })); // el setVaryBy... es para que no se quede alamcenado en la cache la variable que viene de las rutas de peliculaId
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPut("/{id:int}", Actualizar).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>().RequireAuthorization();
            group.MapDelete("/{id:int}", Borrar).RequireAuthorization();

            return group;
        }


        static async Task<Results<Created<ComentarioDTO>, NotFound, BadRequest<string>>> Crear(int peliculaId, CrearComentarioDTO crearComentarioDTO, IRepositoriosComentarios repositoriosComentarios,
            IRepositorioPeliculas repositorioPeliculas, IMapper mapper, IOutputCacheStore outputCacheStore, IServicioUsuarios servicioUsuarios) { //Nunca se envia el UsuarioIdPor parametro
            if (!await repositorioPeliculas.Existe(peliculaId)) { //Si no existe la pelicula pues,,, no hay comentario que crear
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId; // le da el PeliculaId al PeliculaId del comentario

            var usuario = await servicioUsuarios.ObtenerUsuario(); // Consigue el Usuario
            if (usuario is null) {
                return TypedResults.BadRequest("Usuario NO encontrado");
            }
            comentario.UsuarioId = usuario.Id; // le pasa el usuarioId conseguido al comentario


            var id = await repositoriosComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Created($"/comentario/{id}", comentarioDTO);
        }


        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(int peliculaId, IRepositoriosComentarios repositoriosComentarios, IRepositorioPeliculas repositorioPeliculas, IMapper mapper) {

            if (!await repositorioPeliculas.Existe(peliculaId)) { //Si no existe la pelicula pues,,, no hay comentario que crear
                return TypedResults.NotFound();
            }

            var comentarios = await repositoriosComentarios.ObtenerTodos(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);

            return TypedResults.Ok(comentariosDTO);
        }


        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(int peliculaId, int id, IRepositoriosComentarios repositoriosComentarios, IMapper mapper) {
            var comentario = await repositoriosComentarios.ObtenerPorId(id);
            if (comentario is null) {
                return TypedResults.NotFound();
            }
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Ok(comentarioDTO);
        }


        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Actualizar(int peliculaId, int id, CrearComentarioDTO crearComentarioDTO, IRepositoriosComentarios repositoriosComentarios, IOutputCacheStore outputCacheStore,
    IRepositorioPeliculas repositorioPeliculas, IServicioUsuarios servicioUsuarios) {
            if (!await repositorioPeliculas.Existe(peliculaId)) { //Si no existe la pelicula pues,,, no hay comentario que crear
                return TypedResults.NotFound();
            }

            var comentarioBD = await repositoriosComentarios.ObtenerPorId(id); //Obtiene todo el comentario de la BD

            if (comentarioBD is null) { // si el comentario es null o no existe
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuarios.ObtenerUsuario(); //Obtiene el usuario
            if (usuario is null) { // si el usuario es nulo o no existe
                return TypedResults.NotFound();
            }

            if (comentarioBD.UsuarioId != usuario.Id) { // Si el cometario.Id es diferente al id del usuario
                return TypedResults.Forbid(); // retorna un Forbid; que significa que esta prohibido esa accion 
            }

            comentarioBD.Cuerpo = crearComentarioDTO.Cuerpo;

            await repositoriosComentarios.Actualizar(comentarioBD);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);

            return TypedResults.NoContent();

        }


        static async Task<Results<NotFound, NoContent,ForbidHttpResult>> Borrar(int peliculaId, int id, IRepositoriosComentarios repositoriosComentarios, IOutputCacheStore outputCacheStore, IServicioUsuarios servicioUsuarios) {

            var comentarioBD = await repositoriosComentarios.ObtenerPorId(id); //Obtiene todo el comentario de la BD

            if (comentarioBD is null) { // si el comentario es null o no existe
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuarios.ObtenerUsuario(); //Obtiene el usuario
            if (usuario is null) { // si el usuario es nulo o no existe
                return TypedResults.NotFound();
            }

            if (comentarioBD.UsuarioId != usuario.Id) { // Si el cometario.Id es diferente al id del usuario
                return TypedResults.Forbid(); // retorna un Forbid; que significa que esta prohibido esa accion 
            }

            await repositoriosComentarios.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);

            return TypedResults.NoContent();
        }



    }
}
