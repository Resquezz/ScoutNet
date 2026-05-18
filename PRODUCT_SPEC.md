# PRODUCT SPECIFICATION (PRD) — ScoutNet

## 1. Project Overview

ScoutNet is a data-driven web application designed for football scouts, tactical analysts, and club managers. The platform serves as a professional workspace where users can filter global player databases using advanced statistical metrics, visually compare player performance through interactive diagrams, and manage structured scouting reports within an internal CRM system.

## 2. Core Problem & Solution

*   **Problem:** Professional football data is often fragmented, expensive, or buried in complex, non-intuitive interfaces. Aspiring scouts, local academies, and smaller clubs struggle to find a centralized, affordable tool to run deep statistical queries (e.g., finding a young defensive midfielder with >85% passing accuracy) and document their expert analysis without relying on chaotic spreadsheets.
*   **Solution:** ScoutNet centralizes player performance data, providing an advanced search engine driven by real-world metrics ($xG$, passing precision, defensive actions). It features visual comparison tools (radar charts) and a structured reporting manager, enabling scouts to create actionable profiles, manage shortlists (Watchlists), and maintain an organized internal database.

## 3. Target Audience & Roles

*   **Guest (Unauthorized User):** Can view the landing page, explore public player profiles with basic statistics, and see generic analytics dashboards. Cannot use advanced filters, create comparison boards, or write reports.
*   **Authorized Scout:** A registered user who can access the advanced search engine, create dual-player comparison views, add players to their personal Watchlist, and create/edit/delete their own Scout Reports.
*   **Club Admin / Lead Scout:** Inherits all Scout permissions, plus the ability to manage user roles, delete inappropriate or outdated reports across the platform, and manage system-wide settings (e.g., configuring data seeding or API caching rules).

## 4. Key Features (MVP)

*   **Advanced Search & Specification Filtering:** A sidebar panel with precise controls (sliders for age, contract duration, match counts, and specific performance metrics like goal conversion, progressive passes, tackles per game). Queries must support multiple criteria simultaneously.
*   **Dual-Player Visual Comparison View:** A split-screen interface allowing scouts to select two players. The central feature is an interactive **Radar Chart ("spider web")** that overlays the performance profiles of both players across core attributes (Pace, Passing, Dribbling, Defending, Physicality, Shooting) with automatic color-coding and metric deltas.
*   **Scout Reports Manager (Internal CRM):** A dedicated section within a player's profile where an authorized scout can fill out a form: rating Current Form (1-10), Potential (1-10), outlining explicit Pros & Cons (text blocks), and writing a final transfer recommendation Summary.
*   **Personal Watchlist (Favorites):** A simple dashboard where scouts can save, categorize, and monitor selected players, bypassing the need to re-run complex search queries.
*   **Data Seeding & Mock API Layer:** To prevent hitting strict external Football API free-tier limits during UI development, the system seeds a rich, static database of 50–100 real football players with accurate multi-season statistics into the local SQL Server.

## 5. Technical Architecture & Constraints

