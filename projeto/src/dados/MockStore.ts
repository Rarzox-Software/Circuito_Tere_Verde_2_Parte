// MockStore.ts
import { Parque, TipoPostagem } from "./Enumerations";
import petpImg from "../assets/petp_sobre.jpg";
import pnmmtImg from "../assets/pnmmt_sobre.jpg";
import pnsoImg from "../assets/pnso_sobre.jpg";
import trilhaMozartCartao from "../assets/trilha_mozart_cartao.jpg";
import pedraTartaruga from "../assets/pedra_tartaruga.jpg";
import cachoeiraFrades from "../assets/cachoeira_frades.png";

// ---------------- Tipagens ---------------- //

export interface Admin {
    id: number;
    nome: string;
    login: string;
    senha: string;
}

export interface Postagem {
    id: number;
    parque: Parque;
    tipo: TipoPostagem;
    titulo: string;
    descricao: string;
    foto: string;
    dataPublicacao: Date;
    dataInicio: Date;
    dataFim?: Date;
}


// ---------------- MockStore ---------------- //

export class MockStore {
    private static instance: MockStore;

    private administradores: Admin[] = [];
    private logado: Admin | null = null;
    private postagens: Postagem[] = [];

    private idSequencialPostagens: number = 8;
    private idSequencialAdmin: number = 2;

