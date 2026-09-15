    export enum Parque {
        PNSO = "Parque Nacional da Serra dos Órgãos",
        PNMMT = "Parque Natural Municipal Montanhas de Teresópolis",
        PETP = "Parque Estadual dos Três Picos"
    }

    export enum TipoPostagem {
        Trilhas = "TRILHAS",
        Cachoeiras = "CACHOEIRAS",
        Biodiversidade = "BIODIVERSIDADE",
        Eventos = "EVENTOS",
        Novidades = "NOVIDADES",
        Atracoes = "ATRAÇÕES",
        Sobre = "SOBRE"
    }

    // Para preencher combobox no front (React/HTML/TS)
    export const ListaParques = Object.values(Parque);
    export const ListaTipos = Object.values(TipoPostagem);


    // Retorna o texto do parque ou "—" se for inválido
    export function getParque(p: string): string {
    return Object.values(Parque).includes(p as Parque) ? p : "—";
    }

    // Retorna o texto do tipo de postagem ou "—" se for inválido
    export function getTipo(t: string): string {
    return Object.values(TipoPostagem).includes(t as TipoPostagem) ? t : "—";
    }
