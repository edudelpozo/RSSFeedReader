# Tasks: MVP RSS Reader — Subscription Management

**Input**: Design documents from `/specs/001-mvp-rss-reader/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/api-subscriptions.md ✅, quickstart.md ✅

**Tests**: Included — constitution principle V mandates xUnit unit tests for service classes and integration tests for API endpoints.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (US1, US2)
- Exact file paths are included in all descriptions

## Path Conventions (per plan.md)

- Backend source: `backend/RSSFeedReader.Api/`
- Backend tests: `backend/RSSFeedReader.Api.Tests/`
- Frontend source: `frontend/RSSFeedReader.UI/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Scaffold all three .NET projects and wire them into a single solution.

- [x] T001 Create `RSSFeedReader.sln` at repository root using `dotnet new sln`
- [x] T002 [P] Scaffold ASP.NET Core Web API project at `backend/RSSFeedReader.Api/` using `dotnet new webapi`, set `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in `backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj`, and add to solution
- [x] T003 [P] Scaffold Blazor WebAssembly standalone project at `frontend/RSSFeedReader.UI/` using `dotnet new blazorwasm`, set `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in `frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj`, and add to solution
- [x] T004 [P] Scaffold xUnit test project at `backend/RSSFeedReader.Api.Tests/` using `dotnet new xunit`, add `Microsoft.AspNetCore.Mvc.Testing` NuGet reference, add project reference to `backend/RSSFeedReader.Api/`, set `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in `backend/RSSFeedReader.Api.Tests/RSSFeedReader.Api.Tests.csproj`, and add to solution

**Checkpoint**: `dotnet build RSSFeedReader.sln` succeeds with no warnings — all three projects compile.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Domain model, DTOs, service interface, CORS, port configuration, and Blazor template cleanup. All user story work is blocked until this phase is complete.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [x] T005 Create `Subscription` domain model (`public record Subscription(string Url)`) in `backend/RSSFeedReader.Api/Models/Subscription.cs`
- [x] T006 [P] Create `AddSubscriptionRequest` DTO (`public record AddSubscriptionRequest([Required] string Url)`) in `backend/RSSFeedReader.Api/DTOs/AddSubscriptionRequest.cs`
- [x] T007 [P] Create `SubscriptionDto` DTO (`public record SubscriptionDto(string Url)`) in `backend/RSSFeedReader.Api/DTOs/SubscriptionDto.cs`
- [x] T008 Create `ISubscriptionService` interface with `Task<SubscriptionDto> AddAsync(string url)` and `Task<IReadOnlyList<SubscriptionDto>> GetAllAsync()` signatures in `backend/RSSFeedReader.Api/Services/ISubscriptionService.cs`
- [x] T009 Configure explicit CORS policy `FrontendPolicy` (allowing only `http://localhost:5213` and `https://localhost:7025; no wildcard`) and register `ISubscriptionService` DI placeholder in `backend/RSSFeedReader.Api/Program.cs`
- [x] T010 [P] Configure backend launch settings to listen on `http://localhost:5151` in `backend/RSSFeedReader.Api/Properties/launchSettings.json`
- [x] T011 [P] Configure frontend launch settings to listen on `http://localhost:5213` in `frontend/RSSFeedReader.UI/Properties/launchSettings.json`
- [x] T012 [P] Set `"ApiBaseUrl": "http://localhost:5151/api/"` in `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`
- [x] T013 Remove Blazor template demo pages (`Counter.razor`, `Weather.razor`, `Home.razor`) from `frontend/RSSFeedReader.UI/Pages/` and remove their nav links from `frontend/RSSFeedReader.UI/Layout/NavMenu.razor`

**Checkpoint**: Foundation ready — `dotnet build RSSFeedReader.sln` still clean; CORS policy is in place; all DTOs and the service interface compile; ports are configured; no template pages remain.

---

## Phase 3: User Story 1 — Add a Feed Subscription (Priority: P1) 🎯 MVP

**Goal**: A user can enter a feed URL and submit it; the URL is immediately added to the visible subscription list without a page reload.

**Independent Test**: Launch backend and frontend, open `http://localhost:5213`, paste any URL into the input field, click **Add**, and confirm the URL appears in the list. Applies acceptance scenarios 1–3 from spec.md US1.

