import { BaseCrudService } from "./base-crud.service";

// interfaces
interface AuthDto {
  email: string;
  senha: string;
}

interface UsuarioAutenticadoDto {
  id: string;
  nome: string;
  email: string;
  perfis: string[];
}

// Criacao do servico
class AuthService extends BaseCrudService<AuthDto> {
  constructor() {
    super("/auth");
  }

  async login(credenciais: AuthDto): Promise<UsuarioAutenticadoDto> {
    // Monta a URL e corpo
    return this.request<UsuarioAutenticadoDto>(`${this.baseUrl}/login`, {
      method: "POST",
      body: JSON.stringify(credenciais),
    });
  }

  async logout(): Promise<void> {
    // Monta a URL
    return this.request<void>(`${this.baseUrl}/logout`, {
      method: "POST",
    });
  }

  async me(): Promise<UsuarioAutenticadoDto> {
    // Monta a URL
    return this.request<UsuarioAutenticadoDto>(`${this.baseUrl}/me`, {
      method: "GET",
    });
  }
}
export { type AuthDto, type UsuarioAutenticadoDto, AuthService };
