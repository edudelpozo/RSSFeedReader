# Data Model: MVP RSS Reader — Subscription Management

**Phase**: 1 — Design & Contracts  
**Branch**: `001-mvp-rss-reader`  
**Date**: 2026-04-14

---

## Entities

### Subscription

The single entity in this MVP. A `Subscription` represents a feed URL that the user wishes to track. It exists only in memory for the duration of the application session.

| Field | Type     | Rules                                                                           |
| ----- | -------- | ------------------------------------------------------------------------------- |
| `Url` | `string` | Required. Non-empty. Treated as an opaque string; no format validation for MVP. |

**C# domain model** (`backend/RSSFeedReader.Api/Models/Subscription.cs`):

```csharp
namespace RSSFeedReader.Api.Models;

public record Subscription(string Url);
```

---

## Storage Model

In-memory singleton service (`SubscriptionService`) holds a `List<Subscription>`. No persistence between sessions (FR-006).

**State**: A single ordered list, append-only for MVP.

| Operation       | Behaviour                                                                                         |
| --------------- | ------------------------------------------------------------------------------------------------- |
| `AddAsync(url)` | Validates non-empty, appends `new Subscription(url)` to the list. Returns the added subscription. |
| `GetAllAsync()` | Returns an immutable snapshot of the list in insertion order.                                     |

**Thread-safety**: Operations lock on a private object to prevent list corruption under concurrent requests.

---

## DTOs (API boundary)

All request and response shapes crossing the HTTP boundary are typed `record` types (constitution principle III).

### `AddSubscriptionRequest` — request body for `POST /api/subscriptions`

```csharp
namespace RSSFeedReader.Api.DTOs;

public record AddSubscriptionRequest([Required] string Url);
```

| Field | Type     | Validation                                    | Notes                                                  |
| ----- | -------- | --------------------------------------------- | ------------------------------------------------------ |
| `Url` | `string` | `[Required]` — must be non-null and non-empty | Enforced by model binding; returns HTTP 400 if missing |

---

### `SubscriptionDto` — response shape for all subscription endpoints

```csharp
namespace RSSFeedReader.Api.DTOs;

public record SubscriptionDto(string Url);
```

| Field | Type     | Notes                                                        |
| ----- | -------- | ------------------------------------------------------------ |
| `Url` | `string` | Opaque URL string as stored; rendered safely in the frontend |

---

## State Transitions

The subscription list is append-only for MVP. There are no status transitions or deletion operations.

```
[Empty list]  →  POST /api/subscriptions  →  [List with N entries]
                                          (repeat for each subscription added)
```

---

## Validation Rules

| Rule                                  | Enforcement Point                                            | Error                                                 |
| ------------------------------------- | ------------------------------------------------------------ | ----------------------------------------------------- |
| URL must not be null or empty         | `[Required]` on `AddSubscriptionRequest.Url` (model binding) | HTTP 400 with validation problem details              |
| URL must not be null or empty (guard) | `SubscriptionService.AddAsync()`                             | `ArgumentException` (caught by controller → HTTP 400) |

No URL format validation is required for MVP (per spec.md assumptions).

---

## Frontend State (Blazor component)

The `Subscriptions.razor` component maintains its own UI state:

| Field           | Type                    | Purpose                                                |
| --------------- | ----------------------- | ------------------------------------------------------ |
| `newUrl`        | `string`                | Two-way bound to the URL input field                   |
| `subscriptions` | `List<SubscriptionDto>` | Display list, refreshed after each successful add      |
| `isLoading`     | `bool`                  | Disables the submit button while awaiting API response |
| `errorMessage`  | `string?`               | Displays validation or network error to the user       |

Business logic (calling the API, updating the list) is encapsulated in `SubscriptionApiClient`, not inside the `.razor` file (constitution principle III).
