import { useQuery } from "@tanstack/react-query";
import { CatalogoService } from "@/services/catalogo.service";

const catalogoService = new CatalogoService();

// Hook para listar parques
export function useParques() {
  return useQuery({
    queryKey: ["parques"],
    queryFn: () => catalogoService.getParques(),
  });
}

// Hook para listar tipos de postagem
export function useTiposPostagem() {
  return useQuery({
    queryKey: ["tipos-postagem"],
    queryFn: () => catalogoService.getTiposPostagem(),
  });
}
