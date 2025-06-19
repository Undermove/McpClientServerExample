using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using System.Text.Json;

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

Console.WriteLine("\nДоступные инструменты:");

// Создаем структуру в формате MCP list_tools
var toolsResponse = new
{
    tools = mcpTools.Select(tool => new
    {
        name = tool.Name,
        description = tool.Description,
        inputSchema = tool.JsonSchema
    }).ToArray()
};

// Выводим в формате JSON с красивым форматированием
var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

var jsonOutput = JsonSerializer.Serialize(toolsResponse, jsonOptions);
Console.WriteLine(jsonOutput);

// 4. Отправляем список тулзов в LLM
Console.WriteLine("\nОтправляем список tools в LLM");

Console.WriteLine("\nВведите запрос к LLM (например, 'Какое сейчас время в Батуми?'):");

// 5. Как бы запрос к LLM с использованием MCP тулзов
var userQuery = Console.ReadLine();

// 6. Эмулируем ответ от LLM - она анализирует запрос и решает какой tool вызвать
Console.WriteLine("\n=== Ответ от LLM ===");
Console.WriteLine("Для ответа на ваш запрос мне нужно вызвать инструмент для получения времени. Вот JSON для вызова:");
var toolCall = new
{
    method = "tools/call",
    @params = new
    {
        name = "GetCurrentTimeInCity",
        arguments = new
        {
            city = "Батуми"
        }
    }
};

var toolCallJson = JsonSerializer.Serialize(toolCall, jsonOptions);
Console.WriteLine(toolCallJson);
                  
// 7. Дергаем инструмент, который попросила дернуть LLM
Console.WriteLine("\n=== Вызываем инструмент GetCurrentTimeInCity ===");


// Вызываем функцию через MCP клиент
string cityToQuery = "Батуми";
var result = await mcpClient.CallToolAsync("GetCurrentTimeInCity", new Dictionary<string, object>
{
    ["city"] = cityToQuery
});

Console.WriteLine("\n=== Результат от сервера ===");
Console.WriteLine($"Результат: {result.Content}");
    
// Выводим результат в JSON формате для красоты
var resultJson = new
{
    toolName = "GetCurrentTimeInCity",
    input = new { city = cityToQuery },
    output = result.Content,
    isError = result.IsError
};
    
var resultJsonString = JsonSerializer.Serialize(resultJson, jsonOptions);
Console.WriteLine("\nРезультат в JSON:");
Console.WriteLine(resultJsonString);

// 8. Передаем ответ от сервера обратно в LLM
Console.WriteLine("\nПередаем результат обратно в LLM для финального ответа...");
Console.WriteLine($"\nОтвет от LLM: {result.Content[0].Text}");

