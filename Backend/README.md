# Backend

API do Circuito Tere Verde em ASP.NET Core, com Identity e SQLite. As imagens ficam no banco junto das postagens.

## Preparar o administrador

Não existe cadastro público. Configure as credenciais iniciais pelo User Secrets, dentro da pasta `CircuitoTereVerdeWebApi`:

```powershell
dotnet user-secrets init
dotnet user-secrets set "BootstrapAdmin:Email" "admin@exemplo.com"
dotnet user-secrets set "BootstrapAdmin:Password" "DefinaUmaSenhaForte1"
```

Ao iniciar a API, o banco e os perfis `USUARIO` e `ADMINISTRADOR` são preparados. A conta configurada recebe o perfil `ADMINISTRADOR`; nenhuma outra conta é criada.

Em desenvolvimento, o `appsettings.Development.json` usa `Backend/circuito-tereverde-dev.db`. Em produção, o `appsettings.Production.json` usa `Backend/circuito-tereverde.db`. A conexão também pode ser substituída pela variável `ConnectionStrings__DefaultConnection`.

## Frontend e cookies

Em desenvolvimento, o CORS permite `http://localhost:5173`. Para outro endereço ou produção, configure `Cors:AllowedOrigins` com as origens exatas do frontend. Não use `*`, pois a API aceita cookies.

O cookie de autenticação é `HttpOnly`. Em produção, o cookie usa `Secure`; se frontend e API estiverem em sites diferentes, configure `Cookie:SameSite` como `None` e use HTTPS.

Para chamadas do navegador, use `credentials: "include"`. Antes de login e de operações que alteram dados, consulte `GET /api/auth/csrf` e envie o token retornado no cabeçalho `X-XSRF-TOKEN`. Depois do login, consulte esse endpoint novamente para obter um token vinculado à sessão autenticada.

## Rotas principais

| Método | Rota | Acesso |
| --- | --- | --- |
| GET | `/api/parques` | Público |
| GET | `/api/tipos-postagem` | Público |
| GET | `/api/postagens` | Público; filtros `parque`, `tipo`, `pagina`, `tamanhoPagina` |
| GET | `/api/postagens/destaques` | Público; até cinco postagens visíveis, sem `SOBRE` |
| GET | `/api/postagens/{id}` | Público; somente enquanto a postagem estiver visível |
| GET | `/api/postagens/{id}/foto` | Público enquanto a postagem estiver visível; administrador também acessa postagens encerradas ou futuras |
| GET | `/api/admin/postagens` | Administrador; inclui postagens futuras e encerradas |
| POST | `/api/admin/postagens` | Administrador; multipart/form-data |
| PUT | `/api/admin/postagens/{id}` | Administrador; multipart/form-data |
| DELETE | `/api/admin/postagens/{id}` | Administrador |
| POST | `/api/auth/login` | Público; cria a sessão do administrador |
| GET | `/api/auth/me` | Autenticado |
| POST | `/api/auth/logout` | Autenticado |

O DTO de postagem usa os campos `parque`, `tipo`, `titulo`, `descricao`, `dataInicio` e `dataFim`; a imagem é enviada no campo multipart `foto`. O frontend prepara a imagem e pode mostrar sua prévia antes do envio. A API recebe os bytes prontos, valida o tipo e o tamanho (JPG, PNG ou WEBP até 5 MB) e grava o conteúdo no SQLite sem redimensionar nem converter. O campo `foto` da resposta contém o caminho relativo para consultar a imagem. `parque` aceita o código ou nome do parque; `tipo` usa os valores do frontend, como `TRILHAS` e `ATRAÇÕES`.

O banco é criado e atualizado automaticamente pela migração `Inicial` na primeira inicialização.
