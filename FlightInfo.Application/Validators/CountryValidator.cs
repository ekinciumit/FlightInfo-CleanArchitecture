using FluentValidation;
using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Validators
{
    /// <summary>
    /// Country validation rules
    /// </summary>
    public class CountryValidator : AbstractValidator<CountryDto>
    {
        public CountryValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Ülke kodu gerekli")
                .Length(2, 3).WithMessage("Ülke kodu 2-3 karakter olmalı")
                .Matches("^[A-Z]{2,3}$").WithMessage("Ülke kodu büyük harflerle olmalı");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ülke adı gerekli")
                .MaximumLength(100).WithMessage("Ülke adı en fazla 100 karakter olabilir");
        }
    }
}


