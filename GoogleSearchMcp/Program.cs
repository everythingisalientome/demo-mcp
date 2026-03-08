using GoogleSearchMcp.Tools;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<GoogleSearchTool>()
    .WithTools<NotepadTool>();

var app = builder.Build();

app.MapMcp();

app.Run();