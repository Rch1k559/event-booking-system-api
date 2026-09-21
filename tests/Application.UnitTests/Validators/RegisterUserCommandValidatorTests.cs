using Application.Users.Commands;
using Application.Users.FluentValid;
using Domain;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators
{
    public class RegisterUserCommandValidatorTests
    {
        private readonly RegisterUserCommandValidator _validator = new();

        [Fact]
        public void Validator_InvalidEmail_ShouldHaveValidationError()
        {
            var command = new RegisterUserCommand("invalid-email", "Pass123!", "John", "Doe", UserRole.Customer);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validator_WeakPassword_ShouldHaveValidationError()
        {
            var command = new RegisterUserCommand("test@mail.com", "password", "John", "Doe", UserRole.Customer);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Validator_AdminRole_ShouldBeRejected()
        {
            var command = new RegisterUserCommand("admin@mail.com", "Pass123!", "Admin", "User", UserRole.Admin);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Role)
                  .WithErrorMessage("Admin role is not allowed for registration");
        }

        [Fact]
        public void Validator_ValidCustomer_ShouldNotHaveErrors()
        {
            var command = new RegisterUserCommand("valid@mail.com", "StrongPassword1!", "John", "Doe", UserRole.Customer);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
