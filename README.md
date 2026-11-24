# GLMS Contract Management

A contract management system for managing clients, contracts and service requests. It includes currency conversion and service-level rules. The repo holds two stages of the project:

| Folder | What it is |
|---|---|
| `GLMS_ST10445830_Solution` | **Final version**: an ASP.NET Core Web API (JWT auth, Swagger) with an MVC front end, unit tests and a Docker Compose setup (.NET 8) |
| `GLMS_ST10445830_Submission` | **Part 2**: a single ASP.NET Core MVC app with unit tests (.NET 10) |

## Running it

### With Docker (final version)

Requires [Docker Desktop](https://www.docker.com/products/docker-desktop/).

```bash
cd GLMS_ST10445830_Solution
docker compose up --build
```

- Web app: http://localhost:5000
- API: http://localhost:5001

This also starts a SQL Server container.

### Without Docker

Requires the .NET 8 SDK (and the .NET 10 SDK for Part 2) and SQL Server LocalDB, which comes with Visual Studio.

Open `GLMS_ST10445830_Solution/GLMS_ST10445830_Solution.slnx` in Visual Studio, set the API and web projects as startup projects, and press **F5**. Run the tests from **Test Explorer**, or with `dotnet test`.
