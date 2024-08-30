
using AnimalApiPeliculas.Repositorios;
using AutoMapper;

namespace AnimalApiPeliculas.Filtros {
    public class FiltroDePrueba : IEndpointFilter {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {

            // Parametros 
            var paramEntero = context.Arguments.OfType<int>().FirstOrDefault(); //El primer parametro que sea un entero 
            var paramRepositorioGenero = context.Arguments.OfType<IRepositorioGeneros>().FirstOrDefault();
            var paramMapper = context.Arguments.OfType<IMapper>().FirstOrDefault();

            //Este codigo se ejecuta antes del endpoint 
            var resultado = await next(context);
            //Este se ejecuta despues del endpooint

            return resultado;

        }
    }
}
