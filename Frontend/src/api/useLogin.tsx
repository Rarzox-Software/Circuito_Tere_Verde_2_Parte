import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { AuthService, AuthDto } from "@/services/auth.service";

const authService = new AuthService();

// Hook para logar
export function useLogin() {
  return useMutation({
    mutationFn: (credenciais: AuthDto) => authService.login(credenciais),
    onSuccess: (data) => {
      // teste
      console.log("Login efetuado com sucesso!", data);
    },
    onError: (error) => {
      // Tratamento de erro
      console.error("Falha no login:", error.message);
    },
  });
}
