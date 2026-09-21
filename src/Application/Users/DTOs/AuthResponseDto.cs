using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.DTOs
{
    public record AuthResponseDto(Guid UserId, string Email, string Token);
}
