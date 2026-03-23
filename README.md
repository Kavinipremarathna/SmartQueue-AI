# SmartQueue Full-Stack Application

SmartQueue is a queue management web application with an ASP.NET Core backend and a React frontend.

## Highlights

- JWT-based authentication with role-based authorization (Admin, Staff, Customer)
- Smart queue with real-time updates through SignalR
- Appointment booking with auto slot adjustment to avoid congestion
- Admin controls for live queue and staff allocation
- Analytics: average wait, peak hour, service efficiency
- SQLite persistence (survives restart)
- Automated backend unit tests for queue logic, ticket creation, and auth

## Tech Stack

- Backend: ASP.NET Core (.NET 10), Entity Framework Core, SQLite
- Frontend: React + Vite
- Realtime: SignalR (WebSockets)
- Logging: Serilog
- Tests: xUnit

## Architecture

The application follows a **clean architecture style** with explicit boundaries:

1. **Controllers** - HTTP endpoints and role-based access
2. **Services** - Business logic (queue, auth, appointments, analytics)
3. **Repositories** - Data access abstractions and EF implementations
4. **DTOs** - Request/response contracts
5. **Entities** - Core domain models
6. **Infrastructure** - JWT token generation, SignalR hub/notifier, seeding

The React frontend communicates with the backend API via HTTP REST calls.

## Project Structure

- `Program.cs`: app startup, DI, CORS, DB initialization
- `Controllers/`: auth, queue, tickets, appointments, admin, analytics endpoints
- `Services/`: business logic + service interfaces
- `Repositories/`: repository interfaces and EF implementations
- `DTOs/`: API contracts
- `Entities/`: domain entities and constants
- `Infrastructure/`: JWT, SignalR, seeding
- `Data/AppDbContext.cs`: EF Core context
- `frontend/`: React app with dashboard UI
  - `frontend/src/components/`: reusable UI panels (login, queue, analytics, appointments)
  - `frontend/src/api/`: API client utilities
- `tests/SmartQueueAPI.Tests/`: backend unit tests

## Run Locally

### 1) Backend

```bash
dotnet restore
dotnet run --urls http://localhost:5055
```

Backend URL: `http://localhost:5055`

Default demo users:

- `admin` / `admin123`
- `staff` / `staff123`
- `customer` / `customer123`

### 2) Frontend

```bash
cd frontend
npm install
npm run dev -- --host --port 5173
```

Frontend URL: `http://localhost:5173`

## Test

```bash
dotnet test tests/SmartQueueAPI.Tests/SmartQueueAPI.Tests.csproj
```

## Build

```bash
dotnet build
cd frontend && npm run build
```

## API Endpoints

Base: `http://localhost:5055/api`

- `POST /auth/login` -> get JWT token
- `GET /queue/current` -> live queue with predicted wait
- `GET /queue/summary` -> queue summary (Admin/Staff)
- `POST /queue/serve-next` -> serve top waiting ticket (Admin/Staff)
- `POST /tickets` -> create ticket
- `PATCH /tickets/{id}/status` -> update status (Admin/Staff)
- `DELETE /tickets/{id}` -> delete ticket (Admin/Staff)
- `GET /tickets` -> all tickets (Admin/Staff)
- `POST /appointments` -> book appointment with auto-adjustment
- `GET /appointments` -> list appointments (Admin/Staff)
- `POST /admin/staff-allocation` -> update staff count (Admin)
- `GET /admin/live-queue` -> admin live queue view
- `GET /analytics` -> analytics metrics (Admin/Staff)

SignalR hub:

- `GET /hubs/queue` -> real-time queue update channel

## Persistence

- Development DB: `smartqueue.dev.db`
- Production/default DB: `smartqueue.v2.db`

Upgraded development schema file:

- `smartqueue.v2.dev.db`

These are created automatically on startup.

## Docker

Use Docker Compose to run both services:

```bash
docker compose up --build
```

- Frontend: `http://localhost:5173`
- Backend: `http://localhost:5055`

Docker Compose now runs PostgreSQL by default for the backend service.

- Postgres: `localhost:5432` (`smartqueue`/`smartqueue`)
- Backend provider in compose: `Database__Provider=Postgres`

## CI

GitHub Actions workflow in `.github/workflows/ci.yml` runs:

- backend restore/build/test
- backend auth + queue smoke test
- frontend install/build

## Notes

- CORS allows local frontend origins `http://localhost:5173` and `http://localhost:5174`.
- SQLite files are ignored by git.
