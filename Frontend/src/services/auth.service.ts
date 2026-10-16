import { BaseCrudService } from "./base-crud.service";

// interfaces
interface AuthDto {
  email: string;
  senha: string;
}

// Criacao do Serviço
class AuthService extends BaseCrudService<AuthDto> {
  constructor() {
    super("/auth");
  }

  async login(credenciais: AuthDto): Promise<AuthDto> {
    // Monta a URL e corpo
    return this.request<AuthDto>(`${this.baseUrl}/login`, {
      method: "POST",
      body: JSON.stringify(credenciais),
    });
  }

  async logout(): Promise<AuthDto> {
    // Monta a URL
    return this.request<AuthDto>(`${this.baseUrl}/logout`, {
      method: "POST",
    });
  }

  async me(): Promise<AuthDto> {
    // Monta a URL
    return this.request<AuthDto>(`${this.baseUrl}/me`, {
      method: "GET",
    });
  }
}
export { type AuthDto, AuthService };
