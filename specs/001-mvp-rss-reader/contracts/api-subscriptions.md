# API Contract: Subscriptions

**Service**: `RSSFeedReader.Api`  
**Base URL**: `http://localhost:5151/api`  
**Version**: MVP (1.0)  
**Date**: 2026-04-14

---

## POST /api/subscriptions

Add a new feed subscription to the in-memory list.

### Request

| Field | Location    | Type     | Required | Notes                                                                      |
| ----- | ----------- | -------- | -------- | -------------------------------------------------------------------------- |
| `url` | Body (JSON) | `string` | Yes      | Feed URL to subscribe to. Must be non-empty. No format validation for MVP. |

**Example request body**:

```json
{
  "url": "https://devblogs.microsoft.com/dotnet/feed/"
}
```

### Responses

#### 201 Created

The subscription was added successfully. The response body contains the created subscription.

```json
{
  "url": "https://devblogs.microsoft.com/dotnet/feed/"
}
```

**Headers**:

- `Content-Type: application/json`

#### 400 Bad Request

The request body is missing, the `url` field is absent, or the URL is an empty string.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Url": ["The Url field is required."]
  }
}
```

---

## GET /api/subscriptions

Retrieve all subscriptions added during the current session, in insertion order.

### Request

No request body. No query parameters.

### Responses

#### 200 OK

Returns an array of subscription objects (may be empty).

```json
[
  { "url": "https://devblogs.microsoft.com/dotnet/feed/" },
  { "url": "https://www.hanselman.com/blog/feed" }
]
```

**Headers**:

- `Content-Type: application/json`

An empty session returns:

```json
[]
```

---

## CORS Policy

The API allows requests only from the configured frontend origin.

| Header                         | Value                                                            |
| ------------------------------ | ---------------------------------------------------------------- |
| `Access-Control-Allow-Origin`  | `http://localhost:5213` (and `https://localhost:7025` for HTTPS) |
| `Access-Control-Allow-Methods` | `GET, POST, OPTIONS`                                             |
| `Access-Control-Allow-Headers` | `Content-Type`                                                   |

Wildcard CORS (`*`) is explicitly forbidden (constitution principle I).

---

## Error Handling Notes

- Empty or null `url` → HTTP 400 (model validation).
- No network operations are performed by the API for MVP; no timeout or feed-fetch errors can occur.
- Unhandled exceptions → HTTP 500 (default ASP.NET Core behaviour); no custom error middleware needed for MVP.
