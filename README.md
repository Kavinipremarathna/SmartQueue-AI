# SmartQueue Full-Stack Application

SmartQueue is a queue management web application with an ASP.NET Core backend and a React frontend.

## Highlights

- Priority-based queue ordering
- Advanced ticket actions: serve next, status updates, delete
- Live dashboard metrics and search
- SQLite persistence (survives restart)
- Automated backend unit tests

## Tech Stack

- Backend: ASP.NET Core (.NET 10), Entity Framework Core, SQLite
- Frontend: React + Vite
- Tests: xUnit

## Architecture

The application follows a **3-layer architecture** for clean separation of concerns:

1. **Controllers Layer** - HTTP endpoints, request validation
2. **Services Layer** - Business logic, queue operations
3. **Data Layer** - Database access, EF Core mapping

The React frontend communicates with the backend API via HTTP REST calls.

## Project Structure

- `Program.cs`: app startup, DI, CORS, DB initialization
- `Controllers/QueueController.cs`: queue API endpoints
- `Services/QueueService.cs`: business logic
- `Data/AppDbContext.cs`: EF Core context
- `frontend/`: React app with dashboard UI
- `tests/SmartQueueAPI.Tests/`: backend unit tests

## Run Locally

### 1) Backend

```bash
dotnet restore
dotnet run --urls http://localhost:5055
```

Backend URL: `http://localhost:5055`

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

Base: `http://localhost:5055/api/queue`

- `GET /` -> waiting queue (priority sorted)
- `GET /all` -> all tickets
- `GET /summary` -> dashboard metrics
- `POST /add?name={name}&priority={0-10}` -> add ticket
- `POST /serve-next` -> serve highest-priority waiting ticket
- `PATCH /{id}/status?status=Waiting|Served|Cancelled` -> update status
- `DELETE /{id}` -> delete ticket

## Persistence

- Development DB: `smartqueue.dev.db`
- Production/default DB: `smartqueue.db`

These are created automatically on startup.

## Docker

Use Docker Compose to run both services:

```bash
docker compose up --build
```

- Frontend: `http://localhost:5173`
- Backend: `http://localhost:5055`

## CI

GitHub Actions workflow in `.github/workflows/ci.yml` runs:

- backend restore/build/test
- frontend install/build

## Notes

- CORS currently allows the local frontend origin `http://localhost:5173`.
- SQLite files are ignored by git.
