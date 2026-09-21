using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Events.Commands
{
    public record CancelExpiredBookingsCommand() : IRequest;
}
