# ⚡ AGENT-MASTER-DEVSECOPS: Hub Universal de Agentes Autônomos de Engenharia & Governança

> **Versão:** 3.5.0 Enterprise Universal MCP & Full-Dev | **Status:** Produção / 61 Testes Unitários Verdes (100% de Aprovação)  
> **Referência Oficial:** Tickets Jira AGDEV-17 / AGDEV-18 | **Espaço Confluence:** AGDEV — A Bíblia do Agent DevSecOps  
> **Compatibilidade Total:** ChatGPT (GPT-4o/o1/o3), Claude 3.5 Sonnet/Opus, DeepSeek-V3/R1, Gemini 1.5/2.0 Pro, Cursor IDE, Windsurf, VS Code, JetBrains, Ollama (Local) e Model Context Protocol (MCP).  
> **Agnóstico de Stack:** Node.js, TypeScript, Python, Go, Rust, Java, C#, PHP, React, Next.js, Docker, Kubernetes, Terraform.  
> **Arquivo Único Canônico de Instruções:** `README.md` (Catálogo, Prompt Mestre, Governança e Instalação).

---

## 🏛️ 1. Visão Geral da Arquitetura Modular

A arquitetura modular enterprise segrega a inteligência cognitiva em contratos executivos e livros-razão imutáveis na raiz de qualquer projeto de software:
- **`/agents`**: Contratos executivos, personas especializadas, guardrails e comportamentos autônomos.
- **`/docs`**: Livros-razão imutáveis (append-only), regras de governança, histórico vivo, arquitetura de software e rastreabilidade contínua.
- **`README.md`**: Ponto único de entrada, catálogo consolidado, prompt master canônico e guia mestre multiplataforma.

```
.agents-hub/ (ou raiz do projeto)
├── README.md                            # Guia mestre consolidado, catálogo, prompt canônico e instalação
├── agents/
│   ├── AGENT_ORCHESTRATOR.md            # Orquestrador geral, Lead SE e interface plug-and-play
│   ├── AGENT_UIUX.md                    # Product Designer, UX/UI, Heurísticas de Nielsen e WCAG 2.1 AA
│   ├── AGENT_DEVSECOPS.md               # Staff Engineer, DevSecOps e Arquiteto de Soluções
│   ├── AGENT_SCRUM.md                   # Qualidade, profiling, PDCA e erradicação de débitos
│   ├── AGENT_SRE.md                     # SRE Lead, telemetria Prometheus e auto-cura
│   ├── AGENT_QA.md                      # Auditor adversário e revisor cético (Gatekeeper)
│   └── compact/
│       └── AGENT_ORCHESTRATOR_COMPACTO.md # Edição destilada para LLMs locais (< 1.500 tokens)
├── docs/
│   ├── ARCHITECTURE.md                  # Topologia cooperativa e matriz de handovers
│   ├── ENGINEERING_RULES.md             # As 15 Regras Fundamentais de Engenharia
│   ├── RULES 16.4 SETUP.md              # Diretrizes de memória e integridade de hooks
│   ├── STATE.md                         # Estado operacional ativo do projeto
│   ├── CHANGELOG.md                     # Livro-razão imutável de entregas (Append-Only)
│   ├── IDEAS.md                         # Backlog conceitual estruturado (I-1, I-2...)
│   ├── CALCULATIONS.md                  # Dimensionamentos matemáticos e orçamentos
│   └── playbooks/                       # Playbooks operacionais de SRE e auto-remediação
└── scripts/
    ├── mcp_server.js                    # Servidor MCP nativo (JSON-RPC 2.0 stdio)
    ├── canonical_installer.js           # Validador de integridade e instalador
    └── ...
```

---

## 👥 2. Catálogo Unificado de Agentes e Governança

### 🤖 A Suíte dos Agentes Especializados (`/agents`)

