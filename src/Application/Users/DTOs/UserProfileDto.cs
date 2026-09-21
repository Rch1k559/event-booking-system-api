using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.DTOs
{
    public record UserProfileDto(Guid Id, string Email, string Role);
}
