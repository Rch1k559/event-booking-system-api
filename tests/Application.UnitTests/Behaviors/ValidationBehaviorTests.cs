using Application.Behaviors;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;

namespace Application.UnitTests.Behaviors
{
    public class ValidationBehaviorTests
    {
        public record SampleCommand(string Name) : IRequest<string>;

        public class SampleCommandValidator : AbstractValidator<SampleCommand>
        {
            public SampleCommandValidator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            }
        }

        [Fact]
        public async Task Handle_InvalidRequest_ShouldThrowValidationException()
        {
            // Arrange
            var validators = new List<IValidator<SampleCommand>> { new SampleCommandValidator() };
            var behavior = new ValidationBehavior<SampleCommand, string>(validators);
            var request = new SampleCommand(string.Empty); 

            var nextDelegate = new Mock<RequestHandlerDelegate<string>>();

            // Act & Assert
            var act = async () => await behavior.Handle(request, nextDelegate.Object, CancellationToken.None);
            await act.Should().ThrowAsync<ValidationException>();

            nextDelegate.Verify(n => n(), Times.Never); 
        }
    }
}
