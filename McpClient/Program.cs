using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;

Console.WriteLine("Start MCP client!");

McpClientOptions options = new()
{
    ClientInfo = new Implementation
    {
        Name = "File System Client",
        Version = "1.0.0"
    }
};

// 1. Говорим, как поднять и подключиться к серверу
var config = new McpServerConfig
{
    Id = "fileSystem",
    Name = "File System MCP Server",
    TransportType = TransportTypes.StdIo,
    TransportOptions = new Dictionary<string, string>
    {
        ["command"] = "../../../../McpServer/bin/Debug/net8.0/McpServer"
    }
};

using var factory =
    LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Critical));

// 2. Создаем клиент, который будет подключаться к серверу
await using var mcpClient = 
    await McpClientFactory.CreateAsync(config, options, loggerFactory: factory);

// 3. Подключаемся к серверу и просим его прислать доступные тулзы
var mcpTools = await mcpClient.GetAIFunctionsAsync();

Console.WriteLine("\nAvailable tools:");
foreach (var tool in mcpTools)
{
    Console.WriteLine($"- {tool.Name}: {tool.Description}");
}