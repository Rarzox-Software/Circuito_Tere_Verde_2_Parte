# Backend — Circuito Terê Verde

API REST em ASP.NET Core 10 para catálogos, postagens e autenticação de administradores. A persistência usa Entity Framework Core e SQLite; as imagens das postagens são armazenadas no banco.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

## Executar em desenvolvimento

Na raiz do repositório, execute no PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project .\Backend\CircuitoTereVerdeWebApi\CircuitoTereVerdeWebApi.csproj --no-launch-profile --urls "https://localhost:2000"
```

O ambiente `Development` é necessário para habilitar o Swagger e carregar as configurações locais. O perfil padrão em `Properties/launchSettings.json` usa um IP fixo; o comando acima não depende desse endereço. Acesse [https://localhost:2000/swagger](https://localhost:2000/swagger). Para confiar o certificado HTTPS de desenvolvimento no Windows, se necessário, rode `dotnet dev-certs https --trust` e aceite a instalação.

Na inicialização, `DatabaseInitializer` aplica as migrações pendentes, prepara os perfis `USUARIO` e `ADMINISTRADOR` e cria o administrador configurado, se ainda não existir.

Credenciais locais definidas em `CircuitoTereVerdeWebApi/appsettings.Development.json`:

- E-mail: `admin@circuitotereverde.test`
- Senha: `AdminTeste123!`

Essas credenciais são apenas para desenvolvimento. Não as use em produção.

## Banco e configurações

- Desenvolvimento: `Backend/circuito-tereverde-dev.db`.
- Produção: `Backend/circuito-tereverde.db`.
- Migração inicial: `CircuitoTereVerde.Core/Infra/Migrations`.

Os caminhos são resolvidos a partir do diretório da API. Um banco novo é criado automaticamente na inicialização; não é necessário executar uma migração manual para o fluxo normal de desenvolvimento.

As configurações podem ser substituídas por variáveis de ambiente. Para produção, informe credenciais próprias e configure as origens exatas do Frontend:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:BootstrapAdmin__Name = "Administrador"
$env:BootstrapAdmin__Email = "admin@seu-dominio.com"
$env:BootstrapAdmin__Password = "SuaSenhaForteComNumero1"
$env:Cors__AllowedOrigins__0 = "https://seu-dominio.com"
dotnet run --project .\Backend\CircuitoTereVerdeWebApi\CircuitoTereVerdeWebApi.csproj --no-launch-profile --urls "https://localhost:2000"
```

Em produção, configure também `ConnectionStrings__DefaultConnection` se o banco precisar ficar em outro local. Use HTTPS. A API não aceita cadastro público de administradores. O cookie de autenticação é `HttpOnly`, `Secure` e `SameSite=Strict`.

Em desenvolvimento, o CORS permite `http://localhost:5173`. O Frontend atual ainda não chama a API; consulte [README do Frontend](../Frontend/README.md) para essa limitação.

## Rotas

| Método | Rota | Acesso | Observação |
| --- | --- | --- | --- |
| GET | `/api/parques` | Público | Lista parques |
| GET | `/api/tipos-postagem` | Público | Lista tipos de postagem |
| GET | `/api/postagens` | Público | Paginação e filtros via cabeçalhos `parque`, `tipo`, `pagina`, `tamanhoPagina` |
| GET | `/api/postagens/destaques` | Público | Até cinco postagens visíveis, sem tipo `SOBRE` |
| GET | `/api/postagens/detalhe` | Público | ID no cabeçalho `id`; postagem deve estar visível |
| POST | `/api/auth/login` | Público | Inicia sessão do administrador |
| GET | `/api/auth/me` | Autenticado | Retorna a sessão atual |
| POST | `/api/auth/logout` | Autenticado | Encerra a sessão |
| GET | `/api/admin/postagens` | Administrador | Inclui futuras e encerradas; paginação via `pagina`, `tamanhoPagina` |
| GET | `/api/admin/postagens/selecionada` | Administrador | ID no cabeçalho `id` |
| POST | `/api/admin/postagens` | Administrador | Cria postagem via `multipart/form-data` |
| PUT | `/api/admin/postagens` | Administrador | Atualiza postagem; ID no cabeçalho `id` |
| DELETE | `/api/admin/postagens` | Administrador | Exclui postagem; ID no cabeçalho `id` |

O formulário de postagem usa `parque`, `tipo`, `titulo`, `descricao`, `dataInicio` e `dataFim`. Na criação, a imagem é obrigatória e vai no campo multipart `foto`; na edição, ela pode ser substituída. O formato deve ser JPG, PNG ou WEBP e o tamanho máximo é 5 MB. As respostas incluem a imagem como data URI Base64 no campo `foto`.

As chamadas de navegador que usem o cookie de sessão devem enviar credenciais (`credentials: "include"`). Para filtros e IDs, use os cabeçalhos indicados na tabela.

## Testes

Na raiz do repositório:

```powershell
dotnet test .\Backend\CircuitoTereVerde.Tests\CircuitoTereVerde.Tests.csproj
```

Os testes cobrem autenticação, rotas principais, CRUD de postagens e inicialização do banco.
