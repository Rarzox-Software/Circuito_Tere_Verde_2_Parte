export class BaseCrudService<T> {
  protected baseUrl: string;

  constructor(endpoint: string, baseApiUrl: string = "/api") {
    this.baseUrl = `${baseApiUrl}${endpoint}`;
  }

  // Trata as requisicoes HTTP
  protected async request<R>(url: string, config: RequestInit): Promise<R> {
    const headers = new Headers(config.headers);
    if (config.body && !(config.body instanceof FormData)) {
      headers.set("Content-Type", "application/json");
    }

    const response = await fetch(url, {
      ...config,
      credentials: "include",
      headers,
    });

    if (!response.ok) {
      const erro = await response.json().catch(() => null);
      throw new Error(erro?.detail ?? erro?.title ?? `Erro na requisicao: ${response.status}.`);
    }

    // Evita erro em respostas sem conteudo
    if (response.status === 204) {
      return undefined as R;
    }

    return response.json() as Promise<R>;
  }
}
