import React, { useEffect, useState } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import Logo from "../assets/logo.svg?react";
import { MockStore } from "../dados/MockStore";
import Container from "./container";

export default function Cabecalho() {

  const store = MockStore.getInstance();
  const navigate = useNavigate();
  const location = useLocation();

  const [logado, setLogado] = useState(store.estaLogado());

  // Detecta login/logout
  useEffect(() => {
    store.subscribe(() => {
      setLogado(store.estaLogado());
    });
  }, []);

  const estaNaPaginaPostagens = location.pathname === "/postagens";

  function sair() {
    store.logout();
    navigate("/");
  }

  //Retorna título conforme rota
  function obterTitulo() {
    if (location.pathname === "/postagens") return "Postagens";
    return "Circuito Terê Verde";
  }

  return (
    <header className="w-full bg-primary">
      <Container className="flex justify-between items-center py-4 text-white">

        <Link to="/">
          <Logo className="cursor-pointer" />
        </Link>

        <h1 className="text-xl font-semibold">
          {obterTitulo()}
        </h1>

        <div className="flex gap-4 items-center">

          {logado && (
            <>
              {!estaNaPaginaPostagens && (
                <Link to="/postagens">Publicações</Link>
              )}

              <button onClick={sair}>Sair</button>
            </>
          )}

          {!logado && (
            <Link to="/login">Login</Link>
          )}

        </div>

      </Container>
    </header>
  );
}