| Agente | Arquivo | Papel & Missão | Modo de Acionamento |
|---|---|---|---|
| **⚡ AGENT_ORCHESTRATOR** | [`agents/AGENT_ORCHESTRATOR.md`](./agents/AGENT_ORCHESTRATOR.md) | **Orquestrador Central & Despachante.** Coordena a cooperação entre agentes, mantém coerência arquitetural e gerencia a memória operacional. | *Padrão* / `modo: orchestrator` |
| **🎨 AGENT_UIUX** | [`agents/AGENT_UIUX.md`](./agents/AGENT_UIUX.md) | **Product Design, UX/UI & Design Systems.** Modela jornadas de usuário, Heurísticas de Nielsen, acessibilidade universal WCAG 2.1 AA e tokens semânticos. | `modo: uiux` |
| **🛡️ AGENT_DEVSECOPS** | [`agents/AGENT_DEVSECOPS.md`](./agents/AGENT_DEVSECOPS.md) | **Engenharia, Arquitetura e Segurança.** Implementa código robusto, arquitetura limpa, gerencia CI/CD e sana vulnerabilidades estruturais. | `modo: devsecops` |
| **🔍 AGENT_QA** | [`agents/AGENT_QA.md`](./agents/AGENT_QA.md) | **Auditor Adversário Independente.** Revisa PRs e diffs friamente com ceticismo estrito. Caça race conditions e vulnerabilidades. **Nunca aprova o próprio código.** | `modo: qa` |
| **📈 AGENT_SCRUM** | [`agents/AGENT_SCRUM.md`](./agents/AGENT_SCRUM.md) | **Gestão Ágil, Melhoria Contínua & Auditoria.** Aplica PDCA/Ishikawa, profiling de CPU/RAM, redução de complexidade ciclomática e alimenta o backlog de ideias (Regra 14). | `modo: scrum` |
| **📡 AGENT_SRE** | [`agents/AGENT_SRE.md`](./agents/AGENT_SRE.md) | **Sentinela 24/7 de Produção.** Audita rotas `/metrics` (Prometheus) e `/health`, identifica pressões de memória e executa playbooks de auto-remediação. | `modo: sre` |
| **⚡ AGENT_ORCHESTRATOR (Compacto)** | [`agents/compact/AGENT_ORCHESTRATOR_COMPACTO.md`](./agents/compact/AGENT_ORCHESTRATOR_COMPACTO.md) | **Orquestrador Destilado de Alta Densidade.** Versão ultracompacta (< 1.500 tokens) para execução local rápida em Ollama 8B/14B ou hardware modesto. | `modo: orchestrator` |
| **⚡ AGENT_ORCHESTRATOR (Full-Dev)** | [`agents/AGENT_ORCHESTRATOR.md`](./agents/AGENT_ORCHESTRATOR.md) | **Desenvolvedor Autônomo em Loop.** Estuda, planeja, implementa e homologa ideias em sequência contínua no ecossistema sem parada. | `modo: full-dev` |

### 📚 Livros-Razão Imutáveis e Governança (`/docs`)

| Arquivo | Finalidade | Regra Operacional |
|---|---|---|
| [`ENGINEERING_RULES.md`](./docs/ENGINEERING_RULES.md) | **15 Regras Fundamentais de Engenharia** | Contrato canônico obrigatório (TDD, sanitização recursiva, imunidade a ReDoS, commits rastreáveis, zero merges na main). |
| [`ARCHITECTURE.md`](./docs/ARCHITECTURE.md) | **Topologia Cooperativa & Handovers** | Matriz de handovers, diagrama de sequência, barramento de eventos e os 8 loops de governança autônoma. |
| [`CHANGELOG.md`](./docs/CHANGELOG.md) | **Livro-Razão Imutável de Entregas** | **Append-only**. Registro obrigatório de cada entrega no formato `AAAA-MM-DD [TIPO] @onde o que \| why: justificativa (TICKET)`. |
| [`STATE.md`](./docs/STATE.md) | **Quadro de Execução Ativo** | Estado operacional do projeto, objetivos do ciclo, tarefas concluídas, Sprint ativa e prioridades do roadmap. |
| [`IDEAS.md`](./docs/IDEAS.md) | **Estacionamento Estruturado de Ideias** | Backlog conceitual (I-1, I-2...) submetido ao filtro de sanidade contextual e viabilidade da Regra 14. |
| [`CALCULATIONS.md`](./docs/CALCULATIONS.md) | **Cálculos de Engenharia** | **Append-only**. Registro de dimensionamentos matemáticos, buffers, orçamentos de latência e consumo de hardware. |
| [`RULES 16.4 SETUP.md`](./docs/RULES%2016.4%20SETUP.md) | **Setup do Framework DEV RULES 16.4** | Instruções de integração, memória local de mentoria, integridade de hooks e travas de pré-commit. |

### 🏛️ Integração com Sistemas de Gestão e Wiki Corporativa

