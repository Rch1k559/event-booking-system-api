using Application.Events.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.FluentValidEvents
{
    public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
    {
        public ConfirmBookingCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty().WithMessage("BookingId should not be empty");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId should not be empty");
        }
    }
}
