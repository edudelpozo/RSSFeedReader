# Quickstart: MVP RSS Reader — Local Development

**Branch**: `001-mvp-rss-reader`  
**Date**: 2026-04-14

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed
- A modern web browser (Chrome, Edge, Firefox)
- Two terminal windows (or tabs)

---

## 1. Clone and navigate

```powershell
git clone https://github.com/edudelpozo/RSSFeedReader.git
cd RSSFeedReader
git switch 001-mvp-rss-reader
```

---

## 2. Start the backend

```powershell
dotnet run --project backend/RSSFeedReader.Api
```

Expected output:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5151
```

The API is now available at `http://localhost:5151/api/subscriptions`.

---

## 3. Start the frontend (separate terminal)

```powershell
dotnet run --project frontend/RSSFeedReader.UI
```

Expected output:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5213
```

Open [http://localhost:5213](http://localhost:5213) in your browser.

---

## 4. Verify the app works

1. You should see the **Subscriptions** page with an empty list and a URL input field.
2. Paste any URL (e.g., `https://devblogs.microsoft.com/dotnet/feed/`) into the input and click **Add**.
3. The URL should appear in the list immediately — no page refresh required.
4. Add a second URL and verify both appear in order.

---

## 5. Pre-test checklist (from ProjectGoals.md)

Before running automated tests or submitting a PR, verify:

- [ ] Backend runs without errors and listens on **port 5151**
- [ ] Frontend runs without errors and loads in the browser on **port 5213**
- [ ] `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` contains `"ApiBaseUrl": "http://localhost:5151/api/"`
- [ ] Backend CORS allows `http://localhost:5213`
- [ ] Browser DevTools console (F12) shows no connection errors when loading the page

---

## 6. Run tests

```powershell
dotnet test backend/RSSFeedReader.Api.Tests
```

Expected: all tests pass.

---

## 7. Verify Blazor template cleanup (before implementing new features)

Confirm that the Blazor template demo pages have been removed:

```powershell
Get-ChildItem frontend/RSSFeedReader.UI/Pages/ -Filter *.razor | Select-Object Name
```

Expected output — only MVP pages:

```
Subscriptions.razor
```

If `Counter.razor`, `Weather.razor`, or `Home.razor` appear, delete them before proceeding (see `TechStack.md`).

---

## Port reference

| Component          | Port | Configuration file                                         |
| ------------------ | ---- | ---------------------------------------------------------- |
| Backend API        | 5151 | `backend/RSSFeedReader.Api/Properties/launchSettings.json` |
| Frontend UI        | 5213 | `frontend/RSSFeedReader.UI/Properties/launchSettings.json` |
| Frontend → API URL | —    | `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`       |
| CORS allowlist     | —    | `backend/RSSFeedReader.Api/Program.cs`                     |
