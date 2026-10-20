import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import BotaoParque from "../botaoParque";

import LogoSerraOrgaos from "@/assets/logo-serra-orgaos.png";
import LogoMontanhasTeresopolis from "@/assets/logo-montanhas-teresopolis.png";
import LogoTresPicos from "@/assets/logo-tres-picos.png";

import type { FiltroPostagens } from "@/services/postagem.service";

export default function FiltrosListagem({
  onFiltroAlterado,
}: {
  onFiltroAlterado: (filtro: Omit<FiltroPostagens, "pagina">) => void;
}) {
  const [filtroSelecionado, setFiltroSelecionado] = useState(0);
  const [parqueSelecionado, setParqueSelecionado] = useState(0);

  const botoes = [
    { label: "TODOS", codigo: undefined },
    { label: "TRILHAS", codigo: "TRILHAS" },
    { label: "CACHOEIRAS", codigo: "CACHOEIRAS" },
    { label: "BIODIVERSIDADE", codigo: "BIODIVERSIDADE" },
    { label: "EVENTOS", codigo: "EVENTOS" },
    { label: "NOVIDADES", codigo: "NOVIDADES" },
    { label: "ATRAÇÕES", codigo: "ATRAÇÕES" },
    { label: "SOBRE", codigo: "SOBRE" },
  ];

  const parques = [
    { codigo: "PNSO", label: "Parque Nacional da Serra dos Órgãos", logo: LogoSerraOrgaos },
    {
      codigo: "PNMMT",
      label: "Parque Natural Municipal Montanhas de Teresópolis",
      logo: LogoMontanhasTeresopolis,
    },
    { codigo: "PETP", label: "Parque Estadual dos Três Picos", logo: LogoTresPicos },
  ];

  useEffect(() => {
    onFiltroAlterado({
      parque: parques[parqueSelecionado].codigo,
      tipo: botoes[filtroSelecionado].codigo,
    });
  }, [filtroSelecionado, onFiltroAlterado, parqueSelecionado]);

  return (
    <>
      <div className="w-full md:w-1/2 flex justify-center pt-2 gap-10">
        {parques.map(({ label, logo }, index) => (
          <BotaoParque
            key={index}
            label={label}
            logo={logo}
            selecionado={parqueSelecionado === index}
            onClick={() => setParqueSelecionado(index)}
          />
        ))}
      </div>
      <div className="p-2 rounded-md bg-secondary flex flex-wrap justify-center gap-2 space-between w-full">
        {botoes.map(({ label }, index) => (
          <Button
            key={index}
            onClick={() => setFiltroSelecionado(index)}
            variant={filtroSelecionado === index ? "selected" : "default"}
          >
            {label}
          </Button>
        ))}
      </div>
    </>
  );
}
