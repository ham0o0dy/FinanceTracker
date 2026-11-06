# Finance Tracker — Project 1 (core version)

ASP.NET Core Web API for a personal finance tracker: accounts, categories, income/expense
operations, transfers, and statistics, backed by PostgreSQL via EF Core.

## Prerequisites
- .NET 10 SDK
- PostgreSQL running locally

## Setup
1. Create a database named `financetracker` (e.g. via pgAdmin).
2. Copy `FinanceTracker/appsettings.Development.json.example` to
   `FinanceTracker/appsettings.Development.json` and fill in your real PostgreSQL password
   and a random 32+ character string for `Jwt:Key`. This file is gitignored — never commit it.
3. Open the solution in Visual Studio, set `FinanceTracker` as the startup project, and run (F5).
   `Program.cs` applies EF Core migrations and seeds demo data automatically on first run
   against an empty database.
4. Swagger UI is available at `/swagger` in Development.

## Demo credentials
- Email: `demo@fintracker.local`
- Password: `Demo12345`

## Project structure
- `FinanceTracker` — Api layer: controllers, Program.cs, configuration. No business logic.
- `FinanceTracker.BusinessLogic` — services (interfaces + implementations), DTOs, business
  rules (R1–R16).
- `FinanceTracker.DataAccess` — EF Core entities, DbContext, migrations, seed data.

## Implemented bonuses
- None yet.

## Configuration
- Accepted currency codes are read from `Currencies:Allowed` in `appsettings.json`
  (restart the app after changing them).
