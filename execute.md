dotnet run --urls http://localhost:5001

To properly test it you have two options:
Option 1: MCP Inspector (quickest)
Microsoft provides a browser-based MCP test tool:
bashnpx @modelcontextprotocol/inspector
or
npx @modelcontextprotocol/inspector
Point it at http://localhost:5001 and you can invoke your tools interactively.



On bundling and running on the server:
dotnet publish creates a self-contained bundle:
bash

cd D:\VisualStudioWrkSpce\2022\demo-mcp\GoogleSearchMcp
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish


This creates a folder with:

GoogleSearchMcp.exe — standalone, no .NET runtime needed on the target machine
All dependencies bundled in

Copy the publish folder to your Windows Server and run:
bashGoogleSearchMcp.exe --urls http://localhost:5001
```

The registry will call this exact command when spawning a session.

---

**On concurrent sessions and HTTP isolation — great question:**

Each session has its **own process**:
```
Session 1: GoogleSearchMcp.exe --urls http://localhost:5001  (its own process, its own memory)
Session 2: GoogleSearchMcp.exe --urls http://localhost:5002  (completely separate process)
So when automation request 2 comes in:

Registry spawns a new session on port 5002
A brand new MCP process starts
Agent 2 talks to MCP on 5002, Agent 1 talks to MCP on 5001
They never interact

No shared state, no interference — full isolation by design.