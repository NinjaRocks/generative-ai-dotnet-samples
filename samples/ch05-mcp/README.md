# Chapter 5 -- Model Context Protocol (MCP) in .NET

Companion samples for **Chapter 5** of *Generative AI in .NET*.

| Sample | Section | What it shows |
|---|---|---|
| [`05.2.10-inventory-server/`](05.2.10-inventory-server/) | 5.2.10 | Full MCP server -- tools + resources + prompts + `.mcp/server.json` for NuGet packing. |
| [`05.3.1-stdio-transport/`](05.3.1-stdio-transport/) | 5.3.1 | Minimal stdio server; logs go to stderr only. |
| [`05.3.2-sse-transport/`](05.3.2-sse-transport/) | 5.3.2 | ASP.NET Core SSE transport with an `Echo` tool. |
| [`05.3.3-streamable-http/`](05.3.3-streamable-http/) | 5.3.3 | Modern streamable-HTTP transport (recommended). |
| [`05.4-mcp-client/`](05.4-mcp-client/) | 5.4.1 -- 5.4.3 | Interactive MCP client REPL (stdio + HTTP). |
| [`05.6.3-mcp-agent-factory/`](05.6.3-mcp-agent-factory/) | 5.6.3 | `McpAgentFactory` -- agents that discover tools at startup. |
| [`cached-mcp-tool-provider/`](cached-mcp-tool-provider/) | 5.4.2 | `CachedMcpToolProvider` (Critical-1 fix). |

Every sample uses the central `Directory.Packages.props` for version pinning.
