using Application.Events.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.FluentValidEvents
{
    public class PublishEventCommandValidator : AbstractValidator<PublishEventCommand>
    {
        public PublishEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId not must be empty");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId not must be empty");
        }
    }
}