    private constructor() {
        // Administrador padrão
        this.administradores.push({
            id: 1,
            nome: "Administrador",
            login: "admin@gmail.com",
            senha: "123"
        });

        // dados dos parques mock

        // Parque Estadual dos Três Picos

        // sobre
        var dadosDescricao = 
            "O Parque Estadual dos Três Picos é uma das maiores e mais impressionantes unidades de conservação\n"
            + "do estado do Rio de Janeiro, abrangendo os municípios de Teresópolis, Nova Friburgo, Cachoeiras de Macacu,\n"
            + "Silva Jardim e Guapimirim. Reconhecido por suas montanhas monumentais e por sua extensa área\n"
            + "de Mata Atlântica preservada, o parque é um verdadeiro paraíso para aventureiros, amantes da natureza\n"
            + "e praticantes de esportes ao ar livre.\n"
            + "\n"
            + "Seu nome vem dos imponentes Três Picos, um conjunto de montanhas que ultrapassa 2.300 metros de altitude,\n"
            + "incluindo o Pico Menor, o Pico Médio e o Pico Maior — este considerado o ponto mais alto\n"
            + "de toda a Serra do Mar no estado. Suas paredes de rocha vertical são destino clássico para escaladores\n"
            + "de todo o Brasil, e também representam um dos cenários mais fotogênicos da região serrana.\n"
            + "\n"
            + "O parque conta com diversas trilhas que variam entre níveis leves, moderados e extremos,\n"
            + "permitindo que visitantes de todos os perfis desfrutem de suas paisagens. Entre as trilhas mais populares\n"
            + "estão a do Vale dos Deuses, a do Cabeça de Dragão, a do Capacete e a rota para o Alto dos Frades.\n"
            + "Cada uma delas revela mirantes naturais, campos de altitude, formações rochosas únicas e vistas panorâmicas\n"
            + "de tirar o fôlego.\n"
            + "\n"
            + "Além das montanhas, o parque abriga uma importante riqueza hídrica, com rios, cachoeiras e nascentes\n"
            + "de águas cristalinas cercadas pela vegetação exuberante da Mata Atlântica. A região é ideal para banho,\n"
            + "observação da fauna, fotografia, camping e momentos de total imersão no ambiente natural.\n"
            + "\n"
            + "A biodiversidade do Parque Estadual dos Três Picos é vasta, incluindo espécies raras de aves,\n"
            + "mamíferos, anfíbios e plantas endêmicas. O parque exerce papel fundamental na conservação do bioma,\n"
            + "na proteção das nascentes e no equilíbrio ecológico da Serra do Mar.\n"
            + "\n"
            + "A gestão procura integrar preservação ambiental, turismo sustentável e ações de educação ecológica,\n"
            + "garantindo uma experiência segura e responsável para os visitantes. Estruturas de apoio, sinalizações\n"
            + "e parcerias com guias credenciados contribuem para que o parque seja acessível tanto a iniciantes\n"
            + "quanto a aventureiros experientes.\n"
            + "\n"
            + "Se você busca aventura, trilhas memoráveis, vistas espetaculares e um encontro profundo com a natureza,\n"
            + "o Parque Estadual dos Três Picos é um destino indispensável na região serrana do Rio de Janeiro.\n"
            + "\n"
            + "Prepare sua mochila, ajuste seu fôlego e explore um dos cenários mais extraordinários da Mata Atlântica.\n"


        this.postagens.push({
            id: 2,
            parque: Parque.PETP,
            tipo: TipoPostagem.Sobre,
            titulo: "Parque Estadual dos Três Picos",
            descricao: dadosDescricao,
            foto: petpImg,
            dataPublicacao: new Date(),
            dataInicio: new Date()
        });

        // cachoeira
        dadosDescricao = 
          "A Cachoeira dos Frades é um dos refúgios naturais mais encantadores da região, famosa por suas águas cristalinas e ambiente tranquilo.\n"
        + "Localizada em um vale cercado por montanhas, oferece poços naturais perfeitos para um banho refrescante.\n"
        + "\n"
        + "Atividades Recomendadas:\n"
        + "- Banho nos poços de água pura e gelada.\n"
        + "- Trilhas leves pelo entorno, com belas paisagens rurais.\n"
        + "- Piquenique e momentos de descanso à beira da água.\n"
        + "- Fotografia da natureza e das quedas d'água.\n"
        + "\n"
        + "Um destino ideal para quem busca relaxamento, contato com a natureza e um passeio acessível e revigorante."

        this.postagens.push({
            id: 3,
            parque: Parque.PETP,
            tipo: TipoPostagem.Cachoeiras,
            titulo: "Cachoeira dos Frades",
            descricao: dadosDescricao,
            foto: cachoeiraFrades,
            dataPublicacao: new Date(),
            dataInicio: new Date()
        });

        // Parque Natural Municipal Montanhas de Teresópoliss

        // sobre
        dadosDescricao = 
        "O Parque Natural Municipal Montanhas de Teresópolis é um dos destinos mais encantadores da Serra dos Órgãos,\n"
        + "celebrado por sua natureza exuberante, clima agradável e impressionantes formações montanhosas.\n"
        + "Localizado no coração de Teresópolis, no estado do Rio de Janeiro, o parque é um convite ao turismo ecológico,\n"
        + "à aventura e à contemplação, atraindo desde visitantes ocasionais até montanhistas experientes.\n"
        + "\n"
        + "Com uma vasta área preservada de Mata Atlântica, o parque abriga uma biodiversidade extraordinária,\n"
        + "incluindo espécies de flora e fauna ameaçadas de extinção. Sua vegetação densa, seus mirantes naturais,\n"
        + "e suas trilhas bem definidas garantem uma experiência inesquecível para quem ama estar em contato com o ambiente natural.\n"
        + "\n"
        + "Entre seus principais atrativos estão trilhas de diferentes níveis, que levam a pontos panorâmicos\n"
        + "com vistas privilegiadas das montanhas e vales da região. Caminhadas ao amanhecer ou ao entardecer\n"
        + "revelam paisagens deslumbrantes, onde o céu e as montanhas se encontram em um espetáculo de cores.\n"
        + "\n"
        + "O parque também é perfeito para observação de aves, fotografia de natureza, piqueniques, descanso\n"
        + "e momentos de tranquilidade junto a riachos e espaços sombreados pela mata. Os visitantes podem ainda\n"
        + "explorar áreas que permitem maior aventura, aproveitando desníveis naturais, formações rochosas e trechos ideais\n"
        + "para atividades ao ar livre.\n"
        + "\n"
        + "Outro destaque do Parque Natural Municipal Montanhas de Teresópolis é seu papel na conservação ambiental.\n"
        + "Além de preservar nascentes e ecossistemas essenciais da Mata Atlântica, o parque desenvolve programas\n"
        + "de educação ambiental, monitoramento ecológico e incentivo ao turismo sustentável, garantindo que a natureza\n"
        + "seja apreciada de forma consciente e responsável.\n"
        + "\n"
        + "Com fácil acesso e infraestrutura pensada para receber turistas, o parque é uma excelente opção para famílias,\n"
        + "grupos de amigos, esportistas e viajantes em busca de aventura leve ou moderada.\n"
        + "\n"
        + "Se você deseja conhecer um dos cenários mais belos da região serrana,\n"
        + "mergulhar na tranquilidade da mata, respirar ar puro e explorar trilhas que levam a visuais surpreendentes,\n"
        + "o Parque Natural Municipal Montanhas de Teresópolis é um destino imperdível.\n"
        + "\n"
        + "Prepare sua mochila, coloque um calçado confortável e venha vivenciar uma das experiências naturais\n"
        + "mais marcantes de Teresópolis.\n"

        this.postagens.push({
            id: 4,
            parque: Parque.PNMMT,
            tipo: TipoPostagem.Sobre,
            titulo: "Parque Natural Municipal Montanhas de Teresópolis",
            descricao: dadosDescricao,
            foto: pnmmtImg,
            dataPublicacao: new Date(),
            dataInicio: new Date()
        });

        // trilhas
        dadosDescricao =  
        "Pedra da Tartaruga\n"
        + "A Pedra da Tartaruga é uma das atrações mais famosas e acessíveis da região.\n"
        + "Com formação rochosa singular, oferece um dos melhores mirantes para apreciar o pôr do sol em Teresópolis.\n"
        + "\n"
        + "Atividades Recomendadas:\n"
        + "- Trilhas leves a moderadas até o topo.\n"
        + "- Rapel e escalada esportiva, muito buscados por aventureiros.\n"
        + "- Fotografia e contemplação da vista panorâmica das montanhas.\n"
        + "\n"
        + "O local é ideal para quem deseja natureza, aventura e um visual inesquecível."

        this.postagens.push({
            id: 5,
            parque: Parque.PNMMT,
            tipo: TipoPostagem.Trilhas,
            titulo: "Pedra da Tartaruga",
            descricao: dadosDescricao,
            foto: pedraTartaruga,
            dataPublicacao: new Date(),
            dataInicio: new Date()
        });

        // Parque Nacional da Serra dos Órgãos

        // sobre
        dadosDescricao = 
        "Parque Nacional da Serra dos Órgãos\n"
        + "\n"
        + "O Parque Nacional da Serra dos Órgãos, conhecido como PARNASO, é uma das joias naturais mais icônicas do Brasil\n"
        + "e um dos parques nacionais mais antigos do país, criado em 1939. Localizado na região serrana do Rio de Janeiro,\n"
        + "abrange áreas dos municípios de Teresópolis, Petrópolis, Guapimirim e Magé, oferecendo paisagens grandiosas,\n"
        + "biodiversidade impressionante e algumas das montanhas mais famosas do território nacional.\n"
        + "\n"
        + "Entre suas atrações mais conhecidas estão formações rochosas que se tornaram símbolos do montanhismo brasileiro,\n"
        + "como o Dedo de Deus, a Agulha do Diabo, a Pedra do Sino e o Escalavrado. Essas montanhas majestosas,\n"
        + "que parecem tocar o céu, fazem do parque um dos principais destinos de escalada e trekking do país.\n"
        + "\n"
        + "O parque reúne um conjunto extraordinário de trilhas para todos os níveis, desde caminhos leves e acessíveis\n"
        + "para famílias e iniciantes, até travessias longas e desafiadoras, como a famosa Travessia Petrópolis–Teresópolis,\n"
        + "considerada por muitos a travessia mais bonita do Brasil. Durante a caminhada, os visitantes testemunham\n"
        + "cenários que variam entre campos de altitude, florestas densas de Mata Atlântica, mirantes naturais\n"
        + "e vales profundos cercados por grandes paredões de pedra.\n"
        + "\n"
        + "Além das montanhas, o PARNASO oferece uma rica rede de rios, poços e cachoeiras, especialmente na portaria\n"
        + "de Guapimirim, onde os visitantes podem desfrutar de águas cristalinas cercadas pela vegetação nativa.\n"
        + "A Biodiversidade é um dos maiores destaques do parque: aves endêmicas, mamíferos de médio e grande porte,\n"
        + "répteis, anfíbios e uma grande variedade de plantas compõem um dos ecossistemas mais preservados\n"
        + "da Mata Atlântica.\n"
        + "\n"
        + "O parque também é um importante centro de educação ambiental, pesquisa científica e conservação.\n"
        + "A gestão prioriza práticas sustentáveis, controle de visitantes, proteção de fauna e flora,\n"
        + "e manutenção das estruturas de apoio, garantindo segurança e qualidade de experiência para o público.\n"
        + "\n"
        + "Com trilhas sinalizadas, áreas de camping, refúgios de montanha, espaços para banho, centros de visitantes\n"
        + "e guias credenciados, o PARNASO oferece excelente infraestrutura para quem deseja explorar\n"
        + "as belezas naturais da região.\n"
        + "\n"
        + "Seja para uma caminhada tranquila, uma aventura intensa, um momento de contemplação,\n"
        + "ou simplesmente para respirar ar puro em meio à natureza preservada, o Parque Nacional da Serra dos Órgãos\n"
        + "é um destino imperdível na Serra Fluminense.\n"
        + "\n"
        + "Descubra mirantes espetaculares, trilhas lendárias e a energia poderosa das montanhas que fazem do PARNASO\n"
        + "um dos lugares mais inesquecíveis do Brasil.\n"

        this.postagens.push({
            id: 6,
            parque: Parque.PNSO,
            tipo: TipoPostagem.Sobre,
            titulo: "Parque Nacional da Serra dos Órgãos",
            descricao: dadosDescricao,
            foto: pnsoImg,
            dataPublicacao: new Date(),
            dataInicio: new Date()
        });

        // trilha
        dadosDescricao = 
        "A Trilha Mozart Catão é uma das rotas clássicas do Parque Nacional da Serra dos Órgãos,\n"
        + "conhecida por seus mirantes naturais e pelo visual impressionante das montanhas da Serra.\n"
        + "O percurso combina trechos de mata preservada, subidas desafiadoras e vistas panorâmicas\n"
        + "que revelam toda a beleza da região.\n"
        + "\n"
        + "Ideal para aventureiros e amantes do montanhismo, a trilha oferece uma experiência intensa\n"
        + "de contato com a natureza e momentos perfeitos para fotografia e contemplação.\n"
        + "Um passeio inesquecível pelos cenários mais marcantes da Serra dos Órgãos.\n";

        this.postagens.push({
            id: 7,
            parque: Parque.PNSO,
            tipo: TipoPostagem.Trilhas,
            titulo: "Trilha Mozart Catão",
            descricao: dadosDescricao,
            foto: trilhaMozartCartao,
            dataPublicacao: new Date(),
            dataInicio: new Date()
        });
    }

