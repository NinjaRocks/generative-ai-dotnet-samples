# Chapter 3 -- Build AI Applications

Companion samples for **Chapter 3** of *Generative AI in .NET*.

| Sample | Section | What it shows |
|---|---|---|
| [`03.1-semantic-search/`](03.1-semantic-search/) | 3.1.1 -- 3.1.5 | Embedding + cosine similarity over a tiny in-memory product catalog. |
| [`03.2-rag-basic/`](03.2-rag-basic/) | 3.2.1 -- 3.2.6 | Retrieve / augment / generate against a 5-row policy KB. |
| [`03.2-ingestion-pipeline/`](03.2-ingestion-pipeline/) | 3.2.2 | Load -> chunk (with overlap) -> hash -> embed -> upsert. |
| [`03.2.7-evaluation/`](03.2.7-evaluation/) | 3.2.7 | LLM-as-judge harness scoring faithfulness + relevance from a golden dataset. |
| [`03.3-vision-extract/`](03.3-vision-extract/) | 3.3.4 | Receipt image -> typed `Receipt` record (multimodal + structured output). |
| [`03.5-local-rag/`](03.5-local-rag/) | 3.5.1 | End-to-end local RAG (Ollama chat + embeddings, no cloud). |

Every sample uses the central `Directory.Packages.props` for version pinning.