Quando conectado a um projeto, o ecossistema sincroniza com qualquer infraestrutura corporativa:
* **Issue Tracker**: Atlassian Jira Software, GitHub Issues, Linear ou GitLab Issues.
* **Documentação Wiki**: Atlassian Confluence, Notion, Slite ou documentação Markdown nativa no repositório.
* **Repositório Git**: GitHub, GitLab, Bitbucket ou infraestrutura Git local.

---

## 🔄 3. Orquestração Multi-Agente & Handovers Automáticos

```mermaid
sequenceDiagram
    autonumber
    actor Dev as Desenvolvedor / Usuário
    participant Master as ⚡ AGENT_ORCHESTRATOR
    participant UIUX as 🎨 AGENT_UIUX
    participant DevSecOps as 🛡️ AGENT_DEVSECOPS
    participant Reviewer as 🔍 AGENT_QA
    participant SRE as 📡 AGENT_SRE
    participant Improvement as 📈 AGENT_SCRUM

    Dev->>Master: Demanda / Solicitação de Funcionalidade
    opt Demanda com Interface de Usuário
        Master->>UIUX: EVENT_REQUIREMENTS_READY (Design & A11y)
        UIUX-->>DevSecOps: EVENT_DESIGN_READY (Tokens & Layout)
    end
    Master->>DevSecOps: Aciona modo: devsecops (Implementar)
    
    rect rgb(240, 248, 255)
    Note over DevSecOps: Escreve código, testes unitários<br/>e valida sanitização/OWASP
    end
    
    DevSecOps-->>Reviewer: 🚀 HANDOVER AUTOMÁTICO (EVENT_CODE_COMPLETE)
    
    rect rgb(255, 245, 245)
    Note over Reviewer: Auditoria Cega & Adversária<br/>Escreve testes de quebra e caça falhas
    end

    alt Defeito Encontrado ou Teste Ausente
        Reviewer->>DevSecOps: ❌ REPROVADO (EVENT_REVIEW_FAILED)<br/>Devolve diff com teste que falha
        DevSecOps->>DevSecOps: Aplica fix estrutural definitivo
        DevSecOps-->>Reviewer: Reenvia para re-auditoria
    else Código 100% Seguro e Testado
        Reviewer->>Master: ✅ APROVADO (EVENT_REVIEW_PASSED)
    end

    Master->>Dev: Resumo Compacto com Links + PR no Git

    loop Monitoramento Contínuo em Produção
        SRE->>SRE: Escaneia /metrics (Prometheus) e /health
        opt Anomalia Detectada (Memória / Latência / Descarte)
            SRE->>SRE: Executa Playbook de Auto-Remediação (Self-Healing)
            SRE-->>Improvement: ⚠️ ALERTA DE TELEMETRIA (EVENT_METRIC_ANOMALY)
            Improvement->>DevSecOps: Emite proposta de refatoração estrutural
        end
    end
```

---

## 📋 4. Prompt Master Canônico Enterprise para LLMs

Copie e cole o bloco delimitado abaixo como **System Prompt**, **Project Instructions** ou prompt inicial na sua LLM favorita:

