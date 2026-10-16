import { useQuery } from "@tanstack/react-query";
import { PostagemService } from "@/services/postagem.service";

const postagemService = new PostagemService();

// Hook para listar
export function usePostagens(parque: number) {
  return useQuery({
    queryKey: ["postagens", parque],
    queryFn: () => postagemService.getPostagensPorParque(parque),
  });
}
