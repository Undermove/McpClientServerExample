global using System.ComponentModel;
global using Microsoft.Extensions.Hosting;
global using ModelContextProtocol;
global using ModelContextProtocol.Server;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
    // 👇🏼 We build an MCP Server   
    .AddMcpServer()
    // 👇🏼 uses Stdio as transport protocol    
    .WithStdioServerTransport()
    // 👇🏼 Register all tools with McpToolType attribute    
    .WithTools();
await builder.Build().RunAsync();

// 👇🏼 Mark our type as a container for MCP tools
[McpToolType]
public static class TimeTool
{
    // 👇🏼 Mark a method as an MCP tools
    [McpTool, Description("Get the current time for a city")]
    public static string GetCurrentTime(string city) => 
        $"It is {DateTime.Now.Hour}:{DateTime.Now.Minute} in {city}.";
}

// 👇🏼 Mark our type as a container for file system tools
[McpToolType]
public static class FileSystemTool
{
    // 👇🏼 Mark a method as an MCP tool for listing files in a directory
    [McpTool, Description("Get a list of files in the specified directory")]
    public static string[] ListFiles(string directoryPath)
    {
        try
        {
            // Check if directory exists
            if (!Directory.Exists(directoryPath))
            {
                return new[] { $"Error: Directory '{directoryPath}' does not exist." };
            }

            // Get all files in the directory
            return Directory.GetFiles(directoryPath);
        }
        catch (Exception ex)
        {
            return new[] { $"Error: {ex.Message}" };
        }
    }
}