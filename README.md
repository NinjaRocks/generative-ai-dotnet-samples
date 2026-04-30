# Generative AI in .NET -- Companion Code

Runnable code samples for **Generative AI in .NET** (Najaf Shaikh).

Every sample in this repo corresponds to a section of the printed book. The full map between book and code lives in [`docs/citation-index.md`](docs/citation-index.md). When the book has been condensed in favor of pointing here, the manuscript cites a **tag** (e.g. `v1.0-print-ready`) so the code stays aligned with the print run.

## Layout

```
samples/
  ch01-foundations/
    01.3-hello-chat/                    -- IChatClient with provider selection
    01.3-embeddings/                    -- IEmbeddingGenerator + cosine similarity
    01.4-secrets-and-config/            -- appsettings + user-secrets + env vars
  ch02-extensions-ai/
    02.1-console-chat-loop/             -- Sliding-window chat REPL
    02.2-streaming-aspnet/              -- SSE streaming in a Minimal API
    02.2-structured-output/             -- GetResponseAsync<T>()
    02.3-function-calling/              -- Three-tool weather + calendar + reminders
    02.4-middleware-pipeline/           -- Logging + caching pipeline
    02.4-custom-middleware/             -- Token-budget DelegatingChatClient
  ch03-rag/
    03.1-semantic-search/               -- Embedding + cosine over a product catalog
    03.2-rag-basic/                     -- Retrieve / augment / generate
    03.2-ingestion-pipeline/            -- Load + chunk + hash + embed + upsert
    03.2.7-evaluation/                  -- LLM-as-judge for RAG (faithfulness + relevance)
    03.3-vision-extract/                -- Receipt image -> typed Receipt record
    03.5-local-rag/                     -- Local-only RAG (Ollama)
  ch04-agent-framework/
    04.2.1-hello-agent/                 -- ChatClientAgent hello world
    04.2.4-anthropic-agents/            -- Same agent API, Claude under the hood
    04.3-persistent-session/            -- AgentThread serialize / resume
    04.4-tools-and-approval/            -- Function tools + RequiresApproval marker
    04.5-agent-middleware/              -- Custom run-middleware
    04.6.3-text-processing-walkthrough/ -- Linear workflow walkthrough
    04.6.7-content-workflow/            -- Researcher / Writer / Editor multi-agent
    04.7-a2a-server/                    -- Expose an agent via A2A protocol
    04.8-agent-with-mcp/                -- Agent with mixed local + MCP tools
  ch05-mcp/
    05.2.10-inventory-server/           -- Full MCP server (tools + resources + prompts)
    05.3.1-stdio-transport/             -- Minimal stdio server
    05.3.2-sse-transport/               -- ASP.NET Core SSE transport
    05.3.3-streamable-http/             -- Streamable HTTP (recommended)
    05.4-mcp-client/                    -- Interactive MCP client REPL
    05.6.3-mcp-agent-factory/           -- McpAgentFactory
    cached-mcp-tool-provider/           -- CachedMcpToolProvider (Critical-1 fix)
  ch06-production/
    06.1-observability/                 -- OpenTelemetry tracing + OTLP export
    06.2.1-resilience/                  -- Standard resilience handler
    06.2.4-model-routing/               -- Classify-then-route cost optimization
    06.5.1-unit-testing/                -- xUnit + StubChatClient
    06.5.3-llm-as-judge/                -- Evaluation harness
    output-guard-chat-client/           -- OutputGuardChatClient (Critical-3 fix)
docs/
  citation-index.md
  version-matrix.md
.github/workflows/ci.yml
Directory.Packages.props
global.json
```

## Prerequisites

- **.NET 9 SDK** (`dotnet --version` should be 9.0.x).
- **Provider credentials** for samples that hit a live model -- each sample's README names what it needs:
  - `OPENAI_API_KEY` for OpenAI / GitHub Models samples.
  - `AZURE_OPENAI_ENDPOINT` + `AZURE_OPENAI_KEY` for Azure OpenAI samples.
  - `ANTHROPIC_API_KEY` for Claude samples.
