import { Routes, Route } from "react-router-dom";
import Principal from "./paginas/principal";
import Login from "./paginas/login/login";
import PainelControle from "./paginas/painel-controle/painelControle";
import RotaProtegida from "./components/rotaProtegida";

export default function Rotas() {
  return (
    <Routes>
      <Route path="/" element={<Principal />} />
      <Route path="/login" element={<Login />} />
      <Route
        path="/postagens"
        element={
          <RotaProtegida>
            <PainelControle />
          </RotaProtegida>
        }
      />
    </Routes>
  );
}
