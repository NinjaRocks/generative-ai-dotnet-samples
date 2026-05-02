# Chapter 4.2.4 -- Anthropic / Claude as a ChatClientAgent

Companion code for **Generative AI in .NET**, Chapter 4 section 4.2.4 ("Anthropic Agents").

Uses `new AnthropicClient(apiKey).Messages` -- which already implements `IChatClient` -- and hands it to a standard `ChatClientAgent`. The same agent API works against OpenAI, Azure OpenAI, Ollama, etc.

> **Known issue (2026-05-02): this sample does not run as-shipped against the repo's central package pins.** It builds clean, but at runtime `ChatClientAgent.RunAsync(...)` throws `MissingMethodException: Method not found: 'System.String Microsoft.Extensions.AI.HostedMcpServerTool.get_AuthorizationToken()'.`. Root cause: `Anthropic.SDK` 5.10.0 was compiled against `Microsoft.Extensions.AI.Abstractions` 10.3.0; the repo pins 10.5.0 (required by `Microsoft.Agents.AI` 1.3.0), which reshapes that property. Per-project `VersionOverride` to 10.3.0 does not help here -- Agents.AI 1.3 itself rejects 10.3 with `CS1705` at compile time. **Mitigation paths under evaluation** (see [`docs/verification-log.md`](../../../docs/verification-log.md), entry `2026-05-02`): (1) wait for `Anthropic.SDK` 5.11+ rebuilt against 10.5+, (2) ship a thin custom `IChatClient` adapter that calls `AnthropicClient.Messages.GetClaudeMessageAsync` directly, or (3) re-target the sample at Anthropic's OpenAI-compatibility endpoint via the OpenAI SDK. The chapter prose still describes the intended pattern; once the underlying packages reconcile, the sample runs unchanged.

## Run it

```bash
export ANTHROPIC_API_KEY=sk-ant-...
dotnet run --project samples/ch04-agent-framework/04.2.4-anthropic-agents
```

Override the model with `ANTHROPIC_MODEL` (defaults to `claude-haiku-4-5-20251001`).

## Manuscript reference

- `Manuscript/Chapter-04.md`, section 4.2.4.
- `Manuscript/Appendix-B-Model-Quick-Reference.md` for current Claude model IDs.

## Prerequisites

- .NET 9 SDK and an Anthropic API key.

## Verifying model IDs are still callable

A throwaway harness at [`tests/AnthropicVerification/`](../../../tests/AnthropicVerification/) hits the live API with each model ID listed in Appendix B. The harness has no `Microsoft.Agents.AI` dependency, so it can pin to M.E.AI 10.3.0 and run end-to-end -- use it during the pre-print verification passes to confirm Anthropic has not rotated the cited identifiers.
