# Event Booking System API

Высокопроизводительный REST API сервиса бронирования билетов на мероприятия, построенный с использованием **Clean Architecture**, паттерна **CQRS** и фреймворка **ASP.NET Core**.

---

## Архитектура проекта

Проект реализован по принципам **Clean Architecture** (Чистая архитектура) с разделением ответственности по слоям:

```text
EventBookingSystem/
├── src/
│   ├── Domain/                 # Ядро системы: доменные сущности и перечисления (Enums)
│   ├── Application/            # CQRS: Команды, Запросы, DTOs, Валидация, Pipeline Behaviors
│   ├── Infrastructure/         # EF Core, AppDbContext, JWT, Hashing, Background Services
│   └── WebApi/                 # Controllers, Middlewares, Настройки аутентификации, Program.cs
```

### Разделение по слоям

1. **Domain (Доменный слой)**
   - Сущности: `User`, `Event`, `TicketType`, `Booking`, `BookingItem`, `AuditLog`.
   - Роли пользователей: `Customer`, `Organizer`, `Admin`.
   - Статусы мероприятий: `Draft`, `Published`, `Cancelled`.
   - Статусы бронирования: `Pending`, `Confirmed`, `Cancelled`.

2. **Application (Прикладной слой)**
   - **CQRS**: Использование библиотеки `MediatR` для разделения чтения и записи (`Commands` и `Queries`).
   - **Pipeline Behaviors**:
     - `LoggingBehavior`: Автоматическое логирование всех запросов с корреляционными ID (`CorrelationId`) и замером времени выполнения.
     - `ValidationBehavior`: Автоматическая валидация всех входящих команд через `FluentValidation`.
   - **DTOs & Models**: Пагинированные результаты (`PagedResult<T>`), структуры ответов на авторизацию и бронирование.

3. **Infrastructure (Слой инфраструктуры)**
   - **EF Core & PostgreSQL**: Конфигурации моделей (`IEntityTypeConfiguration`), индексы, каскадные удаления и точность типов (`Npgsql`).
   - **Security**: Генерация JWT-токенов (`JwtTokenGenerator`) и хеширование паролей (`PasswordHasher` на базе BCrypt).
   - **Background Worker**: `ExpiredBookingsCleanerHostedService` — фоновая служба, периодически (каждую минуту) отменяющая просроченные бронирования (`CancelExpiredBookingsCommand`).

4. **WebApi (Слой представления)**
   - **Controllers**: Эндпоинты управления пользователями, мероприятиями и бронированиями.
   - **Middlewares**: `ExceptionHandlingMiddleware` для централизованной обработки ошибок (Validation, NotFound, Conflict, Internal Server Error).

---

## Технологический стек

- **Язык & Платформа**: C# / .NET 8+
- **Архитектурные паттерны**: Clean Architecture, CQRS, Pipeline Pattern, Repository/UnitOfWork (через EF Core `IApplicationDbContext`)
- **База данных**: PostgreSQL (Entity Framework Core + Npgsql)
- **Медиатор**: MediatR
- **Валидация**: FluentValidation
- **Аутентификация & Безопасность**: JWT Bearer Tokens, BCrypt Password Hashing
- **Фоновые задачи**: ASP.NET Core Hosted Services (`IHostedService` / `BackgroundService`)

---

## Основной функционал

### 1. Пользователи и Авторизация
- Регистрация пользователей (`RegisterUserCommand`) с ролями (`Customer`, `Organizer`). *Регистрация роли Admin заблокирована на уровне валидации.*
- Аутентификация (`LoginUserCommand`) с выдачей JWT-токена.
- Профиль текущего пользователя (`GetCurrentUserQuery` по эндпоинту `/api/user/me`).

### 2. Управление Мероприятиями (Events)
- Создание мероприятий организаторами в статусе `Draft` с возможностью указания типов билетов (`CreateEventCommand`).
- Публикация мероприятий (`PublishEventCommand`) с проверкой прав доступа организатора и отправкой уведомлений (`EventPublishedNotification`).
- Просмотр списка опубликованных мероприятий с фильтрацией, поиском и пагинацией (`GetPublishedEventsQuery`).
- Получение детальной информации о мероприятии (`GetEventDetailsQuery`).

### 3. Бронирование и Билеты (Bookings & Tickets)
- Резервирование билетов (`ReserveTicketsCommand`) с временной блокировкой мест. При резервировании устанавливается срок действия брони (15 минут).
- Подтверждение бронирования (`ConfirmBookingCommand`).
- Автоматическая отмена просроченных броней фоновым сервисом с возвратом доступного количества билетов (`ExpiredBookingsCleanerHostedService`).

---

## REST API Эндпоинты

### Аутентификация и Пользователи (`/api/User`)
| Метод | Эндпоинт | Доступ | Описание |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/user/register` | Публичный | Регистрация нового пользователя |
| `POST` | `/api/user/login` | Публичный | Вход в систему и получение JWT |
| `GET` | `/api/user/me` | Bearer Auth | Получение профиля текущего пользователя |

### Мероприятия (`/api/Events`)
| Метод | Эндпоинт | Доступ | Описание |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/events` | Organizer, Admin | Создание нового мероприятия |
| `PUT` | `/api/events/{id}/publish` | Organizer | Публикация мероприятия |
| `GET` | `/api/events` | Публичный | Поиск и пагинация опубликованных событий |
| `GET` | `/api/events/{id}` | Публичный | Детальная информация о событии |

### Бронирования (`/api/Bookings`)
| Метод | Эндпоинт | Доступ | Описание |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/bookings` | Customer | Резервирование билетов |
| `POST` | `/api/bookings/{id}/confirm` | Customer | Подтверждение бронирования |

---

## Настройка и Запуск

### Предварительные требования
- **.NET 8.0 SDK** или новее
- **PostgreSQL Database**

### Конфигурация (`appsettings.json`)

Создайте или настройте файл `appsettings.json` в проекте `WebApi`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=EventBookingDb;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "Secret": "YourSUPER_Secret_Key_At_Least_32_Characters_Long!",
    "Issuer": "TicketCraft",
    "Audience": "TicketCraftClient",
    "ExpiryMinutes": "120"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Запуск проекта

1. **Клонируйте репозиторий**:
   ```bash
   git clone https://github.com/your-username/event-booking-system.git
   cd event-booking-system
   ```

2. **Примените миграции базы данных**:
   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/WebApi
   ```

3. **Запустите REST API**:
   ```bash
   dotnet run --project src/WebApi
   ```

API будет доступно по адресу `http://localhost:5000` (или `https://localhost:5001`).

---

## Обработка Ошибок

В проекте настроен глобальный Middleware (`ExceptionHandlingMiddleware`), который перехватывает исключения и возвращает форматированный JSON-ответ:

- `400 Bad Request` — ошибки валидации `FluentValidation` с детализацией по полям.
- `404 Not Found` — запрашиваемый ресурс (событие/пользователь) не найден (`NotFoundException`).
- `409 Conflict` — бизнес-конфликты (`InvalidOperationException`).
- `500 Internal Server Error` — непредвиденные ошибки сервера.
