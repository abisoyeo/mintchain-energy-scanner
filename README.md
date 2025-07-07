# Mintchain Energy Scanner

A modular .NET application that scans tree IDs on the Mintchain blockchain for available stealable energy, using a Blazor frontend, a RESTful API service, and a core library that handles scraping and validation logic.

## Features

- Blazor WebAssembly frontend for live input and result display
- ASP.NET Core API backend with token-based interaction
- Core scraping logic in a shared library
- Progress tracking during scanning
- Graceful handling of `204 No Content`, `400 Bad Request`, and `401 Unauthorized` responses
- Clipboard integration in UI for easy copying of tree IDs
- Modular architecture for maintainability

---

## Project Structure

```

mintchain-energy-scanner/
├── MintChainDropsBlazorClient/       \# Blazor WASM frontend
├── MintChainDropsApiService/         \# ASP.NET Core API
├── EnergyStealLibrary/               \# Core service logic
├── MintChainDropsTracker/            \# Optional console runner
├── MintChainDropsApp.sln             \# Solution file
└── README.md

````

---

## Technologies Used

- .NET 7 / 8
- ASP.NET Core Web API
- Blazor WebAssembly
- C# (HttpClient, JSON, async/await)
- Newtonsoft.Json
- Bootstrap 5 (via Blazor)

---

## How It Works

1.  The user provides:
    -   Bearer Auth Key
    -   Start/Stop Tree ID range
    -   Minimum energy drop threshold
2.  The API:
    -   Iterates over the tree ID range
    -   Fetches associated user IDs
    -   Queries energy data for each user
    -   Returns only results with energy amount > threshold and marked as stealable
3.  The Blazor client:
    -   Displays progress and results
    -   Handles errors and empty results
    -   Supports clipboard copy for each result

---

## Setup

### Prerequisites

-   [.NET SDK 7+](https://dotnet.microsoft.com/)
-   Visual Studio 2022+ or VS Code
-   (Optional) Mintchain account & Bearer token

---

## Getting Started

### Clone and Run

```bash
git clone https://github.com/abisoyeo/mintchain-energy-scanner.git
cd mintchain-energy-scanner
````

Open `MintChainDropsApp.sln` in Visual Studio
Set `MintChainDropsBlazorClient` as the startup project
Run the solution (`F5` or `dotnet run`)

To run the console app manually:

```bash
cd MintChainDropsTracker
dotnet run
```

### API Endpoint

`POST /api/EnergySteal`

### Request Body

```json
{
  "authKey": "your-mintchain-token",
  "startTreeId": 1000,
  "stopTreeId": 1100,
  "minDrop": 20
}
```

### Responses

  * `200 OK` – Returns list of stealable tree energy
  * `204 No Content` – No matching energy found
  * `206 Partial Content` – Some results + error details
  * `400 Bad Request` – Invalid request
  * `401 Unauthorized` – Invalid or expired token
  * `500 Internal Server Error` – Unexpected error

### Blazor UI Highlights

  * Form validation for required fields
  * Progress bar while scanning
  * Formatted result cards with copy-to-clipboard
  * Error messaging for various failure states

-----

## License

MIT License – see `LICENSE` file for details.

