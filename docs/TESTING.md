# Plano de testes

Use somente pastas descartáveis até concluir a validação de cada sistema:
- pastas aninhadas, arquivos vazios, nomes Unicode e arquivos grandes;
- link simbólico/junção dentro da origem (deve bloquear antes da cópia);
- destino existente (deve bloquear sem modificar origem/destino);
- falha na criação do link (deve restaurar a origem);
- alteração de origem durante a cópia (SHA-256 deve falhar com segurança);
- confirmação de que o link resolve exatamente para o destino;
- falta de permissão e falta de espaço em disco.

Execute no Windows, Linux e macOS. No Windows, a criação de links pode exigir Developer Mode ou elevação. Nunca aponte testes destrutivos para pastas de aplicações reais.