    public static getInstance(): MockStore {
        if (!MockStore.instance) {
            MockStore.instance = new MockStore();
        }
        return MockStore.instance;
    }

    // ---------------- Administradores ---------------- //

    public addAdministrador(nome: string, login: string, senha: string): Admin {
        var id = this.idSequencialAdmin;
        this.idSequencialPostagens++;

        const novoAdmin: Admin = {
            id,
            nome,
            login,
            senha,
        };

        this.administradores.push(novoAdmin);
        return novoAdmin;
    }

    public login(login: string, senha: string): boolean {
        const admin = this.administradores.find(a => a.login === login && a.senha === senha);
        if (!admin) return false;

        this.logado = admin;
        this.notify();
        return true;
    }

    public logout(): void {
        this.logado = null;
        this.notify();
    }

    public estaLogado(): boolean {
        return this.logado !== null;
    }

    public getUsuarioLogado(): Admin | null {
        return this.logado;
    }


    // ---------------- Postagens ---------------- //

    public addPostagem(data: Postagem) {
        this.idSequencialPostagens++;
        data.id = this.idSequencialPostagens;
        data.dataPublicacao = new Date();

        this.postagens.push(data);
    }

    public editarPostagem(id: number, novosDados: Partial<Postagem>): boolean {
        const index = this.postagens.findIndex(p => p.id === id);
        if (index === -1) return false;

        this.postagens[index] = { ...this.postagens[index], ...novosDados };
        return true;
    }

