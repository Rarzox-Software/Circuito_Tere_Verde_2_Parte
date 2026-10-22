<img width="150" height="150" alt="Logo Circuito TereVerde" src="./Frontend/prototipo/Logo Circuito TereVerde.png" />

# Grupo:
**Rafael Mendes,**
**Igor Augusto,** 
**Nauan Pereira,** 
**Ingrid Mendes**

# Situação Problema: 
Dificuldade em acessar informações sobre o Parque Nacional da Serra dos Órgãos, o Parque Estadual dos Três Picos e o Parque Natural Municipal Montanhas de Teresópolis, como informações sobre suas trilhas, cachoeiras e biodiversidade.

Apresentamos o  **Circuito Terê Verde**, um projeto desenvolvido para promover o ecoturismo e tornar mais simples a descoberta das belezas naturais, reunindo em uma única plataforma dados sobre trilhas, cachoeiras, atividades de ecoturismo e a conscientização ambiental na região.

## Índice
- [O Desafio](#O-Desafio)
- [MVP](#MVP)
- [Funcionalidades principais](#funcionalidades-principais)
- [Atores Envolvidos](#Atores-Envolvidos)
- [Público Alvo](#público-alvo)
- [Dores do público alvo](#dores-do-público-alvo)
- [Benefícios](#Benefícios)
- [Como executar com Docker](#como-executar-com-docker)


## O Desafio
Desenvolver uma solução digital que centralize informações sobre as unidades de conservação de Teresópolis, permitindo que turistas e visitantes planejem suas visitas de forma eficiente, consciente e segura, valorizando o ecoturismo e a preservação ambiental.

## MVP
O MVP do Circuito Terê Verde consiste em uma plataforma digital mínima e funcional que centraliza informações essenciais sobre as atrações naturais de Teresópolis, focando nas trilhas, cachoeiras e biodiversidade das unidades de conservação. Seu objetivo é orientar turistas e moradores locais de forma prática e consciente, oferecendo dados atualizados em uma interface moderna e de fácil acesso.

## Funcionalidades principais:
- *Página de Informações sobre Parques*: Apresenta dados sobre trilhas, cachoeiras, biodivercidade, eventos, novidades e atrações. Tudo em um só lugar facilitando a busca.
- *Publicação de postagens*: Administradores poderão criar e divulgar diferentes tipos de conteúdo relacionados aos parques.
- *Interface responsiva e intuitiva*: Design simples para acesso via celular e computador.

## Atores Envolvidos:
- *Visitantes*: Turistas e moradores que buscam informações sobre trilhas, biodiversidade e eventos.
- *Administradores*: Responsáveis por manter o site atualizado.

## Público-alvo: 
  - *visitantes*: Turistas e moradores interessados em ecoturismo, trilhas e atividades de montanhismo em Teresópolis.

## Dores do público alvo:
- *Informações dispersas*: Visitantes encontram dificuldade em localizar informações sobre os parques.
- *Planejamento de visitas*: Sem um calendário centralizado, turistas podem perder eventos ou horários de visitação ideais.
- *Acesso limitado a dados*: Dados sobre trilhas, fauna e flora nem sempre estão disponíveis de forma confiável.

## Benefícios:
- *Planejamento eficiente de visitas*: Turistas podem se organizar de forma prática.
- *Informações atualizadas*: Administradores podem atualizar conteúdos em tempo real.
- *Promoção do ecoturismo*: Divulgação de atrações naturais de forma sustentável.
- *Acesso remoto e conveniente*: Plataforma disponível em múltiplos dispositivos.


## Slide de Apresentação
Você pode visualizar os slides de apresentação do MVP no link abaixo:

[Slides de Apresentação](https://gtiunifeso-my.sharepoint.com/:p:/g/personal/nauanlima_unifeso_com_vc/IQBJe6kAHZOgTawivyfIlXn0AfEWFrAa0uvdk4LgmooNpIY?e=fCOpgB)

## Como executar com Docker

### Backend

Com o Docker Desktop aberto, execute:

```powershell
docker run -d --name circuito-tereverde-api -p 8080:8080 -v circuito-tereverde-dados:/data -e ASPNETCORE_ENVIRONMENT=Development rarzox/circuitotereverdewebapi:beta-teste
```

A API e o Swagger estarão disponíveis em:

```text
http://localhost:8080/swagger
```

### Frontend

O comando do site será incluído nesta seção quando a imagem do frontend estiver publicada.

Após iniciar o backend e o frontend, entre como administrador de desenvolvimento com:

```text
E-mail: admin@circuitotereverde.test
Senha: AdminTeste123!
```


## Frontend

[README do Frontend](./Frontend)

## Backend

[README do Backend](./Backend)
