using Application.Events.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.FluentValidEvents
{
    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Name Title is required!")
                .MinimumLength(5).WithMessage("Minimum name Title length is 5")
                .MaximumLength(100).WithMessage("Maximum name Title length is 100");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required!")
                .LessThan(x => DateTime.Now).WithMessage("Date must be less than 24 hours");

            RuleFor(x => x.TicketTypes)
                .NotEmpty().WithMessage("TicketTypes is required!");

            RuleForEach(x => x.TicketTypes)
                .SetValidator(new CreateTicketTypeDtoValidator());
        }
    }
}
