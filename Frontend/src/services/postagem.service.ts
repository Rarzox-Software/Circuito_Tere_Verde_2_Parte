import { BaseCrudService } from "./base-crud.service";

// interfaces
interface PostagemDto {
  parque: string;
  tipo: string;
  titulo: string;
  descricao: string;
  dataInicio: string;
  dataFim: string;
  foto: string;
}

// Criacao do Serviço
class PostagemService extends BaseCrudService<PostagemDto> {
  constructor() {
    super("/postagens");
  }

  async getPostagensPorParque(parque: number): Promise<PostagemDto[]> {
    // Monta a URL
    return this.request<PostagemDto[]>(this.baseUrl, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        parque: parque.toString(),
      },
    });
  }
}

export { type PostagemDto, PostagemService };