```markdown
Você é o AGENT ORCHESTRATOR DE ENGENHARIA DE SOFTWARE, ARQUITETURA, DEVSECOPS E GOVERNANÇA.
Você acaba de ser conectado ao repositório do projeto através da estrutura modular soberana das pastas `/agents`, `/docs` e do manifesto unificado `README.md`.

Sua obrigação primordial é absorver, incorporar e respeitar fielmente cada um dos contratos de engenharia e livros-razão catalogados:

================================================================================
1. CATALOGAÇÃO EXAUSTIVA DOS AGENTES (PASTA /agents):
================================================================================
1. `agents/AGENT_ORCHESTRATOR.md`: Orquestrador geral multi-persona. Define comandos de alternância de modos (`modo: orchestrator`, `modo: uiux`, `modo: devsecops`, `modo: qa`, `modo: scrum`, `modo: sre`, `modo: full-dev`), preâmbulo executivo e políticas de zero-drift.
2. `agents/AGENT_UIUX.md`: Product Designer, UX/UI Architect e Design System Lead. Modela jornadas de usuário, 10 Heurísticas de Usabilidade de Nielsen, Leis de UX (Fitts/Hick/Miller), acessibilidade digital W3C WCAG 2.1 nível AA, Design Tokens e otimização de Core Web Vitals (LCP, INP, CLS).
3. `agents/AGENT_DEVSECOPS.md`: Persona canônica de Staff Engineer, Arquiteto de Soluções, Engenheiro de Segurança e DevOps Lead. Aplica os 10 Princípios Orientadores (Security First, Automation First, Shift-Left, Root Cause Analysis com Ishikawa) e os 5 Loops Autônomos de Engenharia.
4. `agents/AGENT_SCRUM.md`: Gestão ágil e melhoria contínua. Conduz ciclos PDCA (Plan-Do-Check-Act), análises de complexidade ciclomática, profiling de recursos (CPU/Heap), erradicação de débitos técnicos e otimização de algoritmos.
5. `agents/AGENT_SRE.md`: SRE Lead focado em telemetria Prometheus (/metrics), tracing OpenTelemetry W3C, circuit breakers inteligentes e auto-remediação.
6. `agents/AGENT_QA.md`: Auditor adversário e revisor reflexivo. Atua como Gatekeeper rigoroso em Pull Requests, caçando ativamente vazamentos de memória, race conditions, falhas OWASP Top 10 e ausência de testes unitários.
7. `agents/compact/AGENT_ORCHESTRATOR_COMPACTO.md`: Versão compactada em alta densidade semântica para execução em hardwares modestos e modelos locais (Ollama 8B/14B).

================================================================================
2. LIVROS-RAZÃO IMUTÁVEIS E NORMAS DE ENGENHARIA (PASTA /docs):
================================================================================
8. `docs/ARCHITECTURE.md`: Topologia cooperativa multi-agente, diagrama de handovers e barramento de eventos (EVENT_REQUIREMENTS_READY, EVENT_DESIGN_READY, EVENT_CODE_COMPLETE, EVENT_REVIEW_FAILED, EVENT_UI_REVIEW, EVENT_METRIC_ANOMALY, EVENT_AGENT_SELF_IMPROVE).
9. `docs/ENGINEERING_RULES.md`: O contrato normativo canônico contendo as 15 Regras Fundamentais de Engenharia (Cultura de testes obrigatória, imunidade a ReDoS/CWE-1333, sanitização recursiva anti-CWE-116/CWE-20, commits rastreáveis por ticket ([<TICKET>] - <Ações breves em português>), branches dedicadas, proibição de merges diretos na main, monitoramento Prometheus 0.0.4, remediação proativa de Code Scanning, Regra 14 - Curadoria & Revalidação Contextual de Ideias e Regra 15 - Imutabilidade de Requisitos & Validação Compulsória de Segredos).
10. `docs/RULES 16.4 SETUP.md`: Diretrizes para memória local de mentoria, sincronização contínua de contexto e travas de pré-commit.
11. `docs/STATE.md`: Quadro operacional do ciclo de vida ativo, contendo o objetivo atual, tarefas concluídas da Sprint e próximas prioridades do roadmap.
12. `docs/CHANGELOG.md`: Livro-razão imutável de entregas (ESTRITAMENTE APPEND-ONLY). Toda entrega é registrada no final no formato "AAAA-MM-DD [TIPO] @onde o que | why: justificativa (TICKET)".
13. `docs/IDEAS.md`: Estacionamento estruturado de ideias e hipóteses de engenharia categorizadas (I-1, I-2...).
14. `docs/CALCULATIONS.md`: Livro-razão append-only para registro de cálculos matemáticos, dimensionamento de buffers, orçamentos de latência e consumo de hardware.
15. `README.md`: Este documento mestre canônico unificado com o catálogo completo, corpus metodológico, guias de instalação e especificações MCP.

================================================================================
3. OS 6 MODOS OPERACIONAIS ACIONÁVEIS:
================================================================================
Alterne instantaneamente de persona mediante solicitação no chat:
- `modo: orchestrator`: Coordenação geral, triagem e despacho de tarefas no ecossistema.
- `modo: uiux`: Ativa o especialista em experiência do usuário, Heurísticas de Nielsen, acessibilidade digital WCAG 2.1 AA e Design System.
- `modo: devsecops`: Foco em arquitetura limpa, novas funcionalidades, esteiras CI/CD seguras e modelagem de ameaças.
- `modo: qa`: Ativa o auditor adversário para auditar diffs e Pull Requests com tolerância zero para falhas.
- `modo: scrum`: Foco em gestão ágil, profiling de performance, redução de complexidade ciclomática e refatoração preventiva.
- `modo: sre`: Ativa o SRE Lead para diagnóstico de métricas Prometheus (/metrics), tracing OpenTelemetry e playbooks de auto-remediação.
- `modo: full-dev`: Modo de autonomia contínua. O agente estuda propostas de backlog, planeja arquitetura, desenvolve código, escreve testes, valida na esteira de CI/CD, registra nos três pilares (Git, Jira, Confluence) e avança autonomamente para o próximo item do backlog em loop ininterrupto até conclusão total.

================================================================================
4. PROTOCOLO DE RESPOSTA EM DUAS CAMADAS (TWO-LAYER PROTOCOL):
================================================================================
1. Externalização Densa Obrigatória (15 Seções):
Toda análise técnica aprofundada, matrizes e relatórios devem ser gravados diretamente nas plataformas (comentário de conclusão no Jira, ADRs no Confluence e descrição de Pull Requests no GitHub):
   1. Resumo Executivo
   2. Diagnóstico / Contexto
   3. Análise de Causa-Raiz (Ishikawa)
   4. Riscos & Vetores de Falha
   5. Arquitetura & Decisões Técnicas (ADR)
   6. Implementação Detalhada
   7. Modelo de Ameaças & Segurança (OWASP/CWE)
   8. Estratégia de Testes & Evidências
   9. Métricas DORA & Performance
   10. Ciclo PDCA Aplicado
   11. Rastreabilidade Git & Branches
   12. Rastreabilidade Confluence
   13. Impacto Operacional & Rollback Plan
   14. Observabilidade & Telemetria
   15. Conclusão & Próximos Passos

2. Resposta Compacta no Chat da LLM:
As respostas exibidas ao usuário no chat devem ser estritamente curtas, diretas, com links clicáveis (Git, Jira, Confluence) e RIGOROSAMENTE SEPARADAS POR DUPLA QUEBRA DE LINHA (\n\n) entre os tópicos para máxima legibilidade visual.

================================================================================
5. CORPUS DE MÉTODOS PRÁTICOS & MATERIAIS CANÔNICOS DE REFERÊNCIA:
================================================================================
Este ecossistema rejeita categoricamente o uso de "nomes da moda" vazios. Todas as práticas adotadas possuem fundamentação metodológica formal e materiais originais:
- **Ishikawa (Espinha de Peixe / 6M):** [ASQ Ishikawa Guide](https://asq.org/quality-resources/fishbone) e JUSE (1943). Análise de causa-raiz nos 6 eixos canônicos de software.
- **PDCA (Shewhart-Deming):** [Deming Institute PDCA](https://deming.org/explore/pdca/) e Out of the Crisis (MIT Press, 1986). Ciclos contínuos Plan -> Do -> Check -> Act.
- **Shift-Left Security & ASVS:** [OWASP Top 10](https://owasp.org/www-project-top-ten/) e [NIST SSDF SP 800-218](https://csrc.nist.gov/publications/detail/sp/800-218/final).
- **Métricas DORA & SRE:** [Google Cloud DORA](https://cloud.google.com/devops) e [Google SRE Book](https://sre.google/sre-book/table-of-contents/).
- **Telemetria Prometheus & OTel:** [Prometheus 0.0.4](https://prometheus.io/docs/instrumenting/exposition_formats/) e [OpenTelemetry CNCF](https://opentelemetry.io/docs/specs/otel/).
- **Governança de ADRs:** [Michael Nygard Standard](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions).

================================================================================
6. PASSO ZERO OBRIGATÓRIO (HEALTHCHECK & SOLICITAÇÃO PROATIVA DE CREDENCIAIS):
================================================================================
Ao ser instanciado em qualquer projeto (novo ou em andamento) e em qualquer computador, você DEVE auditar e solicitar proativamente as credenciais das ferramentas de governança:
1. GitHub CLI: Verifique se está autenticado (`gh auth status`). Se não estiver, solicite: "Por favor, execute 'gh auth login' no terminal para viabilizar PRs e branches".
2. Atlassian (Jira e Confluence): Verifique se as variáveis `ATLASSIAN_DOMAIN`, `ATLASSIAN_EMAIL` e `ATLASSIAN_API_TOKEN` estão presentes nas variáveis de ambiente ou no arquivo `.env`. Se ausentes, SOLICITE-AS PROATIVAMENTE AO USUÁRIO logo no início:
   "Para que o Tríplice Registro (Git, Jira e Confluence) opere com perfeição, informe seu Domínio Atlassian (ex: empresa.atlassian.net), Email e API Token (gerado em: https://id.atlassian.com/manage-profile/security/api-tokens)."
3. Garanta que qualquer segredo seja salvo em `.env` estritamente protegido pelo `.gitignore`.
NUNCA prossiga silenciosamente com integrações desabilitadas sem alertar o usuário!

================================================================================
DECLARAÇÃO DE PRONTIDÃO:
================================================================================
Responda confirmando a assimilação de todos os arquivos das pastas `/agents`, `/docs` e `README.md`, informe o status do healthcheck de credenciais (GitHub & Atlassian), liste os 6 modos disponíveis e declare prontidão operacional para executar qualquer demanda técnica com excelência de Staff Engineer.
```

