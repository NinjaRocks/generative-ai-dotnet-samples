# Version Matrix

Mirrors the package matrix in `Appendix-C-Provider-Support-Matrix.md` and `Appendix-A-Package-Reference.md`. Maintain independently of the manuscript so readers can see the *currently-validated* set without rebuying the book.

> **Note.** Versions below are the build-time pins from `Directory.Packages.props`. Per the Critical-5 cadence: weekly until print, monthly for the first six months post-print, quarterly thereafter. When a major or minor version moves, update both `Directory.Packages.props` and this file together.

## Core abstractions

| Package | Pinned version | Stability |
|---|---|---|
| `Microsoft.Extensions.AI` | 10.5.1 | Stable |
| `Microsoft.Extensions.AI.Abstractions` | 10.5.1 | Stable |
| `Microsoft.Extensions.AI.OpenAI` | 10.5.1 | Stable |
| `Microsoft.Extensions.AI.Evaluation` | 10.5.1 | Stable |
| `Microsoft.Extensions.AI.Evaluation.Quality` | 10.5.1 | Stable |

## Microsoft Agent Framework

| Package | Pinned version | Stability |
|---|---|---|
| `Microsoft.Agents.AI` | 1.3.0 | Stable |
| `Microsoft.Agents.AI.Abstractions` | 1.3.0 | Stable |
| `Microsoft.Agents.AI.OpenAI` | 1.3.0 | Stable |
| `Microsoft.Agents.AI.Workflows` | 1.3.0 | Stable |
| `Microsoft.Agents.AI.Foundry` | 1.3.0 | Stable |
| `Microsoft.Agents.AI.A2A` | 1.3.0-preview.260423.1 | Preview |
| `Microsoft.Agents.AI.Hosting` | 1.3.0-preview.260423.1 | Preview |
| `A2A.AspNetCore` | 1.0.0-preview2 | Preview |

## Model Context Protocol

| Package | Pinned version | Stability |
|---|---|---|
| `ModelContextProtocol` | 1.2.0 | Stable |
| `ModelContextProtocol.Core` | 1.2.0 | Stable |
| `ModelContextProtocol.AspNetCore` | 1.2.0 | Stable |

## Provider SDKs

| Package | Pinned version | Stability |
|---|---|---|
| `Azure.AI.OpenAI` | 2.1.0 | Stable |
| `OpenAI` | 2.10.0 | Stable |
| `OllamaSharp` | 5.4.25 | Stable |
| `Anthropic.SDK` | 5.10.0 | Stable |

## Hosting / DI / ASP.NET Core

| Package | Pinned version |
|---|---|
| `Microsoft.Extensions.Hosting` | 9.0.0 |
| `Microsoft.Extensions.DependencyInjection` | 9.0.0 |
| `Microsoft.Extensions.Configuration` | 9.0.0 |
| `Microsoft.Extensions.Logging` | 9.0.0 |
| `Microsoft.AspNetCore.OpenApi` | 9.0.0 |

## Resilience and Telemetry

| Package | Pinned version |
|---|---|
| `Microsoft.Extensions.Http.Resilience` | 9.0.0 |
| `Microsoft.Extensions.Caching.Memory` | 9.0.0 |
| `OpenTelemetry.Extensions.Hosting` | 1.10.0 |
| `OpenTelemetry.Exporter.OpenTelemetryProtocol` | 1.10.0 |

## Last validated

- **2026-05-03** -- NuGet sweep; bumped `Microsoft.Extensions.AI` family from `10.5.0` to `10.5.1`. All other Critical-5 pins confirmed at their current stable ceiling. Version matrix rewritten from stale 0.3.x-preview baseline.
