global using Microsoft.Extensions.Logging;
global using ModelContextProtocol.Client;
global using ModelContextProtocol.Configuration;
global using ModelContextProtocol.Protocol.Transport;

Console.WriteLine("Hello, official MCP csharp-sdk and MCP Server!");

// 👇🏼 Configure the MCP client options
McpClientOptions options = new()
{
    ClientInfo = new() { Name = "File System Client", Version = "1.0.0" }
};

// 👇🏼 Configure the Model Context Protocol server to use
var config = new McpServerConfig
{
    Id = "fileSystem",
    Name = "File System MCP Server",
    TransportType = TransportTypes.StdIo,
    TransportOptions = new Dictionary<string, string>
    {
        // 👇🏼 The command executed to start the MCP server
        ["command"] = @"../../../../McpServer/bin/Debug/net8.0/McpServer"
    }
};

using var factory =
    LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Error));

// 👇🏼 Get an MCP session scope used to get the MCP tools
await using var mcpClient = 
    await McpClientFactory.CreateAsync(config, options, loggerFactory: factory);

// 👇🏼 Get the MCP tools
var mcpTools = await mcpClient.GetAIFunctionsAsync();

// Display available tools
Console.WriteLine("\nAvailable tools:");
foreach (var tool in mcpTools)
{
    Console.WriteLine($"- {tool.Name}: {tool.Description}");
}
