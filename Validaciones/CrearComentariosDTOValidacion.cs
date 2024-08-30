using AnimalApiPeliculas.DTOs;
using FluentValidation;

namespace AnimalApiPeliculas.Validaciones {
    public class CrearComentariosDTOValidacion : AbstractValidator<CrearComentarioDTO> {
        public CrearComentariosDTOValidacion() {
            RuleFor(x => x.Cuerpo).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje);
        }

    }
}
