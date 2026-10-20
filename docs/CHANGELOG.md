# CHANGELOG (Livro-Razão Imutável de Entregas — LinkMover)

> **Regra Operacional:** Este arquivo é ESTRITAMENTE APPEND-ONLY. Nenhuma linha anterior pode ser editada ou deletada.
> **Formato Obrigatório:** `AAAA-MM-DD [TIPO] @onde o que | why: justificativa (TICKET)`

2026-10-20 [FEAT] @root Criação inicial do LinkMover com .NET 10 LTS e interface Avalonia Fluent UI | why: migração do legado para stack moderna e multiplataforma (LM-001)
2026-10-20 [FEAT] @core Implementação do mecanismo transacional de movimentação com hash SHA-256 e rollback | why: garantir integridade total dos dados sem perda em caso de falha (LM-002)
2026-10-20 [RELEASE] @releases Geração de binários Windows x64 e pacotes cross-build linux-x64, osx-x64, osx-arm64 | why: disponibilizar pacotes portáveis para teste (LM-003)
2026-10-20 [SECURITY] @root Adoção da suíte Agent Master DevSecOps e configuração do Tríplice Registro | why: estabelecer governança autônoma, cultura de testes e segurança shift-left (AGDEV-17)
2026-10-20 [TEST] @tests Migração dos testes de LinkMover.Core para framework formal xUnit com asserções de rollback | why: estabelecer cultura formal de testes automatizados e validação contínua (LM-002)
2026-10-20 [CI] @ci Provisionamento de pipeline GitHub Actions com build Release, xUnit e scanner SAST | why: automação da esteira de CI/CD e bloqueio preventivo de regressões (LM-002)
