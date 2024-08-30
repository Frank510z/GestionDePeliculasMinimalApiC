using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;
using AnimalApiPeliculas.Filtros;
using AnimalApiPeliculas.Repositorios;
using AnimalApiPeliculas.Servicios;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AnimalApiPeliculas.Endpoints {
    public static class PeliculasEndpoints {
        private static readonly string contenedor = "peliculas"; //Nombre del contenedor/Carpeta donde se almacenaran las de Imagenes

        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group) {

            group.MapPost("/", Crear).DisableAntiforgery().DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>().RequireAuthorization("esadmin").WithOpenApi();
            group.MapGet("/", Obtener).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("peliculas-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapGet("/{titulo}", ObtenerPorTitulo);
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>().RequireAuthorization("esadmin").WithOpenApi();
            group.MapDelete("/{id:int}", Borrar).RequireAuthorization("esadmin");
            group.MapPost("/{id:int}/asignargeneros", AsignarGeneros).RequireAuthorization("esadmin");
            group.MapPost("/{id:int}/asignaractores", AsignarActores).RequireAuthorization("esadmin");
            return group;
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorioPeliculas, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper) {

            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);

            if (crearPeliculaDTO.Poster is not null) {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }

            var id = await repositorioPeliculas.Crear(pelicula);
            await outputCacheStore.EvictByTagAsync("pelicula-get", default);
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Created($"/peliculas/{id}", peliculaDTO);
        }




        static async Task<Ok<List<PeliculaDTO>>> Obtener(IRepositorioPeliculas repositorio, IMapper mapper, int pagina = 1, int recordsPagina = 10) {

            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPagina };
            var peliculas = await repositorio.ObtenerTodos(paginacion);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);

            return TypedResults.Ok(peliculasDTO);
        }


        static async Task<Results<Ok<PeliculaDTO>, NotFound>> ObtenerPorId(int id, IRepositorioPeliculas repositorio, IMapper mapper) {

            var pelicula = await repositorio.ObtenerPorId(id);

            if (pelicula is null) {
                return TypedResults.NotFound();
            }
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Ok(peliculaDTO);
        }


        static async Task<Ok<List<PeliculaDTO>>> ObtenerPorTitulo(string titulo, IRepositorioPeliculas repositorio, IMapper mapper) {

            var pelicula = await repositorio.ObtenerPorTitulo(titulo);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(pelicula);
            return TypedResults.Ok(peliculasDTO);
        }


        static async Task<Results<NoContent, NotFound>> Actualizar(int id, [FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper) {
            var peliculaDB = await repositorio.ObtenerPorId(id);

            if (peliculaDB is null) {
                return TypedResults.NotFound();
            }

            var peliculaParaActualizar = mapper.Map<Pelicula>(crearPeliculaDTO);
            peliculaParaActualizar.Id = id;
            peliculaParaActualizar.Poster = peliculaDB.Poster;

            if (crearPeliculaDTO.Poster is not null) {
                var url = await almacenadorArchivos.Editar(peliculaParaActualizar.Poster, contenedor, crearPeliculaDTO.Poster);
                peliculaParaActualizar.Poster = url;
            }

            await repositorio.Actualizar(peliculaParaActualizar);
            await outputCacheStore.EvictByTagAsync("pelicula-get", default);

            return TypedResults.NoContent();
        }



        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioPeliculas repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore) {

            var peliculaDB = await repositorio.ObtenerPorId(id);

            if (peliculaDB is null) {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(peliculaDB.Poster, contenedor);
            await outputCacheStore.EvictByTagAsync("pelicula-get", default);
            return TypedResults.NoContent();

        }


        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id, List<int> generosIds, IRepositorioPeliculas repositorioPeliculas, IRepositorioGeneros repositorioGeneros) {

            if (!await repositorioPeliculas.Existe(id)) {
                return TypedResults.NotFound();
            }

            var generosExistentes = new List<int>();

            if (generosIds.Count != 0) {
                generosExistentes = await repositorioGeneros.ExistenGeneros(generosIds);
            }

            if (generosExistentes.Count != generosIds.Count) {
                var generosNoExistentes = generosIds.Except(generosExistentes);   //generosIds expto los que estan en generosExistentes
                return TypedResults.BadRequest($"Los generos de id{string.Join(",", generosNoExistentes)} NO EXISTEN");
            }

            await repositorioPeliculas.AsignarGeneros(id, generosIds);
            return TypedResults.NoContent();
        }


        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AsignarActores(int id, List<AsignarActorPeliculaDTO> actoresDTO, IRepositorioPeliculas repositorioPeliculas, IRepositorioActores repositorioActores, IMapper mapper) {

            if (!await repositorioPeliculas.Existe(id)) { //Verificamos si existe la pelicula
                return TypedResults.NotFound();
            }

            var actoresExistentes = new List<int>();
            var actoresIds = actoresDTO.Select(a => a.ActorId).ToList();

            if (actoresDTO.Count != 0) {
                actoresExistentes = await repositorioActores.ExistenGeneros(actoresIds);
            }

            if (actoresExistentes.Count != actoresDTO.Count) {
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);
                return TypedResults.BadRequest($"Los actores de id{string.Join(",", actoresNoExistentes)} No EXISTEN");
            }

            var actores = mapper.Map<List<ActorPelicula>>(actoresDTO);
            await repositorioPeliculas.AsignarActores(id, actores);
            return TypedResults.NoContent();
        }


    }
}
