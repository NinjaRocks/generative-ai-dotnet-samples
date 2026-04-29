# Chapter 4.2.4 -- Anthropic / Claude as a ChatClientAgent

Companion code for **Generative AI in .NET**, Chapter 4 section 4.2.4 ("Anthropic Agents").

Wraps `Anthropic.SDK`'s message client as `IChatClient` via `.AsIChatClient(modelId)`, then hands it to a standard `ChatClientAgent`. The exact same agent API works against OpenAI, Azure OpenAI, Ollama, etc.

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
