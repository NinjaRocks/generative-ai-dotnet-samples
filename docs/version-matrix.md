# Version Matrix

Mirrors the package matrix in `Appendix-C-Provider-Support-Matrix.md` and `Appendix-A-Package-Reference.md`. Maintain independently of the manuscript so readers can see the *currently-validated* set without rebuying the book.

> **Note.** Versions below are the build-time pins from `Directory.Packages.props`. Run the package sweep documented in `Manuscript/user-action-plan.md` (Critical-5) on the cadence noted there, and update both files together.

## Core abstractions

| Package | Pinned version | Stability |
|---|---|---|
| `Microsoft.Extensions.AI` | 9.0.0 | Stable |
| `Microsoft.Extensions.AI.Abstractions` | 9.0.0 | Stable |
| `Microsoft.Extensions.AI.OpenAI` | 9.0.0-preview | Preview |
| `Microsoft.Extensions.AI.Ollama` | 9.0.0-preview | Preview |
| `Microsoft.Extensions.AI.AzureAIInference` | 9.0.0-preview | Preview |

## Microsoft Agent Framework

| Package | Pinned version | Stability |
|---|---|---|
| `Microsoft.Agents.AI` | 0.3.0-preview | Preview |
| `Microsoft.Agents.AI.OpenAI` | 0.3.0-preview | Preview |
| `Microsoft.Agents.AI.Workflows` | 0.3.0-preview | Preview |
| `Microsoft.Agents.AI.AzureAI` | 0.3.0-preview | Preview |
| `Microsoft.Agents.AI.A2A` | 0.3.0-preview | Preview |
| `Microsoft.Agents.AI.Hosting` | 0.3.0-preview | Preview |

## Model Context Protocol

| Package | Pinned version | Stability |
|---|---|---|
| `ModelContextProtocol` | 0.3.0-preview | Preview |
| `ModelContextProtocol.Client` | 0.3.0-preview | Preview |
| `ModelContextProtocol.Server` | 0.3.0-preview | Preview |
| `ModelContextProtocol.AspNetCore` | 0.3.0-preview | Preview |

## Provider SDKs

| Package | Pinned version |
|---|---|
| `Azure.AI.OpenAI` | 2.1.0 |
| `OpenAI` | 2.1.0 |
| `OllamaSharp` | 4.0.0 |
| `Anthropic.SDK` | 5.0.0 |

## Last validated

- **2026-04-29** -- initial pin, set during companion repo scaffolding. Re-verify against live NuGet before tagging `v1.0-first-print`.
