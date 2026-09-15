import { useState, useEffect } from "react";
import { ArrowUp } from "lucide-react"; 

export default function BotaoTopo() {
  const [visivel, setVisivel] = useState(false);

  // Mostrar botão somente após rolar a página
  useEffect(() => {
    function aoRolar() {
      setVisivel(window.scrollY > 200);
    }

    window.addEventListener("scroll", aoRolar);
    return () => window.removeEventListener("scroll", aoRolar);
  }, []);

  // Voltar ao topo
  function voltarAoTopo() {
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  return (
    <>
      {visivel && (
        <button
          onClick={voltarAoTopo}
          className="
            fixed bottom-6 right-6 z-50
            bg-primary text-white 
            p-3 rounded-full shadow-lg
            hover:bg-primary/80 transition
            flex items-center justify-center
          "
        >
          <ArrowUp size={22} />
        </button>
      )}
    </>
  );
}
