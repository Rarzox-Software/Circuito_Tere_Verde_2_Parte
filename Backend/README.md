# Backend

API do Circuito Tere Verde em ASP.NET Core, com Identity e SQLite. As imagens ficam no banco junto das postagens.

## Preparar o administrador

Não existe cadastro público. Em desenvolvimento, o `appsettings.Development.json` já configura uma conta de teste:

- E-mail: `admin@circuitotereverde.test`
- Senha: `AdminTeste123!`

Em produção, o e-mail e a senha ficam vazios no `appsettings.Production.json`. Informe os dois por variáveis de ambiente antes de iniciar a API:

```powershell
$env:BootstrapAdmin__Email = "admin@exemplo.com"
$env:BootstrapAdmin__Password = "DefinaUmaSenhaForte1"
```

Ao iniciar a API, o banco e os perfis `USUARIO` e `ADMINISTRADOR` são preparados. A conta configurada é criada pelo Identity, que protege a senha com hash e atribui o perfil `ADMINISTRADOR`. Se ela já existir, não é duplicada e sua senha é preservada. Com as duas credenciais vazias, a API prepara o banco, mas não cria uma conta.

Em desenvolvimento, o `appsettings.Development.json` usa `Backend/circuito-tereverde-dev.db`. Em produção, o `appsettings.Production.json` usa `Backend/circuito-tereverde.db`. A conexão também pode ser substituída pela variável `ConnectionStrings__DefaultConnection`.

## Frontend e cookies

Em desenvolvimento, o CORS permite `http://localhost:5173`. Para outro endereço ou produção, configure `Cors:AllowedOrigins` com as origens exatas do frontend. Não use `*`, pois a API aceita cookies.

O cookie de autenticação é `HttpOnly`, `SameSite=Strict` e `Secure` em todos os ambientes. A API deve usar HTTPS. O frontend e a API devem estar no mesmo site para o navegador enviar o cookie. Essas opções são definidas em `AutenticacaoDependencies`, sem seção `Cookie` no appsettings.

Para chamadas do navegador, use `credentials: "include"`. Não há rota nem token CSRF. Filtros, paginação e ID da postagem são enviados nos cabeçalhos `parque`, `tipo`, `pagina`, `tamanhoPagina` e `id`, conforme a rota.

## Rotas principais

| Método | Rota | Acesso |
| --- | --- | --- |
| GET | `/api/parques` | Público |
| GET | `/api/tipos-postagem` | Público |
| GET | `/api/postagens` | Público; filtros e paginação nos cabeçalhos `parque`, `tipo`, `pagina` e `tamanhoPagina` |
| GET | `/api/postagens/destaques` | Público; até cinco postagens visíveis, sem `SOBRE` |
| GET | `/api/postagens/detalhe` | Público; ID no cabeçalho `id`; somente enquanto a postagem estiver visível |
| GET | `/api/admin/postagens` | Administrador; inclui postagens futuras e encerradas; paginação nos cabeçalhos `pagina` e `tamanhoPagina` |
| GET | `/api/admin/postagens/selecionada` | Administrador; ID no cabeçalho `id` |
| POST | `/api/admin/postagens` | Administrador; multipart/form-data |
| PUT | `/api/admin/postagens` | Administrador; multipart/form-data; ID no cabeçalho `id` |
| DELETE | `/api/admin/postagens` | Administrador; ID no cabeçalho `id` |
| POST | `/api/auth/login` | Público; cria a sessão do administrador |
| GET | `/api/auth/me` | Autenticado |
| POST | `/api/auth/logout` | Autenticado |

O DTO de postagem usa os campos `parque`, `tipo`, `titulo`, `descricao`, `dataInicio` e `dataFim`; a imagem selecionada é enviada no campo multipart `foto` ao gravar. A API valida o tipo e o tamanho (JPG, PNG ou WEBP até 5 MB) e salva os bytes no SQLite. A foto é devolvida dentro do próprio campo `foto` como data URI em Base64, que o React usa diretamente em `img src`. `parque` aceita o código ou nome do parque; `tipo` usa os valores do frontend, como `TRILHAS` e `ATRAÇÕES`.

## Migrações e dados iniciais

A migração `Inicial` já está gerada em `CircuitoTereVerde.Core/Infra/Migrations`. Ela cria as tabelas e insere os três parques e os sete tipos de postagem. Cada postagem guarda `FotoPostagemId`, que aponta para uma foto com ID próprio em `FotosPostagem`. A exclusão da postagem também remove sua foto.

Os IDs inteiros são gerados automaticamente pelo SQLite. As contas e os perfis recebem IDs gerados pelo Identity. Os IDs dos parques e tipos base são informados somente na migração para inserir os catálogos fixos.

Em cada início, `DatabaseInitializer` chama `MigrateAsync` para criar um banco novo ou aplicar todas as migrações pendentes. As migrações já aplicadas não são executadas novamente. Depois, o Identity prepara os perfis e o administrador conforme o appsettings; a senha não fica na migração.

Os testes usam a API e o SQLite fixo de desenvolvimento, com o administrador configurado nesse ambiente. Eles validam login, principal, CRUD com foto, Swagger e a preparação dos dados sem duplicação. As postagens criadas pelos testes são removidas ao terminar.
