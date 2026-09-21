using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Commands
{
    public record PublishEventCommand(Guid EventId, Guid UserId) : IRequest<Guid>;
}
