<!--
SYNC IMPACT REPORT
==================
Version change: (none — initial fill) → 1.0.0
Modified principles: N/A (first concrete version)
Added sections:
  - Core Principles (I–V)
  - Technology Constraints
  - Development Workflow & Quality Gates
  - Governance
Removed sections: N/A (template placeholders replaced)
Templates reviewed:
  - .specify/templates/plan-template.md ✅ no changes required; Constitution Check gate is dynamically filled per feature
  - .specify/templates/spec-template.md ✅ no changes required; structure aligns with principles
  - .specify/templates/tasks-template.md ✅ no changes required; phase structure aligns with MVP-First principle
Follow-up TODOs: none — all placeholders resolved
-->

# RSS Feed Reader Constitution

## Core Principles

### I. Security-First Development (NON-NEGOTIABLE)

Security constraints MUST be enforced at every layer of the application, even in MVP stages:

- All API endpoints MUST validate and sanitize input at the system boundary before processing.
  Even when URL validation is intentionally omitted for MVP functional scope, inputs MUST never
  be reflected back to clients or executed without sanitization.
- CORS MUST be explicitly configured on the ASP.NET Core backend to allow only the specific
  frontend origin. Wildcard (`*`) CORS policies are FORBIDDEN in any environment.
- No secrets, credentials, API keys, or connection strings may appear in source code or be
  committed to the repository. Use .NET User Secrets (`dotnet user-secrets`) for local
  development and environment variables for production.
- HTTP client calls introduced in Extended-MVP or later MUST enforce explicit timeout limits
  and MUST handle network failures with graceful error responses—no unhandled exceptions
  propagated to the client.
- External URLs stored as subscriptions MUST be stored as opaque strings and rendered safely
  (e.g., as plain text or validated anchor hrefs), never injected into DOM or executable
  contexts without sanitization.

**Rationale**: This is a client-server web application. CORS misconfigurations, XSS via
unsanitized URL reflection, and exposed secrets are the highest-risk attack vectors for
this architecture and MUST be mitigated from day one.

### II. MVP-First Simplicity (YAGNI)

Each implementation phase MUST implement only what is scoped to that phase:

- Code MUST NOT implement features beyond the current phase (MVP / Extended-MVP / Post-MVP)
  even if the future shape seems obvious. Future features are out of scope until explicitly
  scheduled.
- In-memory storage (a `List<T>` or equivalent) is the REQUIRED storage strategy for MVP.
  No database, file-based, or external persistence layer may be introduced until the
  Post-MVP persistence phase.
- No abstraction layers (repositories, unit-of-work, service buses, etc.) may be introduced
  unless the current phase actually requires them. Justify every added layer.
- The Blazor template cleanup (removal of `Counter.razor`, `Weather.razor`, `Home.razor`
  demo pages and their nav links) MUST be completed and verified before any MVP UI
  feature work begins.

**Rationale**: Premature complexity is the primary risk for a proof-of-concept project.
Keeping each phase minimal and verifiable ensures rapid delivery and prevents scope creep
from invalidating the demonstration goals.

### III. Clean Separation of Concerns

The backend and frontend MUST maintain strict responsibility boundaries:

- The ASP.NET Core Web API backend is responsible for: all business logic, data management,
  feed operations, and exposing typed HTTP endpoints.
- The Blazor WebAssembly frontend is responsible for: UI presentation and user interaction
  only. It MUST communicate with the backend exclusively via the defined API.
- Business logic and data transformation MUST NOT appear directly inside Razor components
  (`.razor` files). Extract logic into injectable C# service classes.
- All API request and response shapes MUST be represented as explicitly defined DTO classes
  or records. Ad-hoc anonymous objects crossing the API boundary are FORBIDDEN.

**Rationale**: The ASP.NET Core + Blazor WebAssembly architecture is chosen specifically
for its clean separation. Violating this boundary makes the codebase unmaintainable and
blocks future extensibility.

### IV. Code Quality & Maintainability

All code MUST meet these quality standards before merging:

- Code MUST follow standard C# naming conventions:
  PascalCase for types, methods, and public members;
  camelCase for local variables and parameters;
  `_camelCase` for private fields.
- Methods MUST remain focused and small. As a practical guideline, a method exceeding
  ~30 lines is a signal to extract and decompose—not a hard limit, but deviations MUST
  be justified.
