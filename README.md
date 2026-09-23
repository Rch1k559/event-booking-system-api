# Event Booking System API

High-performance Event Booking REST API built with **Clean Architecture**, **CQRS** pattern, and **ASP.NET Core** (.NET 8).

## 📐 Architecture Overview

The project follows **Clean Architecture** principles with clear separation of concerns across layers:

```text
EventBookingSystem/
├── src/
│   ├── Domain/                 # Core domain entities and enums
│   ├── Application/            # CQRS: Commands, Queries, DTOs, Validation, Pipeline Behaviors
│   ├── Infrastructure/         # EF Core, AppDbContext, JWT, Hashing, Background Services
│   └── WebApi/                 # Controllers, Middlewares, Auth Configuration, Program.cs
```

### Layer Responsibilities

1. **Domain Layer**
   - Core Entities: `User`, `Event`, `TicketType`, `Booking`, `BookingItem`, `AuditLog`.
   - User Roles: `Customer`, `Organizer`, `Admin`.
   - Event Statuses: `Draft`, `Published`, `Cancelled`.
   - Booking Statuses: `Pending`, `Confirmed`, `Cancelled`.

2. **Application Layer**
   - **CQRS**: Utilizes `MediatR` to separate read and write pipelines (`Commands` and `Queries`).
   - **Pipeline Behaviors**:
     - `LoggingBehavior`: Logs request execution times and correlation IDs (`CorrelationId`).
     - `ValidationBehavior`: Performs request validation using `FluentValidation`.
   - **DTOs & Models**: Pagination support (`PagedResult<T>`), authentication and booking response payloads.

3. **Infrastructure Layer**
   - **EF Core & PostgreSQL**: Model configurations (`IEntityTypeConfiguration`), indices, cascading deletes, and column precision (`Npgsql`).
   - **Security**: JWT token generation (`JwtTokenGenerator`) and BCrypt password hashing (`PasswordHasher`).
   - **Background Services**: `ExpiredBookingsCleanerHostedService` — a background worker that runs periodically (every 1 minute) to cancel expired pending bookings (`CancelExpiredBookingsCommand`).

4. **WebApi Layer**
   - **Controllers**: Endpoints for managing users, events, and ticket bookings.
   - **Middlewares**: `ExceptionHandlingMiddleware` for centralized error handling (Validation, NotFound, Conflict, Internal Server Error).

---

## 🛠 Tech Stack

- **Language & Framework**: C# / .NET 8
- **Architectural Patterns**: Clean Architecture, CQRS, Pipeline Pattern, Repository/UnitOfWork (via EF Core `IApplicationDbContext`)
- **Database**: PostgreSQL (Entity Framework Core + Npgsql)
- **Mediator**: MediatR
- **Validation**: FluentValidation
- **Authentication & Security**: JWT Bearer Tokens, BCrypt Password Hashing
- **Background Jobs**: ASP.NET Core Hosted Services (`IHostedService` / `BackgroundService`)

---

## 🚀 Key Features

### 1. User & Authentication Management
- User registration (`RegisterUserCommand`) with roles (`Customer`, `Organizer`). *Admin role registration is restricted at validation level.*
- User authentication (`LoginUserCommand`) returning JWT bearer tokens.
- Profile management (`GetCurrentUserQuery` at `/api/user/me`).

### 2. Event Management
- Event creation by organizers in `Draft` state with ticket category definitions (`CreateEventCommand`).
- Event publication (`PublishEventCommand`) with authorization checks and notification dispatch (`EventPublishedNotification`).
- Paginated event catalog listing with filtering and search (`GetPublishedEventsQuery`).
- Detailed event information retrieval (`GetEventDetailsQuery`).

### 3. Bookings & Ticket Reservations
- Ticket reservation (`ReserveTicketsCommand`) with a temporary 15-minute seat lock (`ExpiresAt`).
- Booking confirmation (`ConfirmBookingCommand`).
- Automatic cancellation of expired pending reservations via a background worker (`ExpiredBookingsCleanerHostedService`), restoring available ticket inventory.

---

## 📡 REST API Reference

### Authentication & Users (`/api/User`)
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/user/register` | Public | Register a new user |
| `POST` | `/api/user/login` | Public | Authenticate user & retrieve JWT token |
| `GET` | `/api/user/me` | Bearer Auth | Get current user profile |

### Events (`/api/Events`)
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/events` | Organizer, Admin | Create a new event |
| `PUT` | `/api/events/{id}/publish` | Organizer | Publish an event |
| `GET` | `/api/events` | Public | List and filter published events |
| `GET` | `/api/events/{id}` | Public | Get detailed event info |

### Bookings (`/api/Bookings`)
| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/bookings` | Customer | Reserve tickets |
| `POST` | `/api/bookings/{id}/confirm` | Customer | Confirm ticket reservation |

---

## ⚙️ Configuration & Getting Started

### Prerequisites
- **.NET 8.0 SDK** or later
- **PostgreSQL Database**

### Configuration (`appsettings.json`)

Configure `appsettings.json` in the `WebApi` project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=EventBookingDb;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "Secret": "YourSUPER_Secret_Key_At_Least_32_Characters_Long!",
    "Issuer": "EventBookingApi",
    "Audience": "EventBookingClient",
    "ExpiryMinutes": "60"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Running the Application

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/event-booking-system.git
   cd event-booking-system
   ```

2. **Apply database migrations**:
   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/WebApi
   ```

3. **Run the REST API**:
   ```bash
   dotnet run --project src/WebApi
   ```

The API will be accessible at `http://localhost:5000` (or `https://localhost:5001`).

---

## 🛡 Exception Handling

The application incorporates a global middleware (`ExceptionHandlingMiddleware`) that catches unhandled exceptions and produces standardized JSON error responses:

- `400 Bad Request` — Validation failures (`FluentValidation`) with detailed field errors.
- `404 Not Found` — Resource missing (`NotFoundException`).
- `409 Conflict` — Business operation state conflicts (`InvalidOperationException`).
- `500 Internal Server Error` — Unhandled server exceptions.