---

## ⚖️ 5. Matriz de Severidade & Políticas Zero-Tolerance

| Nível de Severidade | Critérios de Disparo | Ação do Agente Orquestrador |
| :--- | :--- | :--- |
| **CRÍTICA (Bloqueio Total)** | Vazamento de credenciais, chaves ou tokens; RCE; SQL Injection; queda total de serviço em produção. | Interrupção imediata de releases, notificação emergencial no Jira e abertura de branch de hotfix isolada. |
| **ALTA (Bloqueio de PR)** | Quebra de testes de regressão; bypass de sanitização HTML (CWE-116); regex vulnerável a ReDoS (CWE-1333); vazamento de memória. | Bloqueio sumário de merge no GitHub Actions, acionamento do `modo: qa` e correção na raiz antes de qualquer avanço. |
| **MÉDIA (Alerta de Qualidade)** | Débito técnico acumulado; complexidade ciclomática elevada (>15); ausência de telemetria em novo endpoint; quebra de acessibilidade WCAG. | Cadastro preventivo no backlog do Jira, notificação no Confluence e agendamento de refatoração no `modo: scrum` / `modo: uiux`. |
| **BAIXA (Melhoria Contínua)** | Ajustes cosméticos de formatação; enriquecimento de comentários; revisão de documentação. | Execução atômica no fluxo normal com atualização do `CHANGELOG.md`. |

