using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;
using AnimalApiPeliculas.Filtros;
using AnimalApiPeliculas.Repositorios;
using AnimalApiPeliculas.Servicios;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AnimalApiPeliculas.Endpoints {
    public static class ActoresEndpoints {
        private static readonly string contenedor = "actores";

        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group) {
            group.MapPost("/", Crear).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearActorDTO>>().WithOpenApi();
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actores-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapGet("/{nombre}", obtenerPorNombre);
            group.MapPut("/{id:int}", Actializar).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearActorDTO>>().WithOpenApi();
            group.MapDelete("/{id }", Borrar);

            return group;
        }

        static async Task<Results<Created<ActorDTO>, ValidationProblem>> Crear([FromForm] CrearActorDTO crearActorDTO, IRepositorioActores repositorio, IOutputCacheStore outputCacheStore, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos) {

            var actor = mapper.Map<Actor>(crearActorDTO);

            if (crearActorDTO.Foto is not null) { //Si el usuario subio imagen
                var url = await almacenadorArchivos.Almacenar(contenedor, crearActorDTO.Foto);
                actor.Foto = url;
            }
            var id = await repositorio.Crear(actor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actores/{id}", actorDTO);
        }


        static async Task<Ok<List<ActorDTO>>> ObtenerTodos(IRepositorioActores repositorio, IMapper mapper, int pagina = 1, int recordsPorPagina = 10) {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var actores = await repositorio.ObtenerTodos(paginacion);
            var actoresDTO = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDTO);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> ObtenerPorId(int id, IRepositorioActores repositorio, IMapper mapper) {
            var actor = await repositorio.ObtenerPorId(id);

            if (actor is null) {
                return TypedResults.NotFound();
            }

            var ActorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(ActorDTO);
        }

        static async Task<Ok<List<ActorDTO>>> obtenerPorNombre(string nombre, IRepositorioActores repositorio, IMapper mapper) {
            var actores = await repositorio.ObtenerPorNombre(nombre);
            var actoresDTO = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actializar(int id, [FromForm] CrearActorDTO crearActorDTO, IAlmacenadorArchivos almacenadorArchivos, IRepositorioActores repositorio, IMapper mapper, IOutputCacheStore outputCacheStore) {

            var actorBD = await repositorio.ObtenerPorId(id); // Verifica primero si existe el Actor
            if (actorBD is null) {
                return TypedResults.NotFound();
            }

            var actorParaActualizar = mapper.Map<Actor>(crearActorDTO);
            actorParaActualizar.Id = id;
            actorParaActualizar.Foto = actorBD.Foto;

            if (crearActorDTO.Foto is not null) {// Si envio para cambio de foto
                var url = await almacenadorArchivos.Editar(actorParaActualizar.Foto, contenedor, crearActorDTO.Foto);
                actorParaActualizar.Foto = url;
            }

            await repositorio.Actualizar(actorParaActualizar);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }


        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioActores repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore) {

            var actorDB = await repositorio.ObtenerPorId(id);
            if (actorDB is null) {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(actorDB.Foto, contenedor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }


    }
}