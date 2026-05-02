# Verification Log

Pre-publication verification record for *Generative AI in .NET*. One entry per pass. The most-recent fully-green entry is the print sign-off gate.

**Cadence:**

- **Pre-print:** weekly for the four weeks before print drop.
- **Post-print:** monthly for six months, then quarterly (drives errata releases).

**What each entry covers:** package versions (Critical-5 list), code samples (build + smoke runs), URLs (chapter links + appendices), Anthropic API surface (model IDs in Appendix B + Chapter 4.2.4).

**Sign-off gate before print:**

- [ ] Most recent week is fully green.
- [ ] No package on the watch list has a known breaking change pending.
- [ ] Companion repo CI green on the latest commit.
- [ ] Companion repo tagged `v1.0-first-print` matching the manuscript version.

---

## Template

Copy this block to start a new entry. Date format `YYYY-MM-DD`.

```markdown
## YYYY-MM-DD verification pass

### Packages (Critical-5 list)
- [ ] Microsoft.Extensions.AI -- vX.Y.Z, no change / changelog reviewed
- [ ] Microsoft.Extensions.AI.Abstractions -- ...
- [ ] Microsoft.Extensions.AI.OpenAI -- ...
- [ ] Microsoft.Extensions.AI.Ollama -- ...
- [ ] Microsoft.Extensions.AI.AzureAIInference -- ...
- [ ] Microsoft.Agents.AI -- ...
- [ ] Microsoft.Agents.AI.OpenAI -- ...
- [ ] Microsoft.Agents.AI.Workflows -- ...
- [ ] Microsoft.Agents.AI.AzureAI -- ...
- [ ] ModelContextProtocol -- ...
- [ ] ModelContextProtocol.Core -- ...
- [ ] ModelContextProtocol.AspNetCore -- ...
- [ ] Microsoft.McpServer.ProjectTemplates -- ...
- [ ] Microsoft.Azure.Functions.Worker.Extensions.Mcp -- ...
- [ ] Azure.AI.OpenAI -- ...
- [ ] OpenAI -- ...
- [ ] OllamaSharp -- ...
- [ ] Anthropic.SDK -- ...

### Code samples
- [ ] CI matrix green on commit `<sha>`
- [ ] Live-API smoke tests green (or skipped, with reason)

### URLs
- [ ] Anthropic / Claude documentation links resolve
- [ ] Microsoft Learn links resolve
- [ ] Azure documentation links resolve
- [ ] NuGet package pages resolve

### Anthropic API surface
- [ ] Every model ID in `Appendix-B-Model-Quick-Reference.md` is callable
- [ ] Every model ID in `Chapter-04.md` section 4.2.4 is callable
- [ ] `Anthropic.SDK` API surface used in the chapter examples matches the latest stable

### Issues found / actions taken
- (none) | <description + commit ref + manuscript section touched>
```

---

## 2026-04-30 -- Initial sweep (kickoff)

**Status:** Partial -- snapshot only; subsequent weeks will be full passes.

### Packages
- [x] Critical-5 list re-verified against live NuGet feed; companion repo pinned to `Microsoft.Agents.AI 1.3` and `ModelContextProtocol 1.2`.
- [x] All 37 sample projects build clean on these versions (companion-repo commits `0047e61` + `35e6fd3`).
- [x] CI matrix green (run 25136332826).

### Code samples
- [x] All 37 samples build clean.
- [ ] Live-API smoke tests -- not yet wired up (P2-1 in next-steps-plan).

### URLs
- [ ] Not run yet -- queue for the first scheduled weekly pass.

### Anthropic API surface
- [ ] Not run yet -- queue for the first scheduled weekly pass (P0-4 in next-steps-plan).

### Issues found / actions taken
- 15 placeholder samples ported to 1.x stable APIs (book-repo commit `09bb7d9` cleared the API-update-pending punch list).

---

*New entries go below this line, most recent first.*

---

## 2026-05-02 -- P0-4 Anthropic live-API smoke

**Status:** Yellow -- model IDs and URLs verified green; one real defect (M.E.AI ↔ Anthropic.SDK runtime binding gap) blocks the existing Anthropic sample and must be cleared before print.

**Test runs in this entry:** initial pass blocked on zero credit balance; re-run later the same day with funded key returned all three model IDs `OK`.

### Packages
- [x] `Anthropic.SDK` -- v5.10.0 (latest stable on NuGet); central pin unchanged.
- [x] `Microsoft.Extensions.AI` -- v10.5.0 central pin; harness overrides to v10.3.0 to match Anthropic.SDK's compile-time target (see Issues).

