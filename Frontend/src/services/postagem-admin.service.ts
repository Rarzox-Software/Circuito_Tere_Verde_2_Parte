import { BaseCrudService } from "./base-crud.service";
import type { PaginaDto, PostagemDto } from "./postagem.service";

// Interfaces
interface SalvarPostagemDto {
  parque: string;
  tipo: string;
  titulo: string;
  descricao: string;
  dataInicio: string;
  dataFim: string;
  foto: string;
}

interface FiltroPostagensAdmin {
  pagina?: number;
  tamanhoPagina?: number;
}

// Criacao do servico
class PostagemAdminService extends BaseCrudService<SalvarPostagemDto> {
  constructor() {
    super("/admin/postagens");
  }

  async getPostagens(filtro: FiltroPostagensAdmin = {}): Promise<PaginaDto<PostagemDto>> {
    const headers = new Headers();
    if (filtro.pagina) headers.set("pagina", filtro.pagina.toString());
    if (filtro.tamanhoPagina) headers.set("tamanhoPagina", filtro.tamanhoPagina.toString());

    return this.request<PaginaDto<PostagemDto>>(this.baseUrl, {
      method: "GET",
      headers,
    });
  }

  async getById(id: number): Promise<PostagemDto> {
    return this.request<PostagemDto>(`${this.baseUrl}/selecionada`, {
      method: "GET",
      headers: { id: id.toString() },
    });
  }

  async create(dados: SalvarPostagemDto): Promise<PostagemDto> {
    return this.request<PostagemDto>(this.baseUrl, {
      method: "POST",
      body: JSON.stringify(dados),
    });
  }

  async update(id: number, dados: SalvarPostagemDto, foto?: File): Promise<PostagemDto> {
    const formulario = new FormData();
    formulario.append("parque", dados.parque);
    formulario.append("tipo", dados.tipo);
    formulario.append("titulo", dados.titulo);
    formulario.append("descricao", dados.descricao);
    formulario.append("dataInicio", dados.dataInicio);
    if (dados.dataFim) formulario.append("dataFim", dados.dataFim);
    if (foto) formulario.append("foto", foto);

    return this.request<PostagemDto>(this.baseUrl, {
      method: "PUT",
      headers: { id: id.toString() },
      body: formulario,
    });
  }

  async delete(id: number): Promise<void> {
    return this.request<void>(this.baseUrl, {
      method: "DELETE",
      headers: { id: id.toString() },
    });
  }
}

export {
  type FiltroPostagensAdmin,
  type SalvarPostagemDto,
  PostagemAdminService,
};
