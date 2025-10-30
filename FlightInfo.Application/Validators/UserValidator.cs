using FluentValidation;
using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Validators
{
    /// <summary>
    /// User validation rules
    /// </summary>
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email gerekli")
                .EmailAddress().WithMessage("Geçerli email adresi giriniz")
                .MaximumLength(100).WithMessage("Email en fazla 100 karakter olabilir");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Ad soyad gerekli")
                .MaximumLength(100).WithMessage("Ad soyad en fazla 100 karakter olabilir");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Rol gerekli")
                .MaximumLength(20).WithMessage("Rol en fazla 20 karakter olabilir");
        }
    }
}