- **Local services** for offline samples: [Ollama](https://ollama.com/) for local inference; an MCP-compatible runtime (`dnx`) for some MCP samples.

The repo uses **central package management** -- versions live in [`Directory.Packages.props`](Directory.Packages.props), shared properties in [`samples/Directory.Build.props`](samples/Directory.Build.props). Project files only carry `<PackageReference Include="..." />` -- no version attributes.

## Running a sample

```bash
dotnet run --project samples/ch04-agent-framework/04.6.3-text-processing-walkthrough
```

Each sample folder has its own `README.md` with run instructions, expected output, and the manuscript section it backs.

## Building everything

```bash
dotnet build
```

## What's offline-runnable

These samples need **only** Ollama (no API keys, no cloud calls):

- `ch01/01.3-hello-chat` (default profile), `01.3-embeddings`, `01.4-secrets-and-config`
- `ch02/02.1-console-chat-loop`, `02.2-streaming-aspnet`, `02.4-middleware-pipeline`, `02.4-custom-middleware`
- `ch03/03.1-semantic-search`, `03.2-rag-basic`, `03.2-ingestion-pipeline`, `03.5-local-rag`
- `ch04/04.2.1-hello-agent`, `04.3-persistent-session`, `04.5-agent-middleware`, `04.7-a2a-server`
- `ch05/*` (all MCP samples; `05.4-mcp-client` needs a server to talk to)
- `ch06/06.1-observability`, `06.2.1-resilience`, `06.5.1-unit-testing`, `output-guard-chat-client`

The rest assume an OpenAI / Anthropic / Azure OpenAI key.

## 1.x API surface (Agent Framework 1.3 / MCP 1.2)

All samples target `Microsoft.Agents.AI` 1.3 and `ModelContextProtocol` 1.2. The manuscript was originally drafted against the 0.3.x previews, so the printed code occasionally differs from what's in this repo. The original 0.3-preview snippets are preserved alongside the live code as `*.cs.book.txt` for traceability. The shape changes that recur most often:

- `ChatClientAgentOptions` no longer carries `Instructions` / `Tools` -- they move to `ChatClientAgentOptions.ChatOptions` (`Instructions`, `Tools`) or to the string-arg `ChatClientAgent` constructor.
- `AgentThread` -> `AgentSession`; `AgentRunResponse` -> `AgentResponse`; `AIAgent.GetNewThread()` -> `AIAgent.CreateSessionAsync(...)`. Session serialization is `agent.SerializeSessionAsync(session)` / `agent.DeserializeSessionAsync(jsonElement)`.
- `WorkflowBuilder()` parameterless ctor is gone -- the start executor is passed to the constructor (`new WorkflowBuilder(start)`). `Executor.From` / `FromAsync` lambdas became `new FunctionExecutor<TIn, TOut>(id, handler)`. `SetOutputExecutor` -> `WithOutputFrom(...)`. `workflow.RunAsync(...)` was replaced by `InProcessExecution.RunAsync(workflow, input)`.
- The `AsBuilder().Use(...)` middleware delegate now receives `(messages, session, options, innerAgent, ct)` -- you call `innerAgent.RunAsync(...)` instead of a `next` delegate.
- `ModelContextProtocol.Client` / `ModelContextProtocol.Server` are no longer separate packages -- everything is in `ModelContextProtocol` / `ModelContextProtocol.Core` / `ModelContextProtocol.AspNetCore`. `McpClientTool` now extends `AIFunction` directly (no `.AsAIFunction()` needed); rename via `tool.WithName(...)`.
- Server bootstrap is `.AddMcpServer().WithHttpTransport().WithToolsFromAssembly()` then `app.MapMcp(...)`. `WithHttpServerTransport(...)` and `HttpTransportType.Sse` have been retired -- streamable HTTP is the only HTTP transport.
- Client transports: `StreamableHttpClientTransport` -> `HttpClientTransport` (with `HttpClientTransportOptions`). `SseClientTransport` is gone.
- A2A server hosting requires implementing `A2A.IAgentHandler`, registering it via `services.AddA2AAgent<THandler>(agentCard)`, then `app.MapA2A("/path")` from `A2A.AspNetCore`. The 0.3-preview's one-liner `app.MapA2A(agent, "/path")` no longer exists.
- `McpClient.OnToolsListChanged(...)` was replaced by `client.RegisterNotificationHandler(NotificationMethods.ToolListChangedNotification, handler)`, which returns an `IAsyncDisposable`.

## Versioning and tags

| Tag | Meaning |
|---|---|
| `v1.0-print-ready` | Code as it appears in the first print run. |
| `v1.x-second-print` | Updated for subsequent print revisions. |
| `main` | Always-current; may drift from any specific print run. |

Use a **tag** in any URL you cite from the book -- never `main`.

## Errata

Open an issue at [github.com/CodeShayk/generative-ai-dotnet-samples](https://github.com/CodeShayk/generative-ai-dotnet-samples). Confirmed errata roll into the next print tag and are noted in the parent book's errata page.

## License

MIT -- see [`LICENSE`](LICENSE).
