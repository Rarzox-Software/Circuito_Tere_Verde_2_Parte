import { useState, useCallback, useEffect, useRef } from "react"
import FiltrosListagem from "./filtros"
import CardPostagem from "./card"
import type { FiltroPostagens } from "@/services/postagem.service";
import { usePostagensInfinito } from "@/api/usePostagem";

export default function Listagem() {
  const [filtro, setFiltro] = useState<Omit<FiltroPostagens, "pagina">>({ parque: "PNSO" });
  const [deveRolar, setDeveRolar] = useState(false);
  const sentinelaRef = useRef<HTMLDivElement>(null);
  const { data, fetchNextPage, hasNextPage, isFetchingNextPage } = usePostagensInfinito(filtro);
  const postagens = data?.pages.flatMap((pagina) => pagina.itens) ?? [];

  const atualizarFiltro = useCallback((novoFiltro: Omit<FiltroPostagens, "pagina">) => {
    setFiltro(novoFiltro);
    setDeveRolar(true);
  }, []);

  useEffect(() => {
    // Rola tela ao filtrar as postagens
    if (deveRolar && postagens.length > 0) {
      requestAnimationFrame(() => {
        document.getElementById("card-0")?.scrollIntoView({
          behavior: "smooth",
          block: "start",
        });
      });
      setDeveRolar(false);
    }
  }, [deveRolar, postagens.length]);

  useEffect(() => {
    const sentinela = sentinelaRef.current;
    if (!sentinela || !hasNextPage) return;

    const observador = new IntersectionObserver(([entrada]) => {
      if (entrada.isIntersecting && !isFetchingNextPage) {
        fetchNextPage();
      }
    }, { rootMargin: "300px" });

    observador.observe(sentinela);
    return () => observador.disconnect();
  }, [fetchNextPage, hasNextPage, isFetchingNextPage]);

  return (
    <>
      <FiltrosListagem onFiltroAlterado={atualizarFiltro} />
      <div className="flex flex-col gap-2 w-full">
        {postagens.map((post, index) => (
          <CardPostagem key={post.id} postagem={post} id={`card-${index}`} />
        ))}
        {data && postagens.length === 0 && (
          <p className="py-10 text-center text-gray-600" role="status">
            Não há postagens de acordo com os filtros selecionados.
          </p>
        )}
        <div ref={sentinelaRef} />
      </div>
    </>
  )
}
