# Chapter 2 -- Generative AI Techniques with Microsoft.Extensions.AI

Companion samples for **Chapter 2** of *Generative AI in .NET*.

| Sample | Section | What it shows |
|---|---|---|
| [`02.1-console-chat-loop/`](02.1-console-chat-loop/) | 2.1.5 | Console REPL with sliding-window history. |
| [`02.2-streaming-aspnet/`](02.2-streaming-aspnet/) | 2.2.2 | Minimal API with SSE streaming + browser client. |
| [`02.2-structured-output/`](02.2-structured-output/) | 2.2.3 -- 2.2.4 | `GetResponseAsync<T>()` with `Description` attributes. |
| [`02.3-function-calling/`](02.3-function-calling/) | 2.3.6 | Three local tools surfaced via `UseFunctionInvocation()`. |
| [`02.4-middleware-pipeline/`](02.4-middleware-pipeline/) | 2.4.6 | Composing logging + distributed cache. |
| [`02.4-custom-middleware/`](02.4-custom-middleware/) | 2.4.5 | A custom `DelegatingChatClient` enforcing a daily token budget. |

Every sample uses the central `Directory.Packages.props` for version pinning.
