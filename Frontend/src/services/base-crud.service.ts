export class BaseCrudService<T> {
  protected baseUrl: string;

  constructor(endpoint: string, baseApiUrl: string = "/api") {
    this.baseUrl = `${baseApiUrl}${endpoint}`;
    console.log(`BaseCrudService initialized with endpoint: ${this.baseUrl}`);
  }

  // tratar as requisições HTTP
  protected async request<R>(url: string, config: RequestInit): Promise<R> {
    try {
      console.log(`Making request to: ${url} with config:`, config);
      const response = await fetch(url, {
        headers: {
          "Content-Type": "application/json",
          ...config.headers,
        },
        ...config,
      });

      if (!response.ok) {
        throw new Error(
          `Erro na requisição: ${response.status} - ${response.statusText}`,
        );
      }

      // Evita erro se for vazio
      if (response.status === 204) {
        return {} as R;
      }

      return await response.json();
    } catch (error) {
      throw error;
    }
  }

  // criar
  async create(data: T): Promise<T> {
    return this.request<T>(this.baseUrl, {
      method: "POST",
      body: JSON.stringify(data),
    });
  }

  // ler
  async getAll(): Promise<T[]> {
    return this.request<T[]>(this.baseUrl, {
      method: "GET",
    });
  }

  // get
  async getById(id: string | number): Promise<T> {
    return this.request<T>(`${this.baseUrl}/${id}`, {
      method: "GET",
    });
  }

  // deletar
  async delete(id: string | number): Promise<void> {
    return this.request<void>(`${this.baseUrl}/${id}`, {
      method: "DELETE",
    });
  }
}
