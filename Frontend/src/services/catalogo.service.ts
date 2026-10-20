import { BaseCrudService } from "./base-crud.service";

interface CatalogoDto {
  id: number;
  codigo: string;
  nome: string;
}

// Criacao do servico
class CatalogoService extends BaseCrudService<CatalogoDto> {
  constructor() {
    super("");
  }

  async getParques(): Promise<CatalogoDto[]> {
    return this.request<CatalogoDto[]>(`${this.baseUrl}/parques`, {
      method: "GET",
    });
  }

  async getTiposPostagem(): Promise<CatalogoDto[]> {
    return this.request<CatalogoDto[]>(`${this.baseUrl}/tipos-postagem`, {
      method: "GET",
    });
  }
}

export { type CatalogoDto, CatalogoService };