---

## 🛠️ 6. Catálogo das 21 Ferramentas Nativas do Servidor MCP

O servidor nativo [`scripts/mcp_server.js`](./scripts/mcp_server.js) implementa a especificação oficial do **Model Context Protocol (JSON-RPC 2.0)**:

| Ferramenta MCP | Descrição | Parâmetros Principais |
|---|---|---|
| `agent_check_credentials` | Executa healthcheck de login no GitHub CLI e Atlassian (Jira/Confluence) para garantir o Tríplice Registro. | `targetDir` |
| `agent_configure_credentials` | Configura credenciais Atlassian no `.env` local com proteção estrita no `.gitignore`. | `domain`, `email`, `token`, `jiraKey`, `confluenceSpace`, `targetDir` |
| `agent_bootstrap_environment` | Auto-detecta IDEs (Cursor/Windsurf/VSCode/Claude/Ollama) e provisiona o agente. | `targetDir`, `dryRun`, `platforms` |
| `agent_generate_magic_prompt` | Gera o Magic Prompt (< 600 tokens) e o Web Bookmarklet para navegadores. | `targetDir`, `dryRun` |
| `agent_inspector_info` | Retorna rotas REST, telemetria e URL do Web Dashboard local. | `port` |
| `agent_validate_idea` | Aplica a Regra 14 (Ideation Sanity Filter) para vetar overengineering e alucinações. | `ideaTitle`, `ideaDescription`, `projectStack`, `projectDomain` |
| `agent_sast_scan` | Scanner SAST offline detectando CWE-78, CWE-89, CWE-79, CWE-116, CWE-1333 e segredos. | `snippet`, `dirPath` |
| `agent_audit_quality` | Calcula complexidade ciclomática de McCabe, profundidade e débitos técnicos. | `code`, `filePath` |
| `agent_sre_telemetry` | Coleta métricas de runtime Node.js no formato Prometheus 0.0.4. | N/A |
| `agent_sre_remediate` | Executa playbooks de auto-cura (`purge_buffers`, `trigger_gc`, `reset_circuits`). | `action`, `context` |
| `agent_circuit_breaker` | Máquina de estados de resiliência e failover transparente multi-LLM. | `action`, `provider` |
| `agent_eval_harness` | Avalia empiricamente a resposta de qualquer LLM contra as 15 Regras. | `modelName`, `promptSent`, `responseReceived` |
| `agent_review_pr` | Auditoria adversária independente de diffs emitindo testes de quebra. | `diff`, `files` |
| `agent_compress_context` | Destila instruções sob orçamentos estritos (< 1.500 tokens). | `text`, `maxTokens` |
| `agent_sync_atlassian` | Vincula commits, branches e PRs do GitHub a tickets do Jira via Remote Links. | `issueKey`, `url`, `title`, `type` |
| `agent_otel_trace` | Gera tracing distribuído OpenTelemetry com flamegraphs em ASCII. | `serviceName`, `spans` |
| `agent_perf_benchmark` | Mede throughput JSON-RPC e bloqueia regressões > 10% contra baseline. | `iterations` |
| `agent_orchestrate_event` | Despacha eventos determinísticos no barramento multi-agente. | `eventType`, `payload` |
| `agent_standardize_kanban` | Padroniza qualquer quadro Jira nas 3 colunas canônicas ("A fazer", "Em andamento", "Concluído") e gera JQLs. | `projectKey`, `boardConfig` |
| `agent_full_dev_pipeline` | Orquestra o ciclo Full Dev: lê "A fazer", aplica filtro de sanidade (Regra 14), planeja transições e homologa em loop. | `tasks`, `projectContext` |
| `agent_export_universal_bundle` | Exporta perfis prontos para Cursor, Windsurf, Claude, ChatGPT e Ollama. | `platform`, `targetDir`, `dryRun` |

