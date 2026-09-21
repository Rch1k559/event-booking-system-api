using Application.Common.Intefaces;
using Application.UnitTests.Common;
using Application.Users.Commands;
using Application.Users.Handlers;
using Domain;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.Users
{
    public class UsersHandlersTests
    {
        private readonly IApplicationDbContext _context;
        private readonly Mock<IPasswordHasher> _hasherMock = new();
        private readonly Mock<IJwtTokenGenerator> _jwtMock = new();

        public UsersHandlersTests()
        {
            _context = TestDbContextFactory.Create();
        }

        [Fact]
        public async Task Register_NewUser_ShouldHashPasswordAndSaveUser()
        {
            // Arrange
            _hasherMock.Setup(h => h.HashPassword("Secret123!")).Returns("hashed_secret");
            var handler = new RegisterUserCommandHandler(_context, _hasherMock.Object);
            var command = new RegisterUserCommand("test@mail.com", "Secret123!", "John", "Doe", UserRole.Customer);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNullOrEmpty();
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == "test@mail.com");
            userInDb.Should().NotBeNull();
            userInDb!.PasswordHash.Should().Be("hashed_secret");
            userInDb.Role.Should().Be(UserRole.Customer);
        }

        [Fact]
        public async Task Register_ExistingEmail_ShouldThrowException()
        {
            // Arrange
            _context.Users.Add(new User { Id = Guid.NewGuid(), Email = "busy@mail.com", PasswordHash = "hash", Role = UserRole.Customer });
            await _context.SaveChangesAsync(CancellationToken.None);

            var handler = new RegisterUserCommandHandler(_context, _hasherMock.Object);
            var command = new RegisterUserCommand("busy@mail.com", "Secret123!", "John", "Doe", UserRole.Customer);

            // Act & Assert
            var act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>().WithMessage("This Email already busy");
        }

        [Fact]
        public async Task Login_ValidCredentials_ShouldReturnJwtToken()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "user@mail.com", PasswordHash = "valid_hash", Role = UserRole.Customer };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(CancellationToken.None);

            _hasherMock.Setup(h => h.VerifyPassword("Pass123!", "valid_hash")).Returns(true);
            _jwtMock.Setup(j => j.GenerateToken(user)).Returns("mocked_jwt_token");

            var handler = new LoginUserCommandHandler(_context, _hasherMock.Object, _jwtMock.Object);

            // Act
            var response = await handler.Handle(new LoginUserCommand("user@mail.com", "Pass123!"), CancellationToken.None);

            // Assert
            response.Token.Should().Be("mocked_jwt_token");
            response.Email.Should().Be("user@mail.com");
        }
    }
}
