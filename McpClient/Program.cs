using McpDotNet.Client;
using McpDotNet.Configuration;
using McpDotNet.Protocol.Transport;
using Microsoft.Extensions.Logging;

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
    LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));

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

// Ask user for directory path
Console.WriteLine("\nEnter directory path to list files (or press Enter for current directory):");
var directoryPath = Console.ReadLine();

// Use current directory if none provided
if (string.IsNullOrWhiteSpace(directoryPath))
{
    directoryPath = Directory.GetCurrentDirectory();
}

Console.WriteLine($"\nListing files in: {directoryPath}");

try
{
    // Find the ListFiles function
    var listFilesFunction = mcpTools.FirstOrDefault(f => f.Name == "ListFiles");
    
    if (listFilesFunction == null)
    {
        Console.WriteLine("Error: ListFiles function not found.");
        return;
    }
    
    // Call the ListFiles function
    var parameters = new Dictionary<string, object> { { "directoryPath", directoryPath } };
    
    // Print available methods on mcpClient for debugging
    Console.WriteLine("\nAvailable methods on mcpClient:");
    foreach (var method in typeof(IMcpClient).GetMethods())
    {
        Console.WriteLine($"- {method.Name}");
    }
    
    // Try to call the function using reflection
    var executeMethod = typeof(IMcpClient).GetMethod("ExecuteFunction");
    if (executeMethod != null)
    {
        Console.WriteLine($"\nFound method: {executeMethod.Name}");
        var task = (Task)executeMethod.Invoke(mcpClient, new object[] { listFilesFunction.Name, parameters });
        await task;
        var result = task.GetType().GetProperty("Result").GetValue(task);
        
        // Display the result
        Console.WriteLine($"\nResult: {result}");
    }
    else
    {
        Console.WriteLine("\nError: ExecuteFunction method not found.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}