- Commented-out code MUST NOT be committed. Remove dead code; version control preserves
  history.
- New NuGet dependencies MUST be explicitly justified in the PR description. The dependency
  list MUST remain minimal. For Extended-MVP, `System.ServiceModel.Syndication` (inbox .NET
  library) is the REQUIRED and ONLY approved feed-parsing approach—no third-party RSS
  parsing libraries.
- The project MUST build without compiler warnings. Warnings treated as errors (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`) is the target configuration.

**Rationale**: Consistent naming, focused methods, and a lean dependency set are the
primary levers for long-term maintainability of a codebase that is intended to grow
incrementally from MVP to full-featured application.

### V. Code Quality Gates (Testing & Review)

No code may be merged to `main` without passing explicit quality gates:

- All new service classes MUST have unit tests covering at minimum: the primary happy-path
  scenario and at least one failure/error path.
- API endpoint integration tests MUST cover: the expected success response shape and at
  least one validation-failure or error response.
- All tests MUST pass in CI before a pull request is merged. No exceptions.
- PRs MUST be reviewed with the Constitution Check from the active feature `plan.md` as
  an explicit gate—the reviewer MUST confirm each check is satisfied.

**Rationale**: Untested code in an incrementally built application becomes a tax on every
future phase. Lightweight but mandatory test coverage ensures each phase ships with
confidence and does not break previous phases.

## Technology Constraints

The following technology choices are established for this project and MUST NOT be
substituted without a constitution amendment:

- **Backend**: ASP.NET Core Web API on the current .NET LTS release.
- **Frontend**: Blazor WebAssembly.
- **Language**: C# throughout (backend and frontend). No JavaScript files except
  interop shims if absolutely required.
- **Feed parsing** (Extended-MVP+): `System.ServiceModel.Syndication` (inbox). No
  third-party feed-parsing NuGet packages.
- **Persistence** (Post-MVP only): Entity Framework Core with SQLite as the default
  target for local/POC scenarios.
- **HTTP client** (Extended-MVP+): `System.Net.Http.HttpClient` via the built-in
  `IHttpClientFactory`. No third-party HTTP client libraries.
- **Target platforms**: Windows, macOS, and Linux. Code MUST NOT use platform-specific
  APIs without an explicit cross-platform abstraction.

## Development Workflow & Quality Gates

Every feature delivery MUST follow these process requirements:

1. **Phase gate**: Confirm the current work is scoped to the active phase (MVP,
   Extended-MVP, or Post-MVP) before writing any code.
2. **Template cleanup gate** (Blazor projects): Verify `Counter.razor`, `Weather.razor`,
   and `Home.razor` are removed and routing conflicts are absent before beginning
   any UI feature implementation. This MUST happen in Phase 2 (Foundational).
3. **Local checklist before PR**: Verify the local dev checklist in `ProjectGoals.md`
   is satisfied: backend runs, frontend runs, `wwwroot/appsettings.json` points to
   the backend, CORS allows the frontend origin, and the browser DevTools console
   shows no connection errors.
4. **Constitution Check in plan.md**: Each feature's `plan.md` MUST include an explicit
   Constitution Check section. All gates MUST be verified before Phase 0 research and
   re-checked after Phase 1 design.
5. **No force-pushes to `main`**. Feature branches MUST be merged via pull request.

## Governance

This constitution supersedes all ad-hoc decisions on architecture, coding style, feature
scope, and technology selection for the RSS Feed Reader project.

**Amendment procedure**:

1. Open a pull request with changes to `.specify/memory/constitution.md`.
2. Increment the version according to semantic rules:
   MAJOR for backward-incompatible governance/principle removals or redefinitions;
   MINOR for new principles or materially expanded guidance;
   PATCH for clarifications, wording, or non-semantic refinements.
3. Update the Sync Impact Report comment at the top of this file.
4. Propagate impacted changes to templates and dependent artifacts as identified in
   the report.
5. PR must be approved before merge.

**Compliance**: All PRs and code reviews MUST reference the Constitution Check section
of the active feature plan. Violations require justification in the Complexity Tracking
table of `plan.md`.

**Version**: 1.0.0 | **Ratified**: 2026-04-14 | **Last Amended**: 2026-04-14
