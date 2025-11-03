# Sistema_GestionFacturacion — Collaboration Guide

Purpose
- This repository is a Windows Forms billing system ( .NET Framework 4.7.2 ).  
- Main features: orders (pedidos), order details, invoice generation, product management (categories, discounts), user/role management.

Goal of this document
- Define how the team should collaborate and how database queries must be implemented.
- Avoid common mistakes (embedding SQL/DB logic inside button handlers, duplicating connection logic, changing established `Conexion` behavior).

Prerequisites (for contributors)
- Visual Studio 2022
- .NET Framework 4.7.2 workload
- Microsoft SQL Server (Express/LocalDB or full instance)
- A valid connection string in `App.config` under the name `ConexionDB`
  - Example: `Server=.;Database=SistemaGestionFacturacion;Trusted_Connection=True;`
- Ensure the database schema exists (roles, users, products, categories, discounts, pedidos, detalles, factura). Run the provided SQL scripts if present.

Do NOT change
- `Sistema_GestionFacturacion\Clases\Conexion.cs` — this class and its lifetime semantics are part of the established project infrastructure. All DB access should respect it or rely on `ConsultasSQL` wrappers.

Primary rule for DB access
- Use only the `ConsultasSQL` class for database operations from forms and business logic.
  - Available helpers: `Buscar`, `Guardar`, `update`, `Eliminar`, `EjecutarConsulta`, `EjecutarComando`.
- Do not embed raw SQL strings and `SqlConnection`/`SqlCommand` code directly inside button click handlers or UI event handlers.
- For parametrized commands (especially for binary columns like `Clave`/`Sal`) create a `SqlCommand`, add parameters and call `consulta.EjecutarComando(cmd)`.

Why
- Centralizing DB access simplifies connection handling, logging and error handling.
- Avoids duplicate code and concurrency errors (open readers / connection state).
- Makes code review and testing easier.

Recommended patterns

- UI layer (forms) — minimal logic:
  - Validate UI input.
  - Call an application-specific method that uses `ConsultasSQL`.
  - Do not place SQL logic in `btnClick` directly.

Example pattern (pseudocode)
- In the form:
  - Create a method that calls `ConsultasSQL`:
    - `void SaveUser() { /* validate */ consulta.Guardar(table, cols, vals); }`
  - `btnSave_Click` should only call `SaveUser()`.

Database binary columns (password hash)
- If `Usuarios.Clave`/`Usuarios.Sal` are `VARBINARY`, use `SqlDbType.VarBinary` and `EjecutarComando` (not `Guardar` with inline strings).
- If you must store Base64 string in `NVARCHAR`, use `Buscar`/`Guardar` accordingly — but storing bytes as VARBINARY + parametrized commands is recommended.

DataGridView notes
- Do not bind password or salt columns to `DataGridView`.
- Remove `Clave`, `Sal`, `Iteraciones` from the DataTable before `DataSource = dt`, or exclude those columns in the SELECT.
- Attach a `dgv.DataError` handler to avoid the default DataGridView error dialog.

Coding conventions (short)
- Always use `using` for `IDisposable` resources (the `ConsultasSQL` methods already do this).
- Escape string values only when building safe SQL for `Guardar`/`update`. Prefer parametrized commands for all user input.
- Keep methods small and single-responsibility — UI should be thin; database calls live in `ConsultasSQL` or small repository wrappers.

Development workflow (Git / GitHub)
- Branching:
  - Create a branch per task: `feature/<short-description>` or `bugfix/<id>-short`.
  - Keep branches small and focused.
- Commits:
  - Use meaningful messages: `feat(users): add create-user form` or `fix(products): validation on price`.
  - Group related changes into a single PR when practical.
- Pull Requests:
  - Open a PR against `main` (or the designated integration branch).
  - Include a short description, steps to test, and any DB script changes required.
  - At least one reviewer must approve before merging.
- Code review checklist:
  - No direct SQL in event handlers
  - Uses `ConsultasSQL` for DB access
  - Connection string not changed in `Conexion.cs`
  - Passwords handled securely (PBKDF2 + salt) and stored as VARBINARY when possible
  - DataGridViews do not bind binary columns
  - Unit / manual test steps included

How to run locally
1. Clone the repository.
2. Open solution in Visual Studio 2022.
3. Update `App.config` > `connectionStrings` > `ConexionDB` with your SQL Server instance and DB.
4. Run any database creation scripts (check `Scripts/` or ask the maintainer if missing).
5. Build and run the app.
6. Use a test admin user or create users through the provided forms.

How to send changes for review
- Create a feature branch.
- Make changes, run and test locally.
- Commit with clear messages and push the branch to the remote.
- Open a Pull Request on GitHub describing:
  - What changed and why
  - How to test (DB scripts, sample data)
  - Any migration steps
- Tag reviewers and wait for CI / manual review.

Files & Forms overview (current)
- Forms:
  - `Login` — authentication; opens `FormRoles` (admin) or `FormPedidos` (employee).
  - `FormPedidos` — create orders; requires employee `Nombre` & `Apellido` only.
  - `FormUsuariosLogin` — user creation and management.
  - `FormRoles` — role management.
  - `FormUsuarios` — (product/users management variations).
  - `Logica_CRUD` — helper orchestration form (main menu).
- Classes:
  - `Clases\ConsultasSQL.cs` — central DB helper (use this).
  - `Clases\Conexion.cs` — shared connection utility (do not modify).
  - `Clases\AlertasDelSistema.cs` — UI alerts.

Final reminders
- Centralize DB access through `ConsultasSQL`.
- Keep UI handlers small and call methods that use `ConsultasSQL`.
- Never modify `Conexion.cs`.
- Use `EjecutarComando` for parametrized `SqlCommand` (VARBINARY columns).
- Follow Git workflow: branch ? commit ? PR ? review ? merge.

If you want, I can:
- Add a sample SQL script `Scripts/CreateSchema.sql` (roles, users with VARBINARY columns).
- Add a lightweight `CONTRIBUTING.md` with a PR template.
