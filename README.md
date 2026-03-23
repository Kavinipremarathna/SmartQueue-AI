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

## Docker Deployment

### Prerequisites

- Install [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Verify installation: `docker --version && docker-compose --version`

### Run with Docker Compose

```bash
# Start all services (PostgreSQL, Backend, Frontend)
docker-compose up -d --build

# Verify services are running
docker-compose ps

# Stop all services
docker-compose down

# Stop and remove database (fresh start)
docker-compose down -v
```

### Access Services

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5055
- **PostgreSQL**: localhost:5432 (internal, user: `smartqueue`, password: `smartqueue`)

### Services

1. **PostgreSQL 17 Alpine** - Production database
   - Accessible at `postgres:5432` (internal container network)
   - Credentials: user=`smartqueue`, password=`smartqueue`, db=`smartqueue`
   - Data persists in `postgres-data` volume

2. **Backend (.NET 10)**
   - Uses PostgreSQL via `Database__Provider=Postgres`
   - Connection: `Host=postgres;Port=5432;Database=smartqueue;Username=smartqueue;Password=smartqueue`
   - Seeded with demo users on startup

3. **Frontend (React 19 + Vite)**
   - Served on port 5173
   - Connects to backend at `http://localhost:5055`

### Docker Logs

```bash
# View all service logs
docker-compose logs -f

# View specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f postgres
```

### Troubleshooting

| Issue                        | Solution                                                     |
| ---------------------------- | ------------------------------------------------------------ |
| Port 5173 already in use     | Change `frontend` port in `docker-compose.yml`               |
| Port 5055 already in use     | Change `backend` port in `docker-compose.yml`                |
| Database connection fails    | Check backend logs: `docker-compose logs backend`            |
| Frontend can't reach backend | CORS is configured for `localhost` origins                   |
| Fresh start needed           | Run `docker-compose down -v && docker-compose up -d --build` |

## CI

GitHub Actions workflow in `.github/workflows/ci.yml` runs:

- backend restore/build/test
- backend auth + queue smoke test
- frontend install/build

## Modern UX Features

### Animated Entry Screen

- **Intro Screen** with flowing SVG curves and animated orbs
- "Are you ready?" text with staggered entrance animation
- Smooth fade transition to login form
- Responsive design for mobile & desktop

### Interactive Components

- Form inputs with focus glow effects
- Buttons with hover lift animation
- Staggered animations for form elements
- Error messages with styled backgrounds
- Real-time queue updates via SignalR

### Tech Stack for UI

- React 19 with Vite 8.0.1 for fast bundling
- CSS3 animations with cubic-bezier easing
- Vector graphics (SVG curves) for smooth animations
- React Router DOM 7.2.0 for role-based navigation

## Notes

- CORS allows local frontend origins `http://localhost:5173` and `http://localhost:5174`
- Demo data is seeded automatically on app startup
- All APIs require JWT bearer token authentication (except POST /api/auth/login)
- SQLite files are ignored by git.
