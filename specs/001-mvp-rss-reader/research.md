# Research: MVP RSS Reader — Subscription Management

**Phase**: 0 — Outline & Research  
**Branch**: `001-mvp-rss-reader`  
**Date**: 2026-04-14

All technical choices for this feature are already established by `TechStack.md` and the project constitution. No NEEDS CLARIFICATION items remain. This document records the decision rationale and alternatives considered to satisfy the Phase 0 requirement.

---

## Decision 1: Backend framework

**Decision**: ASP.NET Core Minimal API _or_ Controller-based API on .NET 10 LTS  
**Chosen approach**: Controller-based (`ApiController`) to match the separation-of-concerns principle (explicit DTO classes, injectable services, testable controller actions).

**Rationale**: Minimal API would suffice for the two MVP endpoints, but controller-based API provides clearer structural boundaries for the service injection pattern required by the constitution (principle III). The test surface for integration tests using `WebApplicationFactory<T>` is identical either way.

**Alternatives considered**:

- Minimal API — rejected because it is harder to enforce DTO separation and service injection discipline for new contributors; a minor convenience gain is not worth the structural ambiguity.
- gRPC — rejected; browser-native gRPC-Web requires additional middleware; no benefit at MVP scale.

---

## Decision 2: In-memory storage strategy

**Decision**: `SubscriptionService` registered as a singleton holds a `List<Subscription>` protected by a `lock` for thread-safety.

**Rationale**: FR-006 requires in-memory storage only. A singleton service is the simplest approach that survives the request lifetime while remaining testable. Thread-safety via `lock` is cheap and prevents list corruption if the browser sends concurrent requests.

**Alternatives considered**:

- `ConcurrentBag<T>` — rejected; does not preserve insertion order (required by acceptance scenario 2 of Story 2); `lock` on a `List<T>` is simpler to reason about.
- `IMemoryCache` — rejected; adds an unnecessary abstraction and dependency for a plain list; constitution principle II prohibits adding abstraction layers beyond what is needed.
- No locking at all — rejected; Blazor WASM may issue rapid successive POSTs and Kestrel handles each on a thread-pool thread; unsafe without synchronisation.

---

## Decision 3: Blazor project structure

**Decision**: Single Blazor WebAssembly standalone project (`RSSFeedReader.UI`) at `frontend/RSSFeedReader.UI/`. No Blazor Server, no hosted model.

**Rationale**: Standalone Blazor WASM keeps the frontend fully decoupled from the backend (communicates only via HTTP), which is the separation-of-concerns model mandated by the constitution. It also matches the TechStack.md description of "Blazor WebAssembly frontend."

**Alternatives considered**:

- Blazor Server — rejected; requires a persistent SignalR connection and a server-side host; significantly more complex; WASM is explicitly chosen in TechStack.md.
- Blazor Hosted (WASM served from the API project) — rejected; merges backend and frontend into one project; violates the clean separation required by principle III.

---

## Decision 4: DTO representation

**Decision**: C# `record` types for request and response DTOs (`AddSubscriptionRequest`, `SubscriptionDto`).

**Rationale**: Records provide value-equality, immutability by default, and concise syntax — ideal for data-transfer objects. They satisfy principle III (explicit DTO classes, no anonymous objects crossing the API boundary) and principle IV (clear type definitions, no compiler warnings).

**Alternatives considered**:

- `class` with properties — functionally equivalent; slightly more verbose; no reason to prefer it.
- Anonymous types / `dynamic` — explicitly forbidden by constitution principle III.

---

## Decision 5: CORS configuration

**Decision**: Explicit CORS policy in `Program.cs` allowing only the frontend origin (`http://localhost:5213`, `https://localhost:7025`). No wildcard.

**Rationale**: Constitution principle I mandates explicit CORS — no wildcard (`*`) policies are permitted in any environment. The TechStack.md documents the exact ports (5151 backend, 5213 frontend) to use.

**Configuration**:

```csharp
builder.Services.AddCors(options =>
    options.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins("http://localhost:5213", "https://localhost:7025")
              .AllowAnyHeader()
              .AllowAnyMethod()));
```

**Alternatives considered**:

- `AllowAnyOrigin()` — forbidden by constitution principle I.
- Environment-variable–driven origin list — useful for production but out of scope for a local POC; premature abstraction violating principle II.

---

## Decision 6: Input validation approach

**Decision**: Validate non-empty URL at the API boundary in the controller action using a model-validation attribute (`[Required]`) on the DTO, plus an explicit guard in `SubscriptionService.Add()`.

**Rationale**: Constitution principle I requires input to be validated and sanitised at the system boundary. FR-005 requires empty strings to be rejected. Using `[Required]` on the DTO provides automatic 400 validation responses; the service guard provides defence-in-depth without duplicating HTTP plumbing concerns.

**Note**: No URL _format_ validation is required for MVP (assumption in spec.md). Storing the URL as an opaque string and rendering it as `<a href="@url">@url</a>` is safe only if Blazor encodes the attribute value — which it does for `@` bindings, satisfying the anti-XSS requirement in principle I.

**Alternatives considered**:

- `[Url]` attribute — rejected; spec explicitly states no URL format validation for MVP.
- Manual regex validation — rejected; same reason; also premature complexity.

---

## Decision 7: Test framework

**Decision**: xUnit with `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<T>`) for integration tests.

**Rationale**: xUnit is the .NET ecosystem standard and the only testing framework referenced in TechStack.md and the constitution. `WebApplicationFactory<T>` is the Microsoft-recommended approach for in-process API integration testing; it requires no external infrastructure.

**Alternatives considered**:

- NUnit / MSTest — no justification to deviate from xUnit which is already the project standard.
- Postman/Newman contract tests — useful but a heavier-weight tool; xUnit integration tests cover the same scenarios without additional tooling.

---

## Summary: No NEEDS CLARIFICATION items remain

All technical unknowns are resolved:

| Item               | Resolution                                                            |
| ------------------ | --------------------------------------------------------------------- |
| .NET version       | .NET 10 LTS (current LTS as of 2026-04-14)                            |
| Backend framework  | ASP.NET Core Web API, controller-based                                |
| Frontend framework | Blazor WebAssembly standalone                                         |
| Storage            | Singleton `SubscriptionService` with `List<Subscription>` + `lock`    |
| DTOs               | C# `record` types                                                     |
| CORS               | Explicit allowlist: `http://localhost:5213`, `https://localhost:7025` |
| Ports              | Backend: 5151, Frontend: 5213                                         |
| Validation         | `[Required]` on DTO + service guard; no URL format validation         |
| Testing            | xUnit + `WebApplicationFactory<T>`                                    |