    public deletarPostagem(id: number): boolean {
        const tamanhoAntes = this.postagens.length;
        this.postagens = this.postagens.filter(p => p.id !== id);
        return this.postagens.length < tamanhoAntes;
    }


    public getPostagensPorParqueETipo(parqueIndex: number, tipoIndex: number): Postagem[] {
        const parqueEnum = parqueIndex === -1 ? null : Object.values(Parque)[parqueIndex];
        const tipoEnum = tipoIndex === -1 ? null : Object.values(TipoPostagem)[tipoIndex];

        return this.postagens.filter(p =>
            (parqueEnum === null || p.parque === parqueEnum) &&
            (((tipoEnum === null) && (p.tipo != TipoPostagem.Sobre)) || (p.tipo === tipoEnum))
        );
    }

    public getUltimasPostagens(): Postagem[] {
        const hoje = new Date();

        // Filtra apenas as não expiradas
        const validas = this.postagens.filter(
            p => (!p.dataFim || p.dataFim >= hoje) && p.tipo !== TipoPostagem.Sobre
        );

        // Ordena da postagem mais recente para a mais antiga
        validas.sort((a, b) => {
            return b.dataInicio.getTime() - a.dataInicio.getTime();
        });

        // Retorna apenas as 5 últimas
        return validas.slice(0, 5);
    }

    public getTodasPostagens(): Postagem[] {
        return this.postagens;
    }

    // ---------------- notificacao ---------------- //
    private listeners: (() => void)[] = [];

    public subscribe(callback: () => void) {
        this.listeners.push(callback);
    }

    private notify() {
        this.listeners.forEach(cb => cb());
    }

}