*   **Frontend Architecture & UI:** React (TypeScript) built with Vite. UI styling is managed strictly via **Tailwind CSS**. Data visualization relies on **Recharts** optimized for responsive rendering of radar and linear charts. Table operations use **TanStack Table** (React Table) for performant client-side sorting and pagination.
    
    The Frontend project structure follows a modular, feature-based design:
    ```text
    src/
    ├── assets/          # Static assets (images, logos, icons)
    ├── components/      # Reusable UI components (Buttons, Inputs, Modals, Layouts)
    ├── context/         # React Contexts (AuthContext, ThemeContext)
    ├── features/        # Feature-based modules (encapsulated logic)
    │   ├── search/      # Sidebar filters, TanStack player table
    │   ├── comparison/  # Split-screen view, Recharts Radar component
    │   ├── reports/     # CRM report forms, validation message blocks
    │   └── watchlist/   # Saved players dashboard grid
    ├── hooks/           # Custom reusable React hooks (useAuth, useFetch)
    ├── services/        # API communication layer (Axios client config)
    ├── types/           # Strict TypeScript interfaces matching .NET DTOs
    ├── App.tsx          # Main application component & React Router setup
    └── main.tsx         # Application entry point
    
Backend: ASP.NET Core Web API running on .NET 8 / .NET 9. The architecture follows a classic Clean Architecture approach based on layers and dependency injection, without CQRS or MediatR. The solution is divided into the following projects:

ScoutNet.Domain: Pure domain entities, enums, and core specifications interfaces (zero external dependencies).

ScoutNet.Application: Application interfaces, DTOs, FluentValidation rules, and business services implementation.

ScoutNet.Infrastructure: DB context, repository implementations, PostgreSQL specific configurations, and Data Seeding.

ScoutNet.WebAPI: Controllers, JWT Authentication setup, DI container configurations, and Global Exception Middleware.

Database & ORM: Managed PostgreSQL hosted on Supabase. Entity Framework Core (EF Core) with the Npgsql.EntityFrameworkCore.PostgreSQL provider is used for Object-Relational Mapping. Dynamic queries for the advanced filtering system must be implemented using the Specification Pattern or custom IQueryable extensions inside repositories/services to ensure clean, reusable, and type-safe LINQ code.

Validation & Security: All incoming API requests (e.g., creating scout reports) must be validated using FluentValidation before entering the service layer. Authentication is handled via JWT tokens (or ASP.NET Core Identity API endpoints).

Local Development Environment Network Ports:
For consistent local development, cross-origin resource sharing (CORS), and AI context alignment, the applications must strictly run on the following assigned ports:

Frontend (React UI): http://localhost:4200 (Configured via Vite's server.port in vite.config.ts).

Backend (Web API): http://localhost:5000 for HTTP and https://localhost:5001 for HTTPS (Configured via launchSettings.json in the WebAPI project).

CORS Policy: The backend must explicitly whitelist http://localhost:4200 in Development mode.

Infrastructure & Deployment (Azure):
The application architecture strictly separates Development (Dev) and Production (Prod) environments to ensure reliable release cycles.

Database Layer: Two separate Supabase projects are used: supabase-scoutnet-dev and supabase-scoutnet-prod.

Backend & Frontend Hosting: Hosted via Azure App Services (Linux plans).

Environments:

Dev Environment: Triggered automatically on every push to the main (or develop) branch. Deployed to scoutnet-api-dev.azurewebsites.net and scoutnet-app-dev.azurewebsites.net.

Prod Environment: Triggered manually or via release tags from the release branch. Deployed to scoutnet-api.azurewebsites.net and scoutnet.com (custom domain).

CI/CD: Managed via GitHub Actions workflows for automated building, testing, database migration execution, and deployment to respective Azure slots/services. Secret management (Connection Strings, JWT keys) is strictly handled via Azure App Service Configuration (Environment Variables) and GitHub Secrets.

6. System Design Guidelines for AI
When generating code, designing endpoints, or writing migrations for this project, the AI must strictly adhere to the following rules:

Layered Separation (Clean Architecture): Keep API Controllers thin. Controllers should only handle HTTP routing and data validation, then delegate business logic directly to specific Services (e.g., PlayerService, ReportService) inside the Application layer. Services interact with EF Core DbContext through Interfaces.

Type Safety: Maintain strict TypeScript types on the frontend matching the Data Transfer Objects (DTOs) returned by the C# backend. Avoid using any in React or untyped object results in .NET.

DRY Filters: Avoid writing messy, hardcoded if-else blocks inside EF Core queries for the player search. Use a structured filter DTO and map it cleanly to an expression tree or a Specification class inside the data layer.

Tailwind Utility Usage: When writing React components, stick to Tailwind utility classes. Avoid inline styles or custom external CSS files unless absolutely necessary for third-party chart adjustments.

7. Intellectual Property & Licensing
Proprietary Content: This specification outlines a unique product concept and system design. It is shared strictly for informational, educational review, and academic grading purposes.

Restrictions: Any unauthorized copying, modification, or distribution of this document or the concepts within for commercial use is prohibited without explicit written consent signed by the author.