# LinkMover

Aplicativo desktop multiplataforma para mover uma pasta e substituir o caminho original por um link simbólico. Interface Avalonia Fluent; núcleo de movimentação independente da interface.

## Estado real do projeto
- Código configurado para Windows, Linux e macOS via .NET 10 LTS e Avalonia.
- O ambiente de desenvolvimento disponível nesta execução é Windows. Os binários Linux/macOS precisam ser gerados e testados em seus sistemas-alvo antes de serem considerados releases prontos.
- O aplicativo não pede administrador ao iniciar. A criação de link simbólico pode exigir Developer Mode ou elevação no Windows; permissões e políticas do filesystem também se aplicam em Linux/macOS.
- Nenhum código consegue garantir zero falsos positivos em antivírus ou SmartScreen.

## Requisitos
.NET 10 SDK e acesso à internet na primeira restauração dos pacotes NuGet.

## Build e execução
Neste workspace Windows, use ` .\\.dotnet\\dotnet.exe` se o SDK local estiver presente; em outro ambiente, instale o .NET 10 SDK e use `dotnet`.

.\\.dotnet\\dotnet.exe restore src/LinkMover.App/LinkMover.App.csproj
dotnet build src/LinkMover.App/LinkMover.App.csproj -c Release
dotnet run --project src/LinkMover.App/LinkMover.App.csproj

## Publicação self-contained
Executar em cada sistema-alvo (ou gerar artefatos cruzados e depois testar no sistema-alvo):
- Windows x64: dotnet publish src/LinkMover.App/LinkMover.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o artifacts/win-x64
- Linux x64: dotnet publish src/LinkMover.App/LinkMover.App.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o artifacts/linux-x64
- macOS Apple Silicon: dotnet publish src/LinkMover.App/LinkMover.App.csproj -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -o artifacts/osx-arm64
- macOS Intel: dotnet publish src/LinkMover.App/LinkMover.App.csproj -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -o artifacts/osx-x64

## Modelo de segurança
1. Rejeita origem que já seja link/reparse point e bloqueia links/junções aninhados.
2. Bloqueia origem/destino iguais ou sobrepostos e destino já existente.
3. Calcula hashes SHA-256 da origem, copia para um destino novo e compara nomes relativos, quantidade, tamanho e hashes.
4. Renomeia a origem para backup único, cria link simbólico e verifica o destino resolvido.
5. Só remove o backup depois da validação do link. Em falha, tenta rollback; se houver incerteza, informa e preserva o backup.

Feche os processos que usam a pasta. Faça backup dos dados importantes e teste primeiro com pastas descartáveis. Arquivos alterados durante a cópia fazem a validação falhar, mas comportamento com arquivos abertos varia entre sistemas.

## Redução de falsos positivos (sem promessa de zero detecção)
- Windows: assinar cada versão com certificado Authenticode válido, timestamp, checksums SHA-256 e identidade consistente do editor. SmartScreen pode alertar em apps novos mesmo quando assinados.
- macOS: assinar com Developer ID, habilitar hardened runtime quando adequado, notarizar e staple com ferramentas/credenciais Apple.
- Linux: distribuir pacotes apropriados (.deb/.rpm/AppImage), checksums e metadados de release assinados.
- Manter builds rastreáveis, dependências fixadas, releases transparentes, sem ofuscação/packers e sem baixar/executar código remoto em runtime.
- Se houver falso positivo, enviar o binário exato ao fornecedor para análise. Não tentar contornar a detecção.

## Checklist de release
- [ ] Build e teste em Windows x64/arm64 conforme necessário.
- [ ] Build e teste em Linux x64/arm64.
- [ ] Build e teste em macOS Intel e Apple Silicon.
- [ ] Testes end-to-end em pastas temporárias em cada sistema.
- [ ] Simular falha ao criar link e verificar recuperação.
- [ ] Assinar executável Windows e aplicar timestamp.
- [ ] Assinar/notarizar pacote macOS.
- [ ] Gerar e publicar checksums SHA-256.
