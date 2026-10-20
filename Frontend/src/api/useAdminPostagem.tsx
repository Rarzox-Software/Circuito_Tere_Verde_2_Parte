import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  PostagemAdminService,
  type SalvarPostagemDto,
} from "@/services/postagem-admin.service";

const postagemAdminService = new PostagemAdminService();

// Atualiza consultas dependentes de postagens
function invalidarPostagens(queryClient: ReturnType<typeof useQueryClient>) {
  queryClient.invalidateQueries({ queryKey: ["adminpostagens"] });
  queryClient.invalidateQueries({ queryKey: ["postagens"] });
  queryClient.invalidateQueries({ queryKey: ["destaques"] });
}

// Hook para listar
export function usePostagensAdmin(pagina = 1, tamanhoPagina = 12) {
  return useQuery({
    queryKey: ["adminpostagens", pagina, tamanhoPagina],
    queryFn: () => postagemAdminService.getPostagens({ pagina, tamanhoPagina }),
  });
}

// Hook para obter postagem administrativa
export function usePostagemAdmin(id?: number) {
  return useQuery({
    queryKey: ["adminpostagem", id],
    queryFn: () => postagemAdminService.getById(id!),
    enabled: Boolean(id),
  });
}

// Hook para criar
export function useCreatePostagem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (postagem: SalvarPostagemDto) => postagemAdminService.create(postagem),
    onSuccess: () => {
      invalidarPostagens(queryClient);
    },
  });
}

// Hook para editar
export function useUpdatePostagem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, postagem, foto }: { id: number; postagem: SalvarPostagemDto; foto?: File }) =>
      postagemAdminService.update(id, postagem, foto),
    onSuccess: () => {
      invalidarPostagens(queryClient);
    },
  });
}

// Hook para excluir
export function useDeletePostagem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => postagemAdminService.delete(id),
    onSuccess: () => {
      invalidarPostagens(queryClient);
    },
  });
}
