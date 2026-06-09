# ScoutNet

Football scouting workspace for searching players, comparing stats on a radar chart, managing a personal watchlist, and writing scout reports. Player data is synced from [API-Football](https://www.api-football.com/) into a local PostgreSQL database.

## Live deployment (dev)

| Component | URL |
|-----------|-----|
| **Frontend** | [https://jolly-field-059323810.7.azurestaticapps.net](https://jolly-field-059323810.7.azurestaticapps.net) |
| **Backend API** | [https://scoutnet-api-dev-fjhfcfedbxhghcfe.polandcentral-01.azurewebsites.net](https://scoutnet-api-dev-fjhfcfedbxhghcfe.polandcentral-01.azurewebsites.net) |
| **Swagger UI** | [https://scoutnet-api-dev-fjhfcfedbxhghcfe.polandcentral-01.azurewebsites.net/swagger](https://scoutnet-api-dev-fjhfcfedbxhghcfe.polandcentral-01.azurewebsites.net/swagger) |

The deployed frontend reads the API URL from `public/config.json` at build time (generated in CI from the `SCOUTNET_API_URL` repository variable).

## Features

- **Player database** — browse players by country, league, club, and season; advanced filters for Scout/Admin roles
- **Player card** — detailed profile and season statistics
- **Radar comparison** — compare two players side by side (pace, shooting, passing, dribbling, defending, physicality)
- **Watchlist** — save players for quick access (Scout/Admin)
- **Scout reports** — create and manage transfer recommendations (Scout/Admin)
- **Admin panel** — manage user roles (Admin)

### Access levels

| Role | Capabilities |
|------|--------------|
| **Guest** | Basic player list (country / league / club filters) |
| **Scout** | Advanced filters, comparison, watchlist, reports |
| **Admin** | Everything Scout can do + user role management |

## Tech stack

| Layer | Stack |
|-------|-------|
| **Frontend** | React 19, TypeScript, Vite, Tailwind CSS, Axios |
| **Backend** | ASP.NET Core 10, Clean Architecture (Domain / Application / Infrastructure / WebAPI) |
| **Database** | PostgreSQL (EF Core) |
| **Auth** | JWT (BCrypt password hashing) |
| **External data** | API-Football |

## Repository structure

```
ScoutNet/
├── ScoutNet.UI/              # React frontend (Vite)
├── ScoutNet.API/
│   ├── ScoutNet.Domain/      # Entities, enums, specifications
│   ├── ScoutNet.Application/ # Services, DTOs, validators
│   ├── ScoutNet.Infrastructure/ # EF Core, repositories, API-Football client
│   └── ScoutNet.WebAPI/      # REST API, JWT, Swagger
└── .github/workflows/        # CI/CD for Azure
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 20+ (for the UI)
- PostgreSQL database (local or hosted, e.g. Supabase)
- [API-Football](https://www.api-football.com/) API key

## Local development

### 1. Clone the repository

```bash
git clone <repository-url>
cd ScoutNet
```

### 2. Configure the API

Edit `ScoutNet.API/ScoutNet.WebAPI/appsettings.json` (or use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)) with:

| Setting | Description |
|---------|-------------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `ApiFootball:ApiKey` | Your API-Football key |
| `Jwt:Key` | Secret key, at least 32 characters |
| `Cors:AllowedOrigins` | Include `http://localhost:4200` for local UI |

Do **not** commit real secrets to git. Use User Secrets or environment variables in production.

### 3. Apply database migrations

From the repository root:

```bash
dotnet ef database update \
  --project ScoutNet.API/ScoutNet.Infrastructure/ScoutNet.Infrastructure.csproj \
  --startup-project ScoutNet.API/ScoutNet.WebAPI/ScoutNet.WebAPI.csproj
```

Install the EF CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

### 4. Run the API

```bash
cd ScoutNet.API/ScoutNet.WebAPI
dotnet run
```

The API starts at **http://localhost:5000**. Swagger is available at [http://localhost:5000/swagger](http://localhost:5000/swagger).

### 5. Run the frontend

```bash
cd ScoutNet.UI
npm install
npm run dev
```

The UI starts at **http://localhost:4200** and talks to `http://localhost:5000/api` by default.

Optional: create `ScoutNet.UI/.env.local`:

```env
VITE_API_URL=http://localhost:5000/api
```

Or copy the runtime config example:

```bash
cp ScoutNet.UI/public/config.json.example ScoutNet.UI/public/config.json
```

### 6. Create an account

Open the UI, switch to **Register**, and create a Scout account. The first registered users get the **Scout** role by default; promote to **Admin** directly in the database or via an existing Admin user.

## API overview

All routes are prefixed with `/api`.

| Area | Base route |
|------|------------|
| Auth | `/api/auth` (login, register, me) |
| Players | `/api/players` (list, details, compare) |
| Reference | `/api/reference` (countries, leagues, teams) |
| Watchlist | `/api/watchlist` |
| Reports | `/api/reports` |
| Admin | `/api/admin/users` |

## Deployment

Deployments run automatically via GitHub Actions.

### Frontend — Azure Static Web Apps

- **Workflow:** `.github/workflows/azure-static-web-apps-jolly-field-059323810.yml`
- **Trigger:** push to `main` (and PR previews)
- **Target:** [Azure Static Web Apps](https://jolly-field-059323810.7.azurestaticapps.net)
- **Build:** Vite production build from `ScoutNet.UI/`
- **Config:** repository variable `SCOUTNET_API_URL` (e.g. `https://scoutnet-api-dev-....azurewebsites.net/api`) is written into `public/config.json` during CI

### Backend — Azure App Service

- **Workflow:** `.github/workflows/deploy-dev.yml`
- **Trigger:** push to `main` or `develop`, or manual dispatch
- **Target:** Azure App Service `scoutnet-api-dev` (Poland Central)
- **Steps:** restore → build → EF migrations → publish → deploy
- **Secrets required:**
  - `SCOUTNET_DEV_CONNECTION_STRING` — PostgreSQL connection string
  - `AZURE_WEBAPP_PUBLISH_PROFILE_SCOUTNET_API_DEV` — App Service publish profile

Azure App Service application settings should mirror `appsettings.json` sections (`ConnectionStrings`, `ApiFootball`, `Jwt`, `Cors`) with production values. Add the Static Web Apps frontend URL to `Cors:AllowedOrigins` so the deployed UI can call the API.

## Useful commands

```bash
# Frontend — production build
cd ScoutNet.UI && npm run build

# Backend — build solution
dotnet build ScoutNet.API/ScoutNet.API.slnx

# Backend — run tests (if added)
dotnet test ScoutNet.API/ScoutNet.API.slnx
```

## License

Private project — all rights reserved unless stated otherwise.
