import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { MockStore } from "../dados/MockStore.ts";

export default function Login() {

  // dados mockado teste
  const store = MockStore.getInstance();

  // router
  const navigate = useNavigate();

  // dados login
  const [login, setLogin] = useState("");
  const [senha, setSenha] = useState("");
  const [erro, setErro] = useState("");

  // funcoes
  function fazerLogin(e) {
    e.preventDefault();
    const resultado = store.login(login, senha);

    if (!resultado) {
      setErro("Email ou senha inválidos.");
      return;
    }

    setErro("");

    // direciona para painel de controle
    navigate("/postagens");
  }

  return (
 <div className="w-screen h-screen flex items-center justify-center bg-[#e7e5d8]">
      <div className="bg-white/60 backdrop-blur-md shadow-xl p-10 rounded-xl w-80 border border-white/40">
        
        <h2 className="text-center text-xl font-semibold text-gray-700 mb-6">
          Login
        </h2>

        <form onSubmit={fazerLogin} className="flex flex-col gap-4">

          <div className="flex flex-col text-sm text-gray-600">
            <label>Email</label>
            <input
              type="email"
              className="mt-1 p-2 bg-white/40 rounded-md border border-gray-300 focus:outline-none focus:ring-2 focus:secondary"
              value={login}
              onChange={(e) => setLogin(e.target.value)}
            />
          </div>

          <div className="flex flex-col text-sm text-gray-600">
            <label>Senha</label>
            <input
              type="password"
              className="mt-1 p-2 bg-white/40 rounded-md border border-gray-300 focus:outline-none focus:ring-2 focus:secondary"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
            />
          </div>

          {erro && (
            <p className="text-red-700 text-sm text-center animate-pulse">
              {erro}
            </p>
          )}

          <button
            type="submit"
            className="
              mt-2 bg-primary text-white py-2 rounded-md font-semibold 
              transition-all duration-200 
              hover:bg-secondary
              active:scale-95 
              hover:shadow-lg
            "
          >
            ENTRAR
          </button>

        </form>

        <p className="text-center text-xs text-green-900/60 mt-4 cursor-pointer">
          Recuperar senha
        </p>
      </div>
    </div>
  )
}