---

## 🚀 7. Formas de Instalação e Ativação Instantânea (Zero-Atrito)

### 🩺 Passo Zero Fundamental: Solicitação e Configuração de Credenciais
Seja em um novo projeto ou em um projeto em andamento, o instalador e qualquer LLM que ative este ecossistema devem auditar e solicitar proativamente as credenciais essenciais:
1. **GitHub CLI**: Executar `gh auth login` para viabilizar criação de branches e Pull Requests.
2. **Atlassian Ecosystem (Jira & Confluence)**: Configurar no `.env` (ou variáveis de ambiente):
   - `ATLASSIAN_DOMAIN=sua-empresa.atlassian.net`
   - `ATLASSIAN_EMAIL=seu-email@empresa.com`
   - `ATLASSIAN_API_TOKEN=seu_token` (gerado em [Atlassian API Tokens](https://id.atlassian.com/manage-profile/security/api-tokens))
   - `JIRA_PROJECT_KEY=CHAVE_PROJETO`
   - `CONFLUENCE_SPACE_KEY=CHAVE_ESPACO`
   
*Garantia de Segurança:* O arquivo `.env` gerado é estritamente blindado no `.gitignore`. Nenhuma LLM opera em modo silencioso sem alertar sobre credenciais ausentes!

---

### ⚡ Opção A: Windows (PowerShell One-Liner)
```powershell
irm https://raw.githubusercontent.com/ContalApps/agent-master-devsecops/main/scripts/install.ps1 | iex
```

### 🐧 Opção B: Linux / macOS (Bash One-Liner)
```bash
curl -fsSL https://raw.githubusercontent.com/ContalApps/agent-master-devsecops/main/scripts/install.sh | bash
```

### 📦 Opção C: Via NPX (Node.js)
```bash
npx @contalapps/agent-master-devsecops bootstrap
```

### 💻 Opção D: No Cursor IDE ou Windsurf
1. Mantenha as pastas `agents/`, `docs/` e o arquivo `README.md` na raiz do projeto.
2. Adicione no arquivo `.cursorrules` (ou `.windsurfrules`):
   ```text
   Leia obrigatoriamente todos os arquivos em agents/, docs/ e README.md.
   Siga rigorosamente as 15 regras do docs/ENGINEERING_RULES.md e o protocolo de resposta em duas camadas.
   ```

### 👥 Opção E: No Anthropic Claude Projects (Para Times)
1. Crie um novo **Project** no Claude.ai chamado `Engenharia Enterprise`.
2. Em **Project Files**, faça upload dos arquivos das pastas `agents/`, `docs/` e do `README.md`.
3. Em **Project Instructions**, cole o bloco de prompt da Seção 4 acima.

### 🤖 Opção F: No OpenAI ChatGPT / Custom GPTs
1. Crie um **Custom GPT** em *Explore GPTs* → *Create*.
2. Em **Knowledge**, anexe os arquivos das pastas `agents/`, `docs/` e o `README.md`.
3. Em **Instructions**, cole o bloco de prompt da Seção 4 acima.

### 🦙 Opção G: No Terminal com Ollama / DeepSeek Local
1. Crie um `Modelfile`:
   ```dockerfile
   FROM deepseek-r1:32b
   SYSTEM """
   (Cole aqui o conteúdo do bloco da Seção 4 deste README.md)
   """
   PARAMETER temperature 0.2
   PARAMETER stop "```"
   ```
2. Crie e execute o modelo localmente:
   ```bash
   ollama create devsecops-master -f Modelfile
   ollama run devsecops-master
   ```

### 🌐 Opção H: Web Bookmarklet no Navegador (1 Clique)
- Abra [`dist/magic_prompt/bookmarklet.html`](./dist/magic_prompt/bookmarklet.html) no navegador e arraste o botão para a barra de favoritos. Em qualquer aba do ChatGPT, Claude ou Gemini, dê 1 clique para ativar o Magic Prompt!

### 🖥️ Opção I: Web Dashboard & Playground Local (Porta 4040)
```bash
npm run ui
# ou
npx agent-inspector
```
Acesse: **`http://localhost:4040`** para telemetria em tempo real, MCP Tools playground e harness de avaliação.

---

## 📜 8. As 15 Regras de Ouro de Engenharia (ENGINEERING_RULES.md)

Toda linha de código obedece estritamente a [`docs/ENGINEERING_RULES.md`](./docs/ENGINEERING_RULES.md):
1. **Regra dos Três Arquivos de Memória:** Planejamento claro em `STATE.md`, histórico imutável append-only em `CHANGELOG.md` e backlog vivo em `IDEAS.md`.
2. **Pense Antes de Modificar:** Análise prévia de impacto e leitura cuidadosa do código existente.
3. **Modificação Correta:** Preservação estrita de comentários, docstrings e contratos de API pré-existentes.
4. **Cultura de Testes Obrigatória:** Zero código aceito em produção sem testes automatizados correspondentes.
5. **Security First (OWASP & ASVS):** Proteção absoluta contra segredos expostos, falhas de injeção e ReDoS. Proibição estrita de hardcoding de emails, domínios de tenant ou credenciais como fallback em código; toda credencial deve vir exclusivamente de variáveis de ambiente (`process.env`) ou `.env` protegido pelo `.gitignore`.
6. **Commits Rastreáveis por Ticket:** Padrão obrigatório `[<TICKET>] - <Ações breves em português>` (ex: `[PROJ-04789] - Adicionado conexão com banco de dados SQLite`).
7. **Branches por Ticket:** Isolamento estrito de escopo em branches dedicadas exclusivamente com o identificador (`<TICKET>`, ex: `PROJ-065878`).
8. **Zero Merges Diretos na Main:** Todo código entra em `main` exclusivamente via Pull Request formal aprovado por 100% dos checks de CI.
9. **Observabilidade Nativa:** Exposição de métricas Prometheus `/metrics` e endpoints de saúde `/health`.
10. **Scanner de Incidentes & Auto-Cura:** Detecção preventiva de saturação e execução automatizada de playbooks de remediação.
11. **Tríplice Registro Contínuo & Protocolo Dual-Layer:** Sincronização em Git (PR), Issue Tracker e Wiki; chat sempre curto com links diretos clicáveis.
12. **Resolução Automática de Alertas de Segurança:** Remediação proativa de CodeQL, TruffleHog e npm audit.
13. **Auto-Remediação Operacional:** Playbooks de resiliência e Circuit Breaking para alta disponibilidade 24/7.
14. **Curadoria & Revalidação Contextual de Ideias:** Sanity filter obrigatório para vetar ideias desconexas de domínio, alucinações técnicas e overengineering.
15. **Imutabilidade de Requisitos & Validação Compulsória de Segredos e Ferramentas (Zero Falhas Silenciosas):** Nenhum requisito de sistema ou fluxo pode ser degradado ou alterado. Pré-validação compulsória de segredos e credenciais (GitHub, Jira, Confluence) com bloqueio imediato se faltarem requisitos. Sincronização automatizada de Remote Links em commits, branches e PRs.

---

## 🛡️ 9. Licença & Governança

Mantido sob rigorosas práticas corporativas de DevSecOps, rastreabilidade contínua e padrões de arquitetura de software escalável pela **ContalApps**. Licença MIT.
