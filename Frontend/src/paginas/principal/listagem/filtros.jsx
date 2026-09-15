import { useState, useEffect } from "react"
import { Button } from "@/components/ui/button"
import BotaoParque from "../botaoParque"

import LogoSerraOrgaos from '../../../assets/logo-serra-orgaos.png'
import LogoMontanhasTeresopolis from '../../../assets/logo-montanhas-teresopolis.png'
import LogoTresPicos from '../../../assets/logo-tres-picos.png'

import { MockStore } from "../../../dados/MockStore.ts";

export default function FiltrosListagem({ retornoFiltro }) {
  const store = MockStore.getInstance();

  const [filtroSelecionado, setFiltroSelecionado] = useState(0);
  const [parqueSelecionado, setParqueSelecionado] = useState(0);

  const botoes = [
    "TODOS",
    "TRILHAS",
    "CACHOEIRAS",
    "BIODIVERSIDADE",
    "EVENTOS",
    "NOVIDADES",
    "ATRAÇÕES",
    "SOBRE"
  ];

  const parques = [
    { label: "Parque Nacional da Serra dos Órgãos", logo: LogoSerraOrgaos },
    { label: "Parque Natural Municipal Montanhas de Teresópolis", logo: LogoMontanhasTeresopolis },
    { label: "Parque Estadual dos Três Picos", logo: LogoTresPicos }
  ];

  useEffect(() => {
    retornoFiltro(store.getPostagensPorParqueETipo(parqueSelecionado, filtroSelecionado - 1));
  }, [filtroSelecionado, parqueSelecionado, retornoFiltro, store]);

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
        {botoes.map((label, index) => (
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
  )
}