### Code samples
- [x] Verification harness at `tests/AnthropicVerification/` builds clean with M.E.AI 10.3.0 override.
- [x] Live-API model-ID smoke through the harness -- **green for all three IDs** (`PASS  claude-opus-4-7: OK`; `PASS  claude-sonnet-4-6: OK`; `PASS  claude-haiku-4-5-20251001: OK`).
- [ ] Existing `samples/ch04-agent-framework/04.2.4-anthropic-agents` -- builds clean but **fails at runtime** with `MissingMethodException` from `Anthropic.SDK.Messaging.ChatClientHelper.CreateMessageParameters` -> `Microsoft.Extensions.AI.FunctionInvokingChatClient.GetResponseAsync` -> `Microsoft.Agents.AI.ChatClientAgent.RunCoreAsync`. Re-confirmed with funded key on 2026-05-02; the defect is independent of credits (see Issues).

### URLs
- [x] `https://docs.anthropic.com/` (cited at `Manuscript/Chapter-04.md:3609`) -- HTTP 200.
- [x] `https://platform.claude.com/docs/en/about-claude/models/overview` (cited at `Manuscript/Appendix-B-Model-Quick-Reference.md:36`) -- HTTP 200.
- [x] Sanity: `https://platform.claude.com/`, `https://docs.anthropic.com/en/docs/about-claude/models/overview` -- both HTTP 200.
- [ ] Microsoft Learn / Azure / NuGet links -- deferred; not in the P0-4 scope.

### Anthropic API surface
- [x] Model IDs in `Manuscript/Appendix-B-Model-Quick-Reference.md:32-34` and `Manuscript/Chapter-04.md:557` (`claude-opus-4-7`, `claude-sonnet-4-6`, `claude-haiku-4-5-20251001`) -- all three callable; harness round-trip returned the expected `OK` response.
- [x] `Anthropic.SDK` package surface used in chapter examples (`new AnthropicClient(key).Messages` as `IChatClient`) -- wire-compatible with the live API; full request/response round-trip confirmed when paired with M.E.AI 10.3.0.

### Issues found / actions taken
- **Defect (blocks print): `samples/ch04-agent-framework/04.2.4-anthropic-agents` does not run against the repo's central pin.** Throws `MissingMethodException: Method not found: 'System.String Microsoft.Extensions.AI.HostedMcpServerTool.get_AuthorizationToken()'.` from `Anthropic.SDK.Messaging.ChatClientHelper.CreateMessageParameters`. Cause: `Anthropic.SDK` 5.10.0's nuspec declares `Microsoft.Extensions.AI.Abstractions >= 10.3.0` but the assembly is compiled against 10.3.0 specifically; M.E.AI 10.5.0 changed the shape of `HostedMcpServerTool.AuthorizationToken` (property-getter no longer present). CI is green because builds succeed against the *abstractions* package surface; runtime binding does not.
    - **Tried: per-project `VersionOverride` to 10.3.0 (initially proposed as option 3).** Verified to *not work* for this sample: `Microsoft.Agents.AI` 1.3.0 is itself compiled against `Microsoft.Extensions.AI.Abstractions` 10.5.0, so downgrading abstractions to 10.3.0 raises `CS1705 (uses higher version than referenced assembly)` at compile time. Override reverted; sample is back to clean build, broken runtime.
    - **The harness avoids the conflict only because it has no `Microsoft.Agents.AI` dependency** -- pure Anthropic.SDK + M.E.AI. Override pin 10.3.0 stays on the harness as the runtime mitigation there.
    - **Remaining viable paths (pre-print decision):**
        1. Wait for `Anthropic.SDK` 5.11+ rebuilt against M.E.AI 10.5+. Cleanest, but not on NuGet as of 2026-05-02 (latest stable: 5.10.0). Watch <https://www.nuget.org/packages/Anthropic.SDK>.
        2. Replace Anthropic.SDK's IChatClient bridge with a small custom `IChatClient` that calls `AnthropicClient.Messages.GetClaudeMessageAsync` directly. ~50 lines in the sample; bypasses the broken `ChatClientHelper`. Changes the chapter narrative slightly (the sample now ships a thin adapter rather than relying on the SDK's built-in adapter).
        3. Switch the sample to the OpenAI SDK pointed at Anthropic's OpenAI-compatibility endpoint. Strongest provider-neutral story, but rewrites the chapter section.
- **CI gap:** the build matrix needs at least one runtime-execution lane for samples that have non-cloud-key prerequisites (currently 04.2.4 needs ANTHROPIC_API_KEY so cannot run unattended; consider adding a stub-key smoke that exercises the M.E.AI binding via `IChatClient` invocation up to the wire-send point).
- Verification harness lives at `tests/AnthropicVerification/` (throwaway, not in `AI in .Net.sln`). Re-run: `ANTHROPIC_API_KEY=<key> dotnet run --project tests/AnthropicVerification`.

### Next-pass to-dos
- [ ] Pick one of paths 1-3 above for the chapter sample. If path 1 (wait for SDK 5.11), open a tracking issue and re-check NuGet weekly. If path 2 (custom IChatClient), implement the bridge, re-run, confirm green. If path 3 (OpenAI-compat endpoint), revise the chapter prose for §4.2.4.
- [ ] Run the broader URL audit (Microsoft Learn, Azure, NuGet) and fold into the next weekly entry.
