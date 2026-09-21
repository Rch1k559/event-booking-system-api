using Application.Events.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.FluentValidEvents
{
    public class ReserveTicketsCommandValidator : AbstractValidator<ReserveTicketsCommand>
    {
        public ReserveTicketsCommandValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Items should not be empty");

            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId should not be empty");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId should not be empty");

            RuleForEach(x => x.Items).SetValidator(new ReverseTicketItemDtoValidator());
        }
    }
}
