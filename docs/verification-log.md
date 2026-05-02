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
