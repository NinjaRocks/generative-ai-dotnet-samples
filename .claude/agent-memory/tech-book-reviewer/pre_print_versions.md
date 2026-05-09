---
name: Pre-print version pinning conventions
description: Canonical package version pins for the Generative AI in .NET book and known consistency hotspots
type: project
---

Canonical version targets for the manuscript (verified 2026-05-09):

- Microsoft.Extensions.AI 10.5.1 (Evaluation pkg stays at 10.5.0)
- Microsoft.Agents.AI 1.3.0 (Workflows / OpenAI / Foundry stable; A2A / Hosting preview 1.3.0-preview.*)
- ModelContextProtocol 1.2.0 (single facade package; .Client / .Server packages retired in 1.x)
- Anthropic.SDK 5.10.0 (uses local `AnthropicChatClient` adapter due to MEAI.Abstractions binding gap)
- OllamaSharp 5.4.25
- OpenAI 2.10.0
- Azure.AI.OpenAI 2.1.0

**Why:** These are the pins agreed for the v1.0-first-print tag.

**How to apply:** When reviewing or editing prose, treat any of the following as defects:
- "Microsoft.Extensions.AI 9.x" / "9.4.0" / floating "9.*" in csproj snippets — should be 10.x (10.5.1).
- "OllamaSharp 5.1.12" — should be 5.4.25.
- Any "Last validated" blockquote that says 9.x — should match 10.5.1.
- Any csproj `Version="9.*"` in chapters 4/5 — should be 10.* now.
- `using ModelContextProtocol.Client;` or `.Server;` in **prose package lists** is fine because the namespaces are preserved for backward compat; only flag if it's listed as a separate **package** to install.
- `AgentSession` is the correct name (not `AgentThread`); methods are `SerializeSessionAsync` / `DeserializeSessionAsync`.

Citation URLs always use the `v1.0-first-print` tag, never `master`/`main`. Branch on the companion repo is `master`. The book repo is also `master`.
