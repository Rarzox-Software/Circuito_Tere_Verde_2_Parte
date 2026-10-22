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

Com o Docker Desktop aberto, crie uma rede para que os dois containers se encontrem:

```powershell
docker network create circuito-tereverde
```

### Backend

Execute a API:

```powershell
docker run -d --name circuito-tereverde-api --network circuito-tereverde -p 8080:8080 -v circuito-tereverde-dados:/data -e ASPNETCORE_ENVIRONMENT=Development rarzox/circuitotereverdewebapi:beta-teste
```

A API e o Swagger estarão disponíveis em:

```text
http://localhost:8080/swagger
```

### Frontend

Depois, execute o site:

```powershell
docker run -d --name circuito-tereverde-web --network circuito-tereverde -p 8081:80 rarzox/circuitotereverdeweb:beta-teste
```

O site estará disponível em:

```text
http://localhost:8081
```

O site encaminha internamente as chamadas para a API. Por isso, o navegador usa a mesma origem HTTP e o cookie de autenticação funciona no ambiente local.

Em uma VPS com Nginx e HTTPS, mantenha os dois containers na rede e encaminhe o domínio para o container do site. O Nginx externo deve enviar o cabeçalho `X-Forwarded-Proto: https`, que o site repassa para a API.

Após iniciar o backend e o frontend, entre como administrador de desenvolvimento com:

```text
E-mail: admin@circuitotereverde.test
Senha: AdminTeste123!
```


## Frontend

[README do Frontend](./Frontend)

## Backend

[README do Backend](./Backend)

## Créditos das imagens de desenvolvimento

As imagens das postagens iniciais são utilizadas somente no ambiente de desenvolvimento. Todas foram obtidas no Wikimedia Commons, mantendo autor e licença de cada arquivo.

- Dedo de Deus: Rafaelm, CC BY-SA 3.0. [Fonte](https://commons.wikimedia.org/wiki/File:Dedo_de_Deus_-_Serra_dos_%C3%93rg%C3%A3os.JPG)
- Bromélia na Serra dos Órgãos: Guilherme Haruo, CC BY-SA 4.0. [Fonte](https://commons.wikimedia.org/wiki/File:Brom%C3%A9lia_na_Serra_dos_%C3%93rg%C3%A3os.jpg)
- Começo da travessia Serra dos Órgãos: Beatriz de Castro Lima Varella, CC BY-SA 3.0. [Fonte](https://commons.wikimedia.org/wiki/File:Come%C3%A7o_da_travessia_Serra_dos_%C3%93rg%C3%A3os.jpg)
- Cachoeira do Parque Nacional da Serra dos Órgãos, sede Guapimirim: Ferreiraandreza, CC BY-SA 3.0. [Fonte](https://commons.wikimedia.org/wiki/File:Cachoeira_do_Parque_Nacional_da_Serra_dos_%C3%93rg%C3%A3os_Sede_Guapimirim.jpg)
- Quati no Parque Nacional da Serra dos Órgãos: Kovags, domínio público. [Fonte](https://commons.wikimedia.org/wiki/File:Coati_(Nasua_nasua)_at_Parque_Nacional_da_Serra_dos_%C3%93rg%C3%A3os,_Teres%C3%B3polis,_Brazil.jpg)
- Cachoeira Véu da Noiva: Ferreiraandreza, CC BY-SA 3.0. [Fonte](https://commons.wikimedia.org/wiki/File:Cachoeira_v%C3%A9u_da_noiva.jpg)
- Parque Estadual dos Três Picos: Rkrr, CC BY-SA 4.0. [Fonte](https://commons.wikimedia.org/wiki/File:Parque_Estadual_dos_Tres_Picos.jpg)
- Cachoeira dos Frades II: Shooterb9, CC BY-SA 3.0. [Fonte](https://commons.wikimedia.org/wiki/File:Cachoeira_dos_Frades_II.jpg)
- Sabiá-laranjeira no Parque Estadual dos Três Picos: Luiz Artur Raya Tavora, CC BY-SA 3.0. [Fonte](https://commons.wikimedia.org/wiki/File:Sabi%C3%A1_Laranjeira(Turdus_rufiventris)Parque_Estadual_dos_Tr%C3%AAs_Picos_(PETP)_Boca_do_Mato_,_Cachoeiras_de_Macacu.JPG)
- Amanhecer nos Três Picos: Carlos Perez Couto, CC BY-SA 3.0. [Fonte](https://commons.wikimedia.org/wiki/File:Amanhecer_nos_Tres_picos.jpg)
- A caminho do cume: Jeovane da Rocha, CC BY-SA 4.0. [Fonte](https://commons.wikimedia.org/wiki/File:A_caminho_do_cume!.jpg)
- Parque Natural Municipal Montanhas de Teresópolis: André Luiz Menezes da Costa, CC BY-SA 4.0. [Fonte](https://commons.wikimedia.org/wiki/File:Parque_Natural_Municipal_Montanhas_de_Teres%C3%B3polis.jpg)
- Pedra da Tartaruga: Fernandonmartins, CC BY-SA 4.0. [Fonte](https://commons.wikimedia.org/wiki/File:Pedradatartaruga.jpg)
- Mulher de Pedra: Fernandonmartins, CC BY-SA 4.0. [Fonte](https://commons.wikimedia.org/wiki/File:Mulherdepedra.jpg)
- Paisagem do Parque Natural Montanhas de Teresópolis: Tedd Santana, CC BY-SA 2.0. [Fonte](https://commons.wikimedia.org/wiki/File:Parque_Natural_Montanhas_de_Teres%C3%B3polis_(48110111748).jpg)
- Vista do Parque Natural Montanhas de Teresópolis: Tedd Santana, CC BY-SA 2.0. [Fonte](https://commons.wikimedia.org/wiki/File:Parque_Natural_Montanhas_de_Teres%C3%B3polis_(48114437796).jpg)
