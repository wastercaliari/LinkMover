LinkMover Universal

Windows: execute LinkMover.exe or Executar-LinkMover.bat.
Linux/macOS: extraia o pacote correspondente na pasta releases e execute o binario LinkMover.App (pode ser necessario marcar como executavel).

O LinkMover copia a pasta selecionada, valida os arquivos por SHA-256, substitui o caminho original por link simbolico e tenta reverter automaticamente se a operacao falhar. Links e juncoes internos sao bloqueados por seguranca.

No Windows, se o Developer Mode nao estiver habilitado, o aplicativo perguntara se pode reiniciar com permissao administrativa via UAC para criar o link. No Linux/macOS, permissao e politica do filesystem podem limitar a criacao de links.

IMPORTANTE: feche os aplicativos que usam a pasta, mantenha backup e teste primeiro com uma pasta descartavel. Os pacotes Linux/macOS foram compilados neste ambiente Windows, mas ainda precisam ser executados e testados nos sistemas-alvo. O projeto nao pode garantir zero falsos positivos em antivirus; as versoes publicas devem ser assinadas/notarizadas conforme README.md.