### Tests for User Story 1

> **Write these tests FIRST; verify they FAIL before implementing the service/controller.**

- [x] T014 [P] [US1] Write `SubscriptionService` unit tests for `AddAsync`: happy path (valid URL is returned and stored) and error path (empty string throws `ArgumentException`) in `backend/RSSFeedReader.Api.Tests/Unit/SubscriptionServiceTests.cs`
- [x] T015 [P] [US1] Write integration tests for `POST /api/subscriptions`: returns `201 Created` with `SubscriptionDto` body on valid URL; returns `400 Bad Request` on empty URL in `backend/RSSFeedReader.Api.Tests/Integration/SubscriptionsControllerTests.cs`

### Implementation for User Story 1

- [x] T016 [US1] Implement `SubscriptionService.AddAsync` with a `private readonly List<Subscription> _subscriptions` and `private readonly object _lock` (thread-safe append, `ArgumentException` guard on empty URL) in `backend/RSSFeedReader.Api/Services/SubscriptionService.cs`
- [x] T017 [US1] Register `SubscriptionService` as a singleton (`builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>()`) in `backend/RSSFeedReader.Api/Program.cs`
- [x] T018 [US1] Implement `POST /api/subscriptions` action (inject `ISubscriptionService`, call `AddAsync`, return `CreatedAtAction` with `SubscriptionDto`) in `backend/RSSFeedReader.Api/Controllers/SubscriptionsController.cs`
- [x] T019 [US1] Create `ISubscriptionApiClient` interface with `Task<SubscriptionDto?> AddSubscriptionAsync(string url)` and `Task<List<SubscriptionDto>> GetSubscriptionsAsync()` signatures in `frontend/RSSFeedReader.UI/Services/ISubscriptionApiClient.cs`
- [x] T020 [US1] Implement `SubscriptionApiClient.AddSubscriptionAsync` (POST JSON to `api/subscriptions`, deserialize response, return `SubscriptionDto?`) in `frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs`
- [x] T021 [US1] Register `SubscriptionApiClient`, configure `HttpClient` with base address read from `IConfiguration["ApiBaseUrl"]` in `frontend/RSSFeedReader.UI/Program.cs`
- [x] T022 [US1] Implement `Subscriptions.razor` (`@page "/"`) with URL text input bound to `newUrl`, **Add** button that calls `AddSubscriptionAsync`, updates `subscriptions` list on success, shows `errorMessage` on failure, and disables the button via `isLoading` flag in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`

**Checkpoint**: US1 fully functional and independently testable — adding a URL via the browser form appends it to the displayed list; `dotnet test` on SubscriptionServiceTests and SubscriptionsControllerTests (POST cases) passes.

---

## Phase 4: User Story 2 — View Current Subscriptions (Priority: P2)

**Goal**: On page load the app displays all subscriptions added during the current session in insertion order; an empty list is shown when no subscriptions have been added.

**Independent Test**: Launch the app; the page loads with an empty list (no phantom entries). Add three URLs in sequence; verify all three are visible in the order they were entered. Applies acceptance scenarios 1–2 from spec.md US2.

### Tests for User Story 2

> **Write these tests FIRST; verify they FAIL before implementing the service method / GET action.**

- [ ] T023 [P] [US2] Write `SubscriptionService` unit tests for `GetAllAsync`: returns empty list on fresh instance; returns all added URLs in insertion order after multiple `AddAsync` calls in `backend/RSSFeedReader.Api.Tests/Unit/SubscriptionServiceTests.cs`
- [ ] T024 [P] [US2] Write integration tests for `GET /api/subscriptions`: returns `200 OK` with empty array on fresh session; returns `200 OK` with correct entries in order after one or more POST calls in `backend/RSSFeedReader.Api.Tests/Integration/SubscriptionsControllerTests.cs`

### Implementation for User Story 2

- [ ] T025 [US2] Implement `SubscriptionService.GetAllAsync` returning `IReadOnlyList<SubscriptionDto>` as an immutable snapshot of `_subscriptions` (locked read) in `backend/RSSFeedReader.Api/Services/SubscriptionService.cs`
- [ ] T026 [US2] Implement `GET /api/subscriptions` action (call `GetAllAsync`, return `Ok(subscriptions)`) in `backend/RSSFeedReader.Api/Controllers/SubscriptionsController.cs`
- [ ] T027 [US2] Implement `SubscriptionApiClient.GetSubscriptionsAsync` (GET `api/subscriptions`, deserialize `List<SubscriptionDto>`, return empty list on error) in `frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs`
- [ ] T028 [US2] Add `OnInitializedAsync` to call `GetSubscriptionsAsync` on page load and add `@foreach` loop to render the subscription list with insertion-order display and empty-state message ("No subscriptions yet.") in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`

