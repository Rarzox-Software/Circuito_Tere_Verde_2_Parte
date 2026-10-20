import { Link, useNavigate, useLocation } from "react-router-dom";
import Logo from "../assets/logo.svg?react";
import { useLogout, useUsuarioAtual } from "@/api/useLogin";
import Container from "./container";

export default function Cabecalho() {
  const navigate = useNavigate();
  const location = useLocation();
  const { data: usuario } = useUsuarioAtual();
  const logoutMutation = useLogout();
  const logado = Boolean(usuario);

  const estaNaPaginaPostagens = location.pathname === "/postagens";

  async function sair() {
    await logoutMutation.mutateAsync();
    navigate("/");
  }

  // Retorna titulo conforme rota
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
