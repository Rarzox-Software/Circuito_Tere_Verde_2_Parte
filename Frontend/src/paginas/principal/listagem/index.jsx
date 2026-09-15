import { useState, useCallback, useEffect } from "react"
import FiltrosListagem from "./filtros"
import CardPostagem from "./card"

import { MockStore } from "../../../dados/MockStore.ts";

export default function Listagem() {
  const store = MockStore.getInstance();
  const [postagens, setPostagens] = useState(store.getPostagensPorParqueETipo(0, -1));

  const retornoFiltro = useCallback((lista) => {
    setPostagens(lista);
  }, []);

  useEffect(() => {
    // Rola tela ao filtrar as postagens
    if (postagens.length > 0) {
      requestAnimationFrame(() => {
        document.getElementById("card-0")?.scrollIntoView({
          behavior: "smooth",
          block: "start",
        });
      });
    }
  }, [postagens]);

  return (
    <>
      <FiltrosListagem retornoFiltro={retornoFiltro} />
      <div className="flex flex-col gap-2 w-full">
        {postagens.map((post, index) => (
          <CardPostagem key={post.id} postagem={post} id={`card-${index}`} />
        ))}
      </div>
    </>
  )
}