using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Server;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools();

builder.Logging.SetMinimumLevel(LogLevel.Error);
await builder.Build().RunAsync();

[McpToolType]
public static class TimeTool
{
    [McpTool, Description("Get the current time for a city")]
    public static string GetCurrentTimeInCity(string city) => 
        $"It is {DateTime.Now.Hour}:{DateTime.Now.Minute} in {city}.";
}

[McpToolType]
public static class FileSystemTool
{
    [McpTool, Description("Get a list of files in the specified directory")]
    public static string[] ListFilesAndSubdirectories(string directoryPath)
    {
        try
        {
            return !Directory.Exists(directoryPath) 
                ? [$"Error: Directory '{directoryPath}' does not exist."]
                : Directory.GetFileSystemEntries(directoryPath);
        }
        catch (Exception ex)
        {
            return [$"Error: {ex.Message}"];
        }
    }
}