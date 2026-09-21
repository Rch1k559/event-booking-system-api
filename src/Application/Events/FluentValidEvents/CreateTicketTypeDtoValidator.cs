using Application.Events.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.FluentValidEvents
{
    public class CreateTicketTypeDtoValidator : AbstractValidator<CreateTicketTypeDto>
    {
        public CreateTicketTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required!")
                .MaximumLength(50).WithMessage("Maximum Name length is 50");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.TotalQuantity)
                .GreaterThan(0).WithMessage("TotalQuantity must be greater than 0");
        }
    }
}
