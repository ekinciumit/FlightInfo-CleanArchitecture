using FluentValidation;
using FlightInfo.Shared.DTOs;

namespace FlightInfo.Application.Validators
{
    /// <summary>
    /// Reservation validation rules
    /// </summary>
    public class ReservationValidator : AbstractValidator<ReservationDto>
    {
        public ReservationValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı ID gerekli");

            RuleFor(x => x.FlightId)
                .GreaterThan(0).WithMessage("Uçuş ID gerekli");

            RuleFor(x => x.PassengerName)
                .NotEmpty().WithMessage("Yolcu adı gerekli")
                .MaximumLength(100).WithMessage("Yolcu adı en fazla 100 karakter olabilir");

            RuleFor(x => x.PassengerEmail)
                .NotEmpty().WithMessage("Yolcu email gerekli")
                .EmailAddress().WithMessage("Geçerli email adresi giriniz")
                .MaximumLength(100).WithMessage("Email en fazla 100 karakter olabilir");

            RuleFor(x => x.PassengerPhone)
                .NotEmpty().WithMessage("Yolcu telefon gerekli")
                .MaximumLength(20).WithMessage("Telefon en fazla 20 karakter olabilir");

            RuleFor(x => x.SeatNumber)
                .NotEmpty().WithMessage("Koltuk numarası gerekli")
                .MaximumLength(10).WithMessage("Koltuk numarası en fazla 10 karakter olabilir");

            RuleFor(x => x.Class)
                .NotEmpty().WithMessage("Sınıf gerekli")
                .MaximumLength(20).WithMessage("Sınıf en fazla 20 karakter olabilir");

            RuleFor(x => x.TotalPrice)
                .GreaterThan(0).WithMessage("Toplam fiyat 0'dan büyük olmalı");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Para birimi gerekli")
                .MaximumLength(3).WithMessage("Para birimi en fazla 3 karakter olabilir");
        }
    }
}


