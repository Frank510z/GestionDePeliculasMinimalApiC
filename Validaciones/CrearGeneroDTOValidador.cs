using AnimalApiPeliculas.DTOs;
using AnimalApiPeliculas.Repositorios;
using FluentValidation;
using System.Security.Cryptography;

namespace AnimalApiPeliculas.Validaciones {
    public class CrearGeneroDTOValidador : AbstractValidator<CrearGeneroDTO> {

        public CrearGeneroDTOValidador(IRepositorioGeneros repositorioGeneros, IHttpContextAccessor httpContextAccessor) {

            // Obtener el ID desde la ruta
            var valorDeRutaId = httpContextAccessor.HttpContext?.Request.RouteValues["Id"];
            var id = 0; // Valor predeterminado

            if (valorDeRutaId is string valorString) { // Si el valor es posible pasarlo a int (si es un numero), valorDeRutaId toma el nombre de valorString para esta condicion
                int.TryParse(valorString, out id); //es lo mismo valorString y valorDeRutaId
            }
            // si no llega el id (caso de que lo creeemos el genro) se queda en id = 0

            RuleFor(x => x.Nombre).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                                  .MaximumLength(50).WithMessage(Utilidades.CampoMaximoDeCacteresMensaje)
                                  .Must(Utilidades.PrimeraLetraEnMayuscula).WithMessage(Utilidades.PrimeraLetraMayusculaMensaje)
                                  .MustAsync(async (nombre, _) => {
                                      var existe = await repositorioGeneros.Existe(id, nombre);  // le pasamos la variable id y nombre
                                      return !existe;
                                  }).WithMessage(g => $"Ya existe un genero con el nombre {g.Nombre}");

        }


    }
}