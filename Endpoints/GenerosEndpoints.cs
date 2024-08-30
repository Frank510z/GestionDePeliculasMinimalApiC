using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Entidades;
using AnimalApiPeliculas.Filtros;
using AnimalApiPeliculas.Repositorios;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace AnimalApiPeliculas.Endpoints {
    public static class GenerosEndpoints {

        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group) {
            // Mapear los endpoints y configurar el almacenamiento en caché
            group.MapGet("/", ObtenerGeneros)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));
            group.MapPost("/", CrearGenero).AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>().RequireAuthorization("esadmin"); // Agregaremos nuestro filtro de validacion generico y pasamos el DTO CrearGeneroDTO
            group.MapGet("/{id:int}", ObtenerGeneroPorId).RequireAuthorization();

             // Agregaremos nuestro filtro de validacion generico y pasamos el DTO CrearGeneroDTO
            group.MapPut("/{id:int}", ActualizarGenero).AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>().RequireAuthorization("esadmin").WithOpenApi(opciones => {

                opciones.Summary = "Actualizar Un genero";
                opciones.Description = "Con este endpoint podemos actualizar un genero por su Id";
                opciones.Parameters[0].Description = "Id del genero a actualizar";
                opciones.RequestBody.Description = "El genero que se decea actualizar";
                return opciones;
            });

            group.MapDelete("/{id:int}", BorrarGeneroPorId).RequireAuthorization("esadmin").RequireAuthorization("esadmin");

            return group;
        }

        // Crear un nuevo género
        static async Task<Results<Created<GeneroDTO>, ValidationProblem>> CrearGenero(CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore, IMapper mapper) {
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            var id = await repositorioGeneros.CrearGenero(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default); //Limpiar cache
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return TypedResults.Created($"/generos/{id}", generoDTO);
        }


        // Obtener todos los géneros
        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper, ILoggerFactory loggerFactory) {
            //Logger:
            var tipo = typeof(GenerosEndpoints);
            var logger = loggerFactory.CreateLogger(tipo.FullName!);
            logger.LogInformation("Obteniendo el listado de generos");

            logger.LogCritical("Este es un log critical");
            logger.LogError("Este es un log error");
            logger.LogWarning("Este es un log warning");
            logger.LogInformation("Este es un log information");
            logger.LogDebug("Este es un log debug");
            logger.LogTrace("Este es un log Trace");

            var generos = await repositorio.ObtenerTodos();
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);
            return TypedResults.Ok(generosDTO);
        }


        // Obtener un género por ID
        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(int id, IRepositorioGeneros repositorioGeneros, IMapper mapper) {
            var genero = await repositorioGeneros.ObtenerPorId(id);
            if (genero is null) {
                return TypedResults.NotFound();
            }
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return TypedResults.Ok(generoDTO);
        }


        // Actualizar un género existente
        static async Task<Results<NoContent, NotFound, ValidationProblem>> ActualizarGenero(int id, CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore, IMapper mapper) {
            var existe = await repositorio.Existe(id);
            if (!existe) {
                return TypedResults.NotFound();
            }
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            genero.Id = id; //Como crearGenero no tiene el id ese se pasa manualmente  
            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }


        // Borrar un género por ID
        static async Task<Results<NoContent, NotFound>> BorrarGeneroPorId(int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore) {
            var existe = await repositorio.Existe(id);

            if (!existe) {
                return TypedResults.NotFound();
            }
            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

    }
}
