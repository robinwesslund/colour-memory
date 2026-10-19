# Memory game (.NET + Angular)

    memory/
    ├── MemoryApi/       ASP.NET Core Minimal API (C#, server-authoritative game logic)
    └── memory-client/   Angular 19 SPA (renders the board, calls the API)

## Prerequisites
- .NET 10 SDK (`dotnet --version`)
- Node.js 18.19+ / 20 / 22 and npm

## Run (two terminals)

Terminal 1, API on http://localhost:5000:

    cd MemoryApi
    dotnet run

Terminal 2, Angular on http://localhost:4200:

    cd memory-client
    npm install
    npm start

Open http://localhost:4200. `npm start` uses `proxy.conf.json`, so `/api/*` is forwarded
to the API and no CORS setup is needed.

## API
| Method | Route                              | Purpose                  |
|--------|------------------------------------|--------------------------|
| POST   | /api/games                         | Start a new shuffled game|
| GET    | /api/games/{id}                    | Current board            |
| POST   | /api/games/{id}/flip/{index}       | Flip card 0-15           |

Face-down cards come back with `symbol: null`, so the board can't be read from DevTools.

## Troubleshooting
- Port 5000 busy: change `applicationUrl` in `MemoryApi/Properties/launchSettings.json`
  and `target` in `memory-client/proxy.conf.json`.
- Using a newer .NET: change `<TargetFramework>` in `MemoryApi/MemoryApi.csproj`.
