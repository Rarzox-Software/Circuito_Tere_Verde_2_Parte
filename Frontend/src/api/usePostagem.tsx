import { useInfiniteQuery, useQuery } from "@tanstack/react-query";
import type { FiltroPostagens } from "@/services/postagem.service";
import { PostagemService } from "@/services/postagem.service";

const postagemService = new PostagemService();

// Hook para listar postagens publicas
export function usePostagens(filtro: FiltroPostagens = {}) {
  return useQuery({
    queryKey: ["postagens", filtro],
    queryFn: () => postagemService.getPostagens(filtro),
  });
}

// Hook para listar postagens com paginacao continua
export function usePostagensInfinito(filtro: Omit<FiltroPostagens, "pagina"> = {}) {
  return useInfiniteQuery({
    queryKey: ["postagens", filtro],
    initialPageParam: 1,
    queryFn: ({ pageParam }) => postagemService.getPostagens({ ...filtro, pagina: pageParam }),
    getNextPageParam: (ultimaPagina) =>
      ultimaPagina.pagina < ultimaPagina.totalPaginas
        ? ultimaPagina.pagina + 1
        : undefined,
  });
}

// Hook para listar destaques
export function useDestaques() {
  return useQuery({
    queryKey: ["destaques"],
    queryFn: () => postagemService.getDestaques(),
  });
}

// Hook para obter postagem publica
export function usePostagem(id?: number) {
  return useQuery({
    queryKey: ["postagem", id],
    queryFn: () => postagemService.getById(id!),
    enabled: Boolean(id),
  });
}
