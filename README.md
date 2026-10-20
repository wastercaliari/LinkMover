# LinkMover Universal

> **Governança & Agente Padrão:** Este projeto adota oficialmente o [**Agent Master DevSecOps**](AGENT_README.md) (`git@github.com:ContalApps/agent-master-devsecops.git`) como suíte autônoma padrão de orquestração, segurança, QA e conformidade enterprise. Instruções do agente ativas em [`AGENTS.md`](AGENTS.md) e regras em [`docs/ENGINEERING_RULES.md`](docs/ENGINEERING_RULES.md).

Aplicativo desktop para Windows, Linux e macOS, com interface Avalonia Fluent e núcleo independente de movimentação segura.

## Entregáveis
- LinkMover.exe e DLLs nativas de suporte na raiz (Windows x64).
- Pacotes por arquitetura em releases.
- LinkMover-source.zip com código e instruções.
- releases/SHA256SUMS.txt com hashes SHA-256 dos pacotes.
- Projeto .NET 10 LTS e Avalonia 12.1.4.

## Estado de validação
- Build da solução no Windows: aprovado, zero avisos e zero erros.
- Inicialização da GUI Windows: aprovada.
- Teste de inventário e rollback: aprovado.
- Verificação de dependências vulneráveis do NuGet: nenhuma vulnerabilidade conhecida nos pacotes atuais.
- Os pacotes Linux/macOS foram publicados por cross-build no Windows, mas ainda precisam ser executados e testados em hardware/sistemas-alvo antes de serem considerados releases validados.
- O executável Windows atual não está assinado (Authenticode: NotSigned). Não existe promessa técnica de zero falsos positivos.

## Compilar
Neste workspace Windows, execute Build-LinkMover.bat ou use:
- .dotnet/dotnet.exe restore LinkMover.slnx
- .dotnet/dotnet.exe build LinkMover.slnx -c Release
- .dotnet/dotnet.exe run --project src/LinkMover.App/LinkMover.App.csproj

Em um novo ambiente, instale o .NET 10 SDK e use o comando dotnet normalmente. Os comandos acima não incluem a pasta .dotnet no pacote-fonte.

## Gerar pacotes
Execute PowerShell:
scripts/publish-all.ps1

O script gera os pacotes self-contained para win-x64, linux-x64, osx-x64 e osx-arm64 e atualiza os hashes. Cross-build não substitui teste no sistema-alvo.

## Segurança da operação
1. Exige que a origem e a pasta pai de destino existam.
2. Bloqueia origem que seja link/reparse point, links/junções internos, destino existente e caminhos sobrepostos.
3. Copia os arquivos e compara caminhos, quantidade, tamanho e hashes SHA-256.
4. Só então renomeia a origem para backup, cria o link simbólico e valida o destino resolvido.
5. Remove o backup somente após validar o link; em falha, tenta restaurar a origem e relata se algo precisou ser preservado.

Feche os aplicativos que usam a pasta, mantenha backup e teste primeiro com pastas descartáveis. A cópia não preserva necessariamente ACLs/permissões avançadas ou metadados específicos de cada plataforma; valide aplicativos críticos antes de usar em produção.

No Windows, se Developer Mode não estiver habilitado, o app pede confirmação antes de reiniciar via UAC para criar o link. Em Linux/macOS, permissões e política do filesystem podem impedir a operação.

## Redução de falsos positivos
- Windows: assinar cada release com certificado Authenticode válido, aplicar timestamp e manter identidade de editor consistente.
- macOS: assinar com Developer ID e notarizar pela Apple.
- Linux: usar pacotes adequados, checksums e metadados de release assinados.
- Manter dependências fixadas, builds rastreáveis, código transparente, sem packers/ofuscação e sem execução de código remoto.
- Enviar falsos positivos ao fornecedor do antivírus para revisão. Não contornar mecanismos de detecção.

Assinatura Windows e notarização Apple dependem de certificados/contas que não foram fornecidos nesta execução.
