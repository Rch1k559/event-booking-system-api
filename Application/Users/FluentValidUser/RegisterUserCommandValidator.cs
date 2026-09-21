using Application.Users.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.FluentValid
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Matches(@"^(?=.*\d)(?=.*[^\w\s])").WithMessage("Password must contain at least one number and one special character.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Role must be a valid enum value: Organizer or Customer");

            RuleFor(x => x.Role)
            .Must(role => !role.HasValue || role.Value != UserRole.Admin)
            .WithMessage("Admin role is not allowed for registration");
        }
    }
}
