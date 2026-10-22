import { Link, useNavigate, useLocation } from "react-router-dom";
import { LogOut, Newspaper } from "lucide-react";
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
                <Link
                  to="/postagens"
                  className="inline-flex items-center gap-2 rounded-full bg-white px-4 py-2 text-sm font-semibold text-primary shadow-sm transition-colors hover:bg-[#E3EAD8] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white/80"
                >
                  <Newspaper className="size-4" aria-hidden="true" />
                  Publicações
                </Link>
              )}

              <button
                className="inline-flex items-center gap-2 rounded-full border border-white/60 px-4 py-2 text-sm font-semibold transition-colors hover:bg-white/15 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white/80 disabled:cursor-not-allowed disabled:opacity-60"
                disabled={logoutMutation.isPending}
                onClick={sair}
              >
                <LogOut className="size-4" aria-hidden="true" />
                {logoutMutation.isPending ? "Saindo..." : "Sair"}
              </button>
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
