import { BaseCrudService } from "./base-crud.service";

// interfaces
interface PostagemAdminDto {
  parque: string;
  tipo: string;
  titulo: string;
  descricao: string;
  dataInicio: string;
  dataFim: string;
  foto: string;
}

// Criacao do Serviço
class PostagemAdminService extends BaseCrudService<PostagemAdminDto> {
  constructor() {
    super("/admin/postagens");
  }
}

export { type PostagemAdminDto, PostagemAdminService };
