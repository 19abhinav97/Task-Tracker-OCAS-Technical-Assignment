# TaskTracker API

A .NET 8 Web API for managing tasks, built as a N-Tier architecture (Controller → Service → Repository) with EF Core + SQLite and
a full xUnit unit-test suite.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)

### Start the API

dotnet run --project TaskTracker.Api

The API will listen on `https://localhost:5001` (and `http://localhost:5000`).  
The SQLite database (`tasks.db`) is created automatically on first run.

## How to run tests

dotnet test

All unit tests should pass (no database required — the repository is mocked).

## Architecture decisions

### N-Tier (Controller → Service → Repository)

Each layer has a single responsibility:

| Layer | Responsibility |
|---|---|
| **Controller** | HTTP concerns only — parse request, delegate to service, return response |
| **Service** | All business logic and domain rules |
| **Repository** | Pure data access — no business logic |

This keeps each layer independently testable and replaceable.  
Controllers and the repository are never tested directly; the service layer is.

### SQLite via EF Core

SQLite was chosen for portability: the database is a single file (`tasks.db`) with
no external server required, making setup trivial for a self-contained assignment.
In a production setting this would be swapped for PostgreSQL or SQL Server by
changing only the connection string and provider package.

### EF Core migrations applied on startup

`context.Database.Migrate()` is called in `Program.cs` so the schema is always
current when the app starts. This is the right trade-off for a development /
assignment context. In production, migrations would run as a separate pre-deploy
step to avoid startup latency and race conditions.

### `TaskStatus` namespace

The custom `TaskStatus` enum lives in `TaskTracker.Api.Models` to avoid a naming
collision with `System.Threading.Tasks.TaskStatus` from the BCL.

---

## Assumptions

1. `CreatedAt` is always stored as UTC (`DateTime.UtcNow`). Callers that need local
   time should convert on the client side.
2. `Status` in the JSON payload is accepted as either a string (`"Todo"`) or an
   integer (`0`); ASP.NET's default JSON deserialization handles both.
3. The `[Required]` annotation on `UpdateTaskDto.Status` relies on the ASP.NET
   `[ApiController]` automatic 400 response — no extra controller code is needed.
4. Soft-delete is out of scope; `DELETE /tasks/{id}` permanently removes the record.
5. No authentication or authorization is implemented. This is an internal API consumed
   by trusted clients only; all endpoints are open by design.
6. Duplicate task titles are permitted. `Id` is the unique identifier for a task — two
   tasks may share the same `Title`. No uniqueness constraint is enforced on `Title`.
7. `GET /tasks` returns all records with no pagination or filtering. This is acceptable
   for the scope of this assignment; a production API would add pagination to avoid 
   unbounded result sets.
8. Status transitions are unconstrained. Any valid status may be set to any other valid
   status (e.g. `Done → Todo` is allowed). The only enforced rule is the one stated in
   the assignment: a task cannot be marked `Done` with an empty title.
9. There is no `UpdatedAt` timestamp. Only `CreatedAt` is tracked; last-upadated auditing
   is out of scope for this assignment.
10. No rate limiting is implemented. As an internal API consumed by trusted clients.
11. An empty or whitespace-only `Title` is rejected on both create and update as a
   general data-integrity rule. This makes the assignment's business rule ("cannot mark
   Done with an empty title") always satisfied by the time the status is checked —
   the explicit guard is kept in the service to make the requirement's intent clear,
   but it is the title validation that enforces it in practice.
