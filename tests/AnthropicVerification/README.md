# Anthropic Live-API Smoke Harness

Throwaway test project used to verify, against the live Anthropic API, that the Claude model identifiers cited in the manuscript are still callable. Lives outside `AI in .Net.sln` on purpose -- it is not part of the shipped sample set.

## What it does

Iterates the three model IDs cited in `Manuscript/Appendix-B-Model-Quick-Reference.md` and `Manuscript/Chapter-04.md`, sends each one a fixed `"Reply with exactly: OK"` prompt via `IChatClient`, and prints `PASS`/`FAIL` per model. Exits 0 if all three respond, 2 if any fail, 1 if `ANTHROPIC_API_KEY` is unset.

## Why it pins `Microsoft.Extensions.AI` to 10.3.0

`Anthropic.SDK` 5.10.0 (latest stable at time of writing) is compiled against `Microsoft.Extensions.AI.Abstractions` 10.3.0. The repo's central pin is 10.5.0, which reshapes `HostedMcpServerTool.AuthorizationToken` -- runtime calls through the SDK's `IChatClient` bridge throw `MissingMethodException`. The harness sidesteps this by overriding the pin locally:

```xml
<PackageReference Include="Microsoft.Extensions.AI" VersionOverride="10.3.0" />
<PackageReference Include="Microsoft.Extensions.AI.Abstractions" VersionOverride="10.3.0" />
```

The override works here because the harness has no `Microsoft.Agents.AI` dependency. The chapter sample at `samples/ch04-agent-framework/04.2.4-anthropic-agents/` cannot use the same override (Agents.AI 1.3 is itself compiled against M.E.AI 10.5 and refuses to bind to 10.3 with `CS1705`); see that sample's README and `docs/verification-log.md` for the workaround status.

When `Anthropic.SDK` 5.11+ ships rebuilt against M.E.AI 10.5+, drop both overrides.

## Run it

```bash
cd code/generative-ai-dotnet-samples
ANTHROPIC_API_KEY='sk-ant-...' dotnet run --project tests/AnthropicVerification
```

Expected output:

```
PASS  claude-opus-4-7: OK
PASS  claude-sonnet-4-6: OK
PASS  claude-haiku-4-5-20251001: OK
```

If a model returns `FAIL`, update `Manuscript/Appendix-B-Model-Quick-Reference.md` to the current Anthropic-published identifier and re-run.

## When to re-run

- During the four pre-print weekly verification passes.
- Whenever Anthropic announces a new Claude release (model IDs include date suffixes that may roll forward).
- After bumping `Anthropic.SDK` or `Microsoft.Extensions.AI*` versions.

Record results in `docs/verification-log.md`.
