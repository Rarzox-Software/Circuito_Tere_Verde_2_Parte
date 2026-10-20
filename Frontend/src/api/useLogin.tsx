import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { AuthService, type AuthDto } from "@/services/auth.service";

const authService = new AuthService();

// Hook para logar
export function useLogin() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (credenciais: AuthDto) => authService.login(credenciais),
    onSuccess: (data) => {
      queryClient.setQueryData(["usuario-atual"], data);
    },
  });
}

// Hook para consultar a sessao atual
export function useUsuarioAtual() {
  return useQuery({
    queryKey: ["usuario-atual"],
    queryFn: () => authService.me(),
    retry: false,
  });
}

// Hook para encerrar a sessao
export function useLogout() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => authService.logout(),
    onSuccess: () => {
      queryClient.removeQueries({ queryKey: ["usuario-atual"] });
    },
  });
}
