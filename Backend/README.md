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

## Tecnologias utilizadas

- **ASP.NET Core 10** — criação da API e organização das rotas.
- **Entity Framework Core** — comunicação entre a aplicação e o banco de dados.
- **SQLite** — armazenamento local de parques, postagens, imagens e contas administrativas.
- **ASP.NET Identity** — autenticação e controle de acesso dos administradores.

## Como executar

É necessário instalar o [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
.
.
.
.
.


## Requisitos e materiais

- [Requisitos do Backend](../requisitos/Requisitos_Backend.xlsx)
- [README principal do Circuito Terê Verde](../README.md)
- [Documentação do Frontend](../Frontend/README.md)
- [Apresentação do MVP](../Apresentação/Apresentacao_MVP.pptx)