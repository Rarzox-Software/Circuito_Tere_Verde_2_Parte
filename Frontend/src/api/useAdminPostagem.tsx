import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  PostagemAdminService,
  PostagemAdminDto,
} from "@/services/postagem-admin.service";

const postagemAdminService = new PostagemAdminService();

// Hook para listar
export function usePostagensAdmin() {
  return useQuery({
    queryKey: ["adminpostagens"],
    queryFn: () => postagemAdminService.getAll(),
  });
}

// Hook para criar
export function useCreatePostagem() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (postagem: PostagemAdminDto) =>
      postagemAdminService.create(postagem),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["adminpostagens"] });
    },
  });
}
