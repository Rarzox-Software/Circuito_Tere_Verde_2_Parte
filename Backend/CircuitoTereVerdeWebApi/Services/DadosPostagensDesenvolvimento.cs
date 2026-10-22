using CircuitoTereVerde.Core.Domain;

namespace CircuitoTereVerdeWebApi.Services;

// Mantém os exemplos exibidos na primeira execução da API em desenvolvimento.
internal static class DadosPostagensDesenvolvimento
{
    #region Postagens iniciais

    public static IReadOnlyList<DadoPostagemDesenvolvimento> ObterPostagens()
    {
        return
        [
            new DadoPostagemDesenvolvimento(
                "dedo-de-deus.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnso, CodigosCatalogo.Tipos.Atracoes,
                "Dedo de Deus: símbolo da Serra dos Órgãos",
                "O Dedo de Deus é uma das formações rochosas mais conhecidas da Serra dos Órgãos e compõe uma das paisagens marcantes do parque.", -16),
            new DadoPostagemDesenvolvimento(
                "bromelia-parnaso.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnso, CodigosCatalogo.Tipos.Sobre,
                "Parque Nacional da Serra dos Órgãos",
                "O Parque Nacional da Serra dos Órgãos, conhecido como PARNASO, protege paisagens marcantes da Serra Fluminense, trilhas, montanhas, rios, cachoeiras e a biodiversidade da Mata Atlântica.", -15),
            new DadoPostagemDesenvolvimento(
                "travessia-serra-dos-orgaos.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnso, CodigosCatalogo.Tipos.Trilhas,
                "Trilha Mozart Catão",
                "A Trilha Mozart Catão é uma rota clássica do Parque Nacional da Serra dos Órgãos. O percurso combina trechos de mata, subidas e mirantes naturais com vista para as montanhas da Serra.", -14),
            new DadoPostagemDesenvolvimento(
                "cachoeira-parnaso.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnso, CodigosCatalogo.Tipos.Cachoeiras,
                "Cachoeira na sede Guapimirim do PARNASO",
                "As cachoeiras do Parque Nacional da Serra dos Órgãos oferecem espaços para contemplação da Mata Atlântica. A visita deve seguir as orientações da unidade de conservação e as condições do local.", -13),
            new DadoPostagemDesenvolvimento(
                "quati-parnaso.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnso, CodigosCatalogo.Tipos.Biodiversidade,
                "Quati no Parque Nacional da Serra dos Órgãos",
                "O quati é um mamífero nativo que pode ser observado em áreas preservadas da Mata Atlântica. Para a segurança do animal e dos visitantes, não ofereça alimento à fauna silvestre.", -12),
            new DadoPostagemDesenvolvimento(
                "veu-da-noiva-parnaso.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnso, CodigosCatalogo.Tipos.Cachoeiras,
                "Cachoeira Véu da Noiva",
                "A Cachoeira Véu da Noiva é uma das paisagens naturais registradas na sede Petrópolis do Parque Nacional da Serra dos Órgãos.", -11),

            new DadoPostagemDesenvolvimento(
                "parque-tres-picos.jpg", "image/jpeg", CodigosCatalogo.Parques.Petp, CodigosCatalogo.Tipos.Sobre,
                "Parque Estadual dos Três Picos",
                "O Parque Estadual dos Três Picos reúne montanhas, florestas e nascentes da Mata Atlântica na região serrana do Rio de Janeiro. Suas trilhas levam a mirantes, formações rochosas e cenários procurados por visitantes e praticantes de montanhismo.", -10),
            new DadoPostagemDesenvolvimento(
                "cachoeira-frades-ii.jpg", "image/jpeg", CodigosCatalogo.Parques.Petp, CodigosCatalogo.Tipos.Cachoeiras,
                "Cachoeira dos Frades",
                "A Cachoeira dos Frades é um refúgio natural de Teresópolis, com poços e quedas-d'água cercados por montanhas. É uma opção para banho, caminhada leve, fotografia e momentos de descanso em contato com a natureza.", -9),
            new DadoPostagemDesenvolvimento(
                "sabia-laranjeira.jpg", "image/jpeg", CodigosCatalogo.Parques.Petp, CodigosCatalogo.Tipos.Biodiversidade,
                "Sabiá-laranjeira no Parque Estadual dos Três Picos",
                "O sabiá-laranjeira é uma espécie comum em áreas de Mata Atlântica. A imagem deste exemplo foi registrada no Parque Estadual dos Três Picos, em Cachoeiras de Macacu.", -8),
            new DadoPostagemDesenvolvimento(
                "amanhecer-tres-picos.jpg", "image/jpeg", CodigosCatalogo.Parques.Petp, CodigosCatalogo.Tipos.Atracoes,
                "Amanhecer nos Três Picos",
                "Os mirantes e formações montanhosas do parque revelam diferentes paisagens ao longo do dia. Planeje a visita com antecedência e consulte as regras da unidade de conservação.", -7),
            new DadoPostagemDesenvolvimento(
                "caminho-cume-tres-picos.jpg", "image/jpeg", CodigosCatalogo.Parques.Petp, CodigosCatalogo.Tipos.Trilhas,
                "Caminho para o cume dos Três Picos",
                "As trilhas de montanha exigem preparo, equipamentos adequados e atenção às condições climáticas. Respeite a sinalização e os limites definidos pelo parque.", -6),

            new DadoPostagemDesenvolvimento(
                "pnmmt-paisagem.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnmmt, CodigosCatalogo.Tipos.Sobre,
                "Parque Natural Municipal Montanhas de Teresópolis",
                "O Parque Natural Municipal Montanhas de Teresópolis preserva áreas de Mata Atlântica e paisagens da Serra dos Órgãos. O espaço reúne trilhas, mirantes e oportunidades para observação de aves, fotografia e educação ambiental.", -5),
            new DadoPostagemDesenvolvimento(
                "pedra-tartaruga-pnmmt.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnmmt, CodigosCatalogo.Tipos.Trilhas,
                "Pedra da Tartaruga",
                "A Pedra da Tartaruga é uma atração conhecida da região, com trilha de nível leve a moderado e vista panorâmica das montanhas. O local é procurado para contemplação, fotografia e atividades de aventura com orientação adequada.", -4),
            new DadoPostagemDesenvolvimento(
                "mulher-de-pedra-pnmmt.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnmmt, CodigosCatalogo.Tipos.Atracoes,
                "Mulher de Pedra",
                "A Mulher de Pedra é uma formação rochosa que integra as paisagens serranas da região. A contemplação responsável ajuda a preservar o ambiente e a experiência de todos os visitantes.", -3),
            new DadoPostagemDesenvolvimento(
                "pnmmt-montanhas.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnmmt, CodigosCatalogo.Tipos.Novidades,
                "Paisagens das Montanhas de Teresópolis",
                "O parque protege remanescentes da Mata Atlântica e contribui para a conservação das paisagens, nascentes e espécies que compõem a região serrana de Teresópolis.", -2),
            new DadoPostagemDesenvolvimento(
                "pnmmt-vista.jpg", "image/jpeg", CodigosCatalogo.Parques.Pnmmt, CodigosCatalogo.Tipos.Biodiversidade,
                "Mata Atlântica no Parque Municipal",
                "A vegetação preservada forma abrigo para espécies da fauna e da flora da Mata Atlântica. Ao visitar, leve seus resíduos de volta e mantenha-se nas áreas permitidas.", -1)
        ];
    }

    #endregion
}

// Representa os dados necessários para criar uma postagem de exemplo e sua foto no banco.
internal sealed class DadoPostagemDesenvolvimento
{
    public string ArquivoFoto { get; }
    public string TipoConteudoFoto { get; }
    public string CodigoParque { get; }
    public string CodigoTipo { get; }
    public string Titulo { get; }
    public string Descricao { get; }
    public int DiasDesdeInicio { get; }

    public DadoPostagemDesenvolvimento(
        string arquivoFoto,
        string tipoConteudoFoto,
        string codigoParque,
        string codigoTipo,
        string titulo,
        string descricao,
        int diasDesdeInicio)
    {
        ArquivoFoto = arquivoFoto;
        TipoConteudoFoto = tipoConteudoFoto;
        CodigoParque = codigoParque;
        CodigoTipo = codigoTipo;
        Titulo = titulo;
        Descricao = descricao;
        DiasDesdeInicio = diasDesdeInicio;
    }
}
