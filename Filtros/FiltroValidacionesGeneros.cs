using AnimalApiPeliculas.DTOs;
using FluentValidation;

namespace AnimalApiPeliculas.Filtros {
      public class FiltroValidacionesGeneros : IEndpointFilter {

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {  // Método principal: Se ejecuta cuando el filtro es aplicado a un endpoint


            var validador = context.HttpContext.RequestServices.GetService<IValidator<CrearGeneroDTO>>();  // Obtiene el servicio de validación para CrearGeneroDTO

            if (validador is null) {  // Si no se encuentra un validador, continúa con el siguiente filtro o endpoint
                return await next(context);
            }

            var InsumoValidar = context.Arguments.OfType<CrearGeneroDTO>().FirstOrDefault(); // Extrae el objeto CrearGeneroDTO de los argumentos.

            if (InsumoValidar is null) {  // Si no se encuentra el objeto a validar, retorna un error indicando que no se pudo validar
                return TypedResults.Problem("No pudo ser encontrada la entidad a validar");
            }

            // Ejecuta la validación del objeto utilizando el validador obtenido anteriormente
            var resultadoValidacion = await validador.ValidateAsync(InsumoValidar);

            if (!resultadoValidacion.IsValid) {  // Si la validación falla, retorna los errores encontrados
                return TypedResults.ValidationProblem(resultadoValidacion.ToDictionary());
            }

            // Si la validación pasa, continúa con el siguiente filtro o endpoint
            return await next(context);
        }
    }
}
