using AnimalApiPeliculas.DTOs;
using FluentValidation;

namespace AnimalApiPeliculas.Validaciones {
    public class EditarClaimDTOValidador : AbstractValidator<EditarClaimDTO> {

        public EditarClaimDTOValidador() {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje).MaximumLength(256).WithMessage(Utilidades.CampoMaximoDeCacteresMensaje).EmailAddress().WithMessage(Utilidades.EmailMensaje);
        }

    }
}
