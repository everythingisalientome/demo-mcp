# demo-mcp
Demo MCP Project with desktop automation

Its a dummy tools project that uses FlaUI to automate notepad and respond with screenshot


# GoogleSearchMcp

A .NET 8 MCP (Model Context Protocol) server that exposes desktop automation tools to AI agents via HTTP. Built with ASP.NET Core and FlaUI for Windows UI automation.

---

## Project Structure

```
GoogleSearchMcp/
├── Program.cs                  # ASP.NET Core host + MCP server registration
└── Tools/
    ├── GoogleSearchTool.cs     # Mock Google Search tool
    └── NotepadTool.cs          # Notepad automation tool with screenshot
```

---

## Tools

### `google_search`
Accepts a search query string and returns mock search results as JSON.
> Replace mock response with real SerpAPI call when ready.

### `open_notepad_and_type`
- Opens Windows Notepad
- Types the given text using SendKeys
- Takes a screenshot of the Notepad window
- Returns the screenshot as a base64 PNG string

Uses FlaUI (UIA3) for window attachment and mouse click, and Win32 P/Invoke (`ShowWindow`, `SetForegroundWindow`) for window management.

Logs to: `C:\agents\notepad_tool.log`

---

## Dependencies

| Package | Version |
|---|---|
| ModelContextProtocol | 1.0.0 |
| ModelContextProtocol.AspNetCore | 1.0.0 |
| FlaUI.UIA3 | 4.0.0 |
| FlaUI.Core | 4.0.0 |
| System.Drawing.Common | 8.0.0 |

Target framework: `net8.0-windows`  
`UseWindowsForms: true` (required for SendKeys)

---

## Build

```bash
cd D:\VisualStudioWrkSpce\2022\demo-mcp\GoogleSearchMcp

# Clean previous build
dotnet clean -c Release

# Publish self-contained Windows x64 executable
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
```

Output: `./publish/GoogleSearchMcp.exe`

---

## Run

```bash
cd D:\VisualStudioWrkSpce\2022\demo-mcp\GoogleSearchMcp\publish

# Run on default port 5001
.\GoogleSearchMcp.exe --urls http://localhost:5001

# Run on a specific port (used by registry for multi-session)
.\GoogleSearchMcp.exe --urls http://localhost:5002
```

> **Note:** The MCP endpoint is mapped to `/` (not `/mcp`).  
> Clients should connect to `http://localhost:{port}` directly.

---

## Test with MCP Inspector

```bash
npx @modelcontextprotocol/inspector
```

Open browser at `http://localhost:6274`:
- Transport: `Streamable HTTP`
- URL: `http://localhost:5001`

---

## Deploy to Server

Copy entire `publish\` folder to:
```
C:\agents\GoogleSearchMcp\
```

Kill existing processes before copying:
```powershell
Get-Process GoogleSearchMcp | Stop-Process -Force
```

The registry (`registry.exe`) will launch this automatically on startup — no need to run manually in production.