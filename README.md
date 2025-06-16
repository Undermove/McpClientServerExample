# McpClientServerExample

A simple MCP (Message Communication Protocol) client-server application for file searching.

## Project Structure

- **McpCommon**: Shared library containing the protocol implementation and message models
- **McpServer**: Server application that handles file search requests
- **McpClient**: Client application that sends search requests to the server

## Building the Solution

```bash
dotnet build
```

## Running the Server

```bash
cd McpServer
dotnet run
```

The server will start and listen for connections on port 8888.

## Running the Client

```bash
cd McpClient
dotnet run [server_host] [server_port]
```

If no arguments are provided, the client will connect to localhost:8888 by default.

## Using the Client

1. Start the server
2. Start the client and connect to the server
3. Choose option 1 to search for files
4. Enter the directory path, search pattern, and whether to include subdirectories
5. View the search results

## Protocol Details

The MCP protocol uses TCP for communication and JSON for message serialization. Each message is prefixed with a 4-byte length header.

### Message Types

- **SearchRequest**: Request to search for files in a directory
- **SearchResponse**: Response containing the list of files found
- **Error**: Error message from the server

## Example Usage

1. Start the server:
   ```
   Server started on port 8888
   Waiting for connections...
   ```

2. Start the client and connect to the server:
   ```
   Connected to server
   ```

3. Search for files:
   ```
   Enter directory to search: C:\Users\username\Documents
   Enter search pattern (default: *): *.txt
   Include subdirectories? (y/n, default: y): y
   ```

4. View the search results:
   ```
   Search Results (5 files found):
   C:\Users\username\Documents\note1.txt
   C:\Users\username\Documents\note2.txt
   C:\Users\username\Documents\subfolder\note3.txt
   C:\Users\username\Documents\subfolder\note4.txt
   C:\Users\username\Documents\another\note5.txt
   ```