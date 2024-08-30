using AnimalApiPeliculas.DTOs;
using FluentValidation;

namespace AnimalApiPeliculas.Validaciones {
    public class CrearActorDTOValidator : AbstractValidator<CrearActorDTO> {
        public CrearActorDTOValidator() {

            RuleFor(x => x.Nombre).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                                             .MaximumLength(150).WithMessage(Utilidades.CampoMaximoDeCacteresMensaje);

            var fechaMinima = new DateTime(1990, 1, 1);

            RuleFor(X => X.FechaNacimiento).GreaterThanOrEqualTo(fechaMinima).WithMessage(Utilidades.FechaMayorOIgualMensaje(fechaMinima));

        }

    }
}
