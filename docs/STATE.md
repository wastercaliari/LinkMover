# STATE (Quadro de Execução Ativo — LinkMover)

**Goal:** Desenvolver, manter e distribuir o LinkMover Universal — aplicativo desktop multiplataforma (Windows, Linux, macOS) com interface gráfica moderna Avalonia Fluent UI e núcleo independente de movimentação segura de diretórios com links simbólicos e suporte a UAC/Developer Mode.

**Done:**
- Projeto estruturado em .NET 10 LTS e Avalonia 12.1.4 (`src/LinkMover.App`, `src/LinkMover.Core`, `tests/LinkMover.Tests`).
- Núcleo de movimentação segura com validação de reparse points, comparação SHA-256 e rollback automático.
- Build da solução no Windows aprovado com zero erros e zero avisos (`LinkMover.exe` na raiz).
- Pacotes self-contained multiplataforma gerados em `releases/` (win-x64, linux-x64, osx-x64, osx-arm64) com hashes SHA-256.
- Script de automação de pacotes `scripts/publish-all.ps1` e script de compilação `Build-LinkMover.bat`.
- Adoção oficial da suíte de governança e multi-agentes **Agent Master DevSecOps** (`git@github.com:ContalApps/agent-master-devsecops.git`).
- Configuração do Tríplice Registro (Git, Jira, Confluence) com credenciais Atlassian protegidas no `.env` e `.gitignore`.
- Migração formal da suíte de testes para xUnit com 7 testes unitários aprovados (100% de cobertura nos métodos centrais e rollback seguro).
- Provisionamento da esteira CI/CD no GitHub Actions (`.github/workflows/ci.yml`) com build Release, execução de testes e scanner SAST integrado.
- Auditoria de acessibilidade WCAG 2.1 AA na interface Avalonia: propriedades de automação assistiva (leitores de tela), tokens com contraste >= 4.5:1, badge dinâmico de status e botão de cancelamento com rollback gracioso (AGENT_UIUX).
- **Publicação Remota no GitHub:** Repositório https://github.com/wastercaliari/LinkMover com branch `main` e workflows de CI 100% verdes (Run 39384440573 aprovada em 1m36s).
- **Tríplice Registro Efetivo Ativado:**
  - Jira: [AGDEV-36](https://wastercaliari.atlassian.net/browse/AGDEV-36) (Concluído com relatório denso de 15 seções).
  - Confluence: [ADR-001](https://wastercaliari.atlassian.net/wiki/spaces/AGDEV/pages/3244034) (Arquitetura do LinkMover homologada no espaço AGDEV).
  - Backlog Priorizado: [AGDEV-37](https://wastercaliari.atlassian.net/browse/AGDEV-37) (Cadastrado em 'A fazer' para a Ideia I-2).

**Next:**
- Implementar gerenciador de links ativos e histórico de links simbólicos na UI (AGDEV-37 / Ideia I-2).
- Executar e validar pacotes Linux e macOS em hardware/sistemas-alvo reais.
- Implementação de assinatura digital Authenticode para executáveis Windows visando erradicar falsos positivos de antivírus (Ideia I-1).
- Testes automatizados adicionais de integração com caminhos longos, permissões NTFS e volumes externos.

**Blockers:** nenhum
**Failed:** nenhum
**Rules:** vide `docs/ENGINEERING_RULES.md`
**Ideas:** vide `docs/IDEAS.md`
