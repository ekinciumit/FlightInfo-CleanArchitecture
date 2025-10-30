using FluentValidation;
using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Validators
{
    /// <summary>
    /// Flight validation rules
    /// </summary>
    public class FlightValidator : AbstractValidator<FlightDto>
    {
        public FlightValidator()
        {
            RuleFor(x => x.FlightNumber)
                .NotEmpty().WithMessage("Uçuş numarası gerekli")
                .MaximumLength(10).WithMessage("Uçuş numarası en fazla 10 karakter olabilir");

            RuleFor(x => x.Origin)
                .NotEmpty().WithMessage("Kalkış noktası gerekli")
                .MaximumLength(50).WithMessage("Kalkış noktası en fazla 50 karakter olabilir");

            RuleFor(x => x.Destination)
                .NotEmpty().WithMessage("Varış noktası gerekli")
                .MaximumLength(50).WithMessage("Varış noktası en fazla 50 karakter olabilir");

            RuleFor(x => x.DepartureTime)
                .NotEmpty().WithMessage("Kalkış saati gerekli")
                .GreaterThan(DateTime.UtcNow).WithMessage("Kalkış saati gelecekte olmalı");

            RuleFor(x => x.ArrivalTime)
                .NotEmpty().WithMessage("Varış saati gerekli")
                .GreaterThan(x => x.DepartureTime).WithMessage("Varış saati kalkış saatinden sonra olmalı");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Durum gerekli")
                .MaximumLength(20).WithMessage("Durum en fazla 20 karakter olabilir");
        }
    }
}


