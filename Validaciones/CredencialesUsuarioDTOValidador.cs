using AnimalApiPeliculas.DTOs;
using FluentValidation;

namespace AnimalApiPeliculas.Validaciones {
    public class CredencialesUsuarioDTOValidador : AbstractValidator<CredencialesUsuarioDTO> {

        public CredencialesUsuarioDTOValidador() {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje).MaximumLength(256).WithMessage(Utilidades.CampoMaximoDeCacteresMensaje).EmailAddress().WithMessage(Utilidades.EmailMensaje);

            RuleFor(x => x.Password).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje);

        }

    }
}
