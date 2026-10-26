# Circuito Terê Verde — Backend

## Apresentação

O Backend  foi desenvolvido para organizar e disponibilizar os dados usados pela plataforma. Ele reúne os parques e os tipos de conteúdo do projeto, permite consultar postagens e oferece recursos para que administradores façam o gerenciamento dessas publicações.

Também foram implementados o armazenamento das informações e das imagens, a autenticação de administradores e regras para controlar o acesso às operações administrativas.

## Objetivo do MVP

Construir a parte responsável por manter os conteúdos do Circuito Terê Verde e disponibilizá-los por meio de uma API. O Backend oferece uma base para que visitantes consultem informações e administradores mantenham atualizadas as publicações sobre as unidades de conservação.

## Funcionalidades desenvolvidas

- Consulta aos parques e às categorias de conteúdo cadastradas.
- Listagem pública de postagens, com filtros e paginação.
- Exibição de destaques e detalhes de postagens disponíveis ao público.
- Login, verificação de sessão e saída de administradores.
- Cadastro, consulta, edição e exclusão de postagens por administradores.
- Armazenamento das postagens e de suas imagens no banco de dados.
- Preparação automática do banco e dos dados iniciais ao iniciar a aplicação.

## Principais endpoints

Os endpoints são os caminhos usados para acessar os recursos da API. Os endpoints administrativos exigem uma sessão de administrador.

![Protótipo da página principal](./Swagger/swagger1.png)


| Método | Endpoint | Para que serve |
| --- | --- | --- |
| GET | `/api/parques` | Consulta os parques disponíveis. |
| GET | `/api/tipos-postagem` | Consulta as categorias de conteúdo. |
| GET | `/api/postagens` | Lista postagens públicas; aceita filtros e paginação. |
| GET | `/api/postagens/destaques` | Consulta postagens em destaque. |
| GET | `/api/postagens/detalhe` | Consulta uma postagem pública pelo ID. |
| POST | `/api/auth/login` | Inicia a sessão do administrador. |
| GET | `/api/auth/me` | Consulta a sessão atual. |
| POST | `/api/auth/logout` | Encerra a sessão do administrador. |
| GET | `/api/admin/postagens` | Lista postagens para administração. |
| GET | `/api/admin/postagens/selecionada` | Consulta uma postagem pelo ID para edição. |
| POST | `/api/admin/postagens` | Cadastra uma postagem. |
| PUT | `/api/admin/postagens` | Atualiza uma postagem existente. |
| DELETE | `/api/admin/postagens` | Exclui uma postagem. |

Os filtros e a paginação da listagem, assim como os IDs usados nas consultas e alterações, são enviados nos cabeçalhos da requisição. O cadastro de uma postagem recebe os dados e a imagem; a imagem deve ser JPG, PNG ou WEBP e ter até 5 MB.

## Requisitos do Backend

🔗 [Clique aqui para acessar a planilha de requisitos](https://docs.google.com/spreadsheets/d/1p7ZKCEkHgihIzKbaAFwqioU4-pCVp2KW/edit?usp=sharing&ouid=113050316799055476201&rtpof=true&sd=true)
- [Baixar Planilha](https://github.com/Rarzox-Software/Circuito_Tere_Verde_2_Parte/blob/main/requisitos/Requisitos_Backend.xlsx)

## Tecnologias utilizadas

- **ASP.NET Core 10** — criação da API e organização das rotas.
- **Entity Framework Core** — comunicação entre a aplicação e o banco de dados.
- **SQLite** — armazenamento local de parques, postagens, imagens e contas administrativas.
- **ASP.NET Identity** — autenticação e controle de acesso dos administradores.
- **Swagger** — documentação e teste dos endpoints da API.

## Estrutura

```text
Backend/
├── CircuitoTereVerde.Core/                  # Regras e estrutura central da aplicação.
│   ├── Contracts/                           # DTOs usados nos contratos da API.
│   ├── Domain/                              # Regras de negócio e exceções de domínio.
│   ├── Infra/                               # Persistência e acesso aos dados.
│   │   ├── Context/                         # Contexto do Entity Framework Core.
│   │   ├── Migrations/                      # Histórico de criação e evolução do banco.
│   │   ├── Repositories/                    # Operações de acesso às entidades.
│   │   └── UnitOfWork/                      # Coordena repositórios e salvamento.
│   ├── Models/                              # Entidades mapeadas para as tabelas do banco.
│   └── Services/                            # Orquestra as regras e operações da aplicação.
│
└── CircuitoTereVerdeWebApi/                 # Projeto que expõe a API HTTP.
    ├── Controllers/                         # Define os endpoints e recebe as requisições.
    ├── Dependencies/                        # Configura conexão, autenticação, CORS e serviços.
    ├── Handlers/                            # Trata erros globais e gera respostas da API.
    └── Services/                            # Adapta requisições da API para os serviços do Core.
```

## Como executar

O projeto pode ser executado por HTTP, Docker ou IIS Express. Para o desenvolvimento, recomendamos o **IIS Express**, pois ele já inicia a API com HTTPS.

1. Baixe o código-fonte do projeto.
2. Abra a solução `Backend/CircuitoTereVerdeWebApi.slnx` no Visual Studio.
3. Na lista de execução no topo do Visual Studio, escolha uma opção:
   - **IIS Express** — opção recomendada para desenvolver e testar com HTTPS.
   - **CTVWebApi** — executa a API diretamente pelo perfil HTTP do projeto.
   - **Docker** — executa a API em um container.
4. Execute o projeto pelo Visual Studio.

Com o IIS Express, o Swagger será aberto em:

```text
https://localhost:44390/swagger
```

Na primeira execução, a API cria o banco SQLite, aplica as migrações pendentes, cadastra os parques, os tipos de postagem e o administrador de desenvolvimento.

As credenciais do administrador de desenvolvimento estão em `appsettings.Development.json`.

## Requisitos e materiais
- [README principal do Circuito Terê Verde](../README.md)
- [Documentação do Frontend](../Frontend/README.md)
- [Apresentação do MVP](../Apresentação/Apresentacao_MVP.pptx)

## Implementações futuras

- **Backup do banco SQLite:** a cópia de segurança do banco, que também guarda as imagens das postagens, ficou como melhoria futura de prioridade baixa.

## Testes

O projeto possui testes de integração que executam a API com o SQLite de desenvolvimento. Eles verificam a criação e atualização do banco, o administrador inicial, login, sessão, permissões, bloqueio de tentativas, consultas públicas, filtros, paginação, CORS e o CRUD de postagens com imagens.

\* Os cenários de teste foram montados com apoio de IA para ampliar a cobertura dos casos de uso do MVP.
