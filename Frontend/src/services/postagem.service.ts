import { BaseCrudService } from "./base-crud.service";

// interfaces
interface PostagemDto {
  id: number;
  parque: string;
  tipo: string;
  titulo: string;
  descricao: string;
  dataPublicacao: string;
  dataInicio: string;
  dataFim: string | null;
  foto: string;
  dataCriacao: string;
  dataAtualizacao: string | null;
}

interface PaginaDto<T> {
  itens: T[];
  pagina: number;
  tamanhoPagina: number;
  totalItens: number;
  totalPaginas: number;
}

interface FiltroPostagens {
  parque?: string;
  tipo?: string;
  pagina?: number;
  tamanhoPagina?: number;
}

// Criacao do servico
class PostagemService extends BaseCrudService<PostagemDto> {
  constructor() {
    super("/postagens");
  }

  async getPostagens(filtro: FiltroPostagens = {}): Promise<PaginaDto<PostagemDto>> {
    // Monta os filtros em cabecalhos
    const headers = new Headers();
    if (filtro.parque) headers.set("parque", filtro.parque);
    if (filtro.tipo) headers.set("tipo", filtro.tipo);
    if (filtro.pagina) headers.set("pagina", filtro.pagina.toString());
    if (filtro.tamanhoPagina) headers.set("tamanhoPagina", filtro.tamanhoPagina.toString());

    return this.request<PaginaDto<PostagemDto>>(this.baseUrl, {
      method: "GET",
      headers,
    });
  }

  async getDestaques(): Promise<PostagemDto[]> {
    return this.request<PostagemDto[]>(`${this.baseUrl}/destaques`, {
      method: "GET",
    });
  }

  async getById(id: number): Promise<PostagemDto> {
    return this.request<PostagemDto>(`${this.baseUrl}/detalhe`, {
      method: "GET",
      headers: { id: id.toString() },
    });
  }
}

export { type FiltroPostagens, type PaginaDto, type PostagemDto, PostagemService };
