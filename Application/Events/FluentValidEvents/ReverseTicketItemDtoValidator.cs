using Application.Events.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.FluentValidEvents
{
    public class ReverseTicketItemDtoValidator : AbstractValidator<ReverseTicketItemDto>
    {
        public ReverseTicketItemDtoValidator()
        {
            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity should not be empty")
                .InclusiveBetween(1, 5);

            RuleFor(x => x.TicketTypeId)
                .NotEmpty().WithMessage("TicketTypeId should not be empty");
        }
    }
}