**Checkpoint**: US1 AND US2 both work — page loads with the current subscription list from the API; new entries appear immediately after submit; all four xUnit test classes pass.

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Code quality verification and end-to-end quickstart validation.

- [ ] T029 [P] Verify `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` is present in all three `.csproj` files and resolve any remaining compiler warnings
- [ ] T030 Run full test suite (`dotnet test backend/RSSFeedReader.Api.Tests`) and confirm all tests pass (unit + integration)
- [ ] T031 Validate app end-to-end against `specs/001-mvp-rss-reader/quickstart.md`: start backend on port 5151, start frontend on port 5213, open browser, add multiple subscriptions, verify list order, confirm DevTools console shows no errors

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 completion — **blocks all user stories**
- **US1 (Phase 3)**: Depends on Phase 2 completion
- **US2 (Phase 4)**: Depends on Phase 3 completion (shares `SubscriptionService` and `Subscriptions.razor`)
- **Polish (Phase 5)**: Depends on Phase 4 completion

### User Story Dependencies

- **US1 (P1)**: Can start after Foundational — no dependency on US2
- **US2 (P2)**: Adds to files created in US1 (`SubscriptionService`, `SubscriptionsController`, `SubscriptionApiClient`, `Subscriptions.razor`) — implemented after US1

### Within Each User Story

- Tests MUST be written and FAIL before service/controller implementation
- Models/interfaces before services
- Services before controllers
- Backend complete before frontend integration

---

## Parallel Opportunities

### Phase 1 (after T001)

```
T002 — backend project scaffold
T003 — frontend project scaffold   ← run simultaneously
T004 — test project scaffold
```

### Phase 2 (independent files)

```
T005 — Subscription model
T006 — AddSubscriptionRequest DTO  ← run simultaneously with T005, T007
T007 — SubscriptionDto

T010 — backend launchSettings.json
T011 — frontend launchSettings.json  ← run simultaneously
T012 — appsettings.json
```

### Phase 3 — US1 (tests are parallel)

```
T014 — SubscriptionService unit tests (AddAsync)  ← run simultaneously
T015 — Controller integration tests (POST)
```

### Phase 4 — US2 (tests are parallel)

```
T023 — SubscriptionService unit tests (GetAllAsync)  ← run simultaneously
T024 — Controller integration tests (GET)
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (**CRITICAL** — blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Run `dotnet test`; open browser, add a subscription, confirm it appears
5. Demo if ready — this is a complete, demonstrable proof-of-concept

### Incremental Delivery

1. Setup + Foundational → clean build, ports configured
2. - User Story 1 → add-subscription flow works end-to-end → **MVP demo** ✅
3. - User Story 2 → list persists across page loads, empty-state handled → full spec delivered
4. - Polish → test suite green, quickstart validated → PR-ready

---

## Notes

- `[P]` tasks touch different files with no cross-dependencies — safe to parallelize
- `[Story]` labels (US1, US2) map directly to user stories in `spec.md`
- Tests must **fail** before implementation — this proves the tests are actually exercising the code
- `SubscriptionService` is split across US1 (`AddAsync`) and US2 (`GetAllAsync`) because these map to separate user stories; the interface in Phase 2 declares both signatures to keep it complete
- No bUnit or Blazor component tests are included — constitution principle V only specifies xUnit service unit tests and `WebApplicationFactory<T>` integration tests
- Duplicate subscriptions are allowed (no de-duplication logic per spec.md assumptions)
- No URL format validation — `[Required]` only (per spec.md assumptions)
