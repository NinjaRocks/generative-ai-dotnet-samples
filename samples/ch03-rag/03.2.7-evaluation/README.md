# Chapter 3.2.7 -- RAG Evaluation Harness

Companion code for **Generative AI in .NET**, Chapter 3 section 3.2.7 ("Evaluation: Measuring RAG Quality").

A minimal LLM-as-judge harness that scores candidate answers on **faithfulness** (supported by ground truth) and **relevance** (answers the question). The judge returns structured JSON via `GetResponseAsync<JudgeVerdict>(...)`, so scores are typed integers, not parsed strings.

The bundled `golden-dataset.json` is a 3-row demo. Replace it with your own questions/ground-truths and plug in your real RAG pipeline's answer in place of the `candidateAnswer` placeholder.

## Run it

```bash
export OPENAI_API_KEY=sk-...
dotnet run --project samples/ch03-rag/03.2.7-evaluation
```

## Manuscript reference

- `Manuscript/Chapter-03.md`, section 3.2.7.
- Figure 3.8 ("RAG Evaluation Pipeline") visualizes the same harness with a CI gate.

## Prerequisites

- .NET 9 SDK and an OpenAI API key (or any `IChatClient` provider).
