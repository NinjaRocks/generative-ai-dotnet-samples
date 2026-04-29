# Chapter 6 -- Production Readiness and Responsible AI

Companion samples for **Chapter 6** of *Generative AI in .NET*.

| Sample | Section | What it shows |
|---|---|---|
| [`06.1-observability/`](06.1-observability/) | 6.1.2 -- 6.1.4 | OpenTelemetry tracing for `IChatClient`, with console + OTLP exporters. |
| [`06.2.1-resilience/`](06.2.1-resilience/) | 6.2.1 | `AddStandardResilienceHandler` -- retry + breaker + timeouts -- driven by a flaky stub. |
| [`06.2.4-model-routing/`](06.2.4-model-routing/) | 6.2.4 | Classify-then-route: cheap model for simple queries, escalate complex ones. |
| [`06.5.1-unit-testing/`](06.5.1-unit-testing/) | 6.5.1 | xUnit tests against a `StubChatClient`. |
| [`06.5.3-llm-as-judge/`](06.5.3-llm-as-judge/) | 6.5.3 | Evaluation pipeline scoring relevance / groundedness / coherence. |
| [`output-guard-chat-client/`](output-guard-chat-client/) | 6.3.2 | `OutputGuardChatClient` (Critical-3 fix). |

Every sample uses the central `Directory.Packages.props` for version pinning.
