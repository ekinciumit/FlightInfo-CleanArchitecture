using FluentValidation;
using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Validators
{
    /// <summary>
    /// City validation rules
    /// </summary>
    public class CityValidator : AbstractValidator<CityDto>
    {
        public CityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Şehir adı gerekli")
                .MaximumLength(100).WithMessage("Şehir adı en fazla 100 karakter olabilir");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Şehir kodu gerekli")
                .MaximumLength(10).WithMessage("Şehir kodu en fazla 10 karakter olabilir")
                .Matches("^[A-Z]{3,10}$").WithMessage("Şehir kodu büyük harflerle olmalı");

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage("Ülke ID gerekli");
        }
    }
}


