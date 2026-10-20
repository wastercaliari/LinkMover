# CALCULATIONS (Livro-Razão de Cálculos e Dimensionamento de Engenharia)

Este documento é estritamente **APPEND-ONLY**. Contém benchmarks e orçamentos teóricos de contexto, tokens e latência para a operação multi-agente.

---

### C-1: Orçamento de Contexto dos Agentes por Plataforma LLM (2026-10-19)

| Plataforma / Modelo | Context Window Total | Peso dos Arquivos (/agents + /docs) | % de Contexto Utilizado | Tokens Livres para Código do Usuário |
|---|---|---|---|---|
| **Claude 3.5 Sonnet / Opus** | 200.000 tokens | ~15.000 tokens | 7.5% | ~185.000 tokens |
| **OpenAI GPT-4o / o1 / o3** | 128.000 / 200.000 tokens | ~15.000 tokens | 7.5% a 11.7% | ~113.000 a 185.000 tokens |
| **DeepSeek V3 / R1** | 64.000 / 128.000 tokens | ~15.000 tokens | 11.7% a 23.4% | ~49.000 a 113.000 tokens |
| **Gemini 1.5 Pro / Flash** | 1.000.000 a 2.000.000 tokens | ~15.000 tokens | < 1.5% | > 985.000 tokens |
| **Modelos Locais (Ollama 32k)** | 32.768 tokens | ~15.000 tokens (ou ~2.500 no Mestre) | 7.6% (Mestre) a 45.7% (Full) | ~17.700 a 30.000 tokens |

*Conclusão Matemática*: O arquivo único `AGENT_ORCHESTRATOR.md` consome apenas ~2.500 tokens (7.6% do menor contexto local de 32k), permitindo execução fluida mesmo em hardware modesto local, enquanto a suíte completa de ~15.000 tokens ocupa menos de 8% em modelos comerciais modernos.
