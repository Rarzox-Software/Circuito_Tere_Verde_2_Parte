import { useState } from "react";
import { getParque, getTipo } from "../../dados/Enumerations";
import type { PostagemDto } from "@/services/postagem.service";
import { useDeletePostagem, usePostagensAdmin } from "@/api/useAdminPostagem";
import PostagemForm from "./cadastroPostagem";


export default function Painel() {

  // Variaveis do modal
  const [modalAberto, setModalAberto] = useState(false);
  const [postagemSelecionada, setPostagemSelecionada] = useState<PostagemDto | null>(null);
  const { data: resultado } = usePostagensAdmin();
  const deletePostagemMutation = useDeletePostagem();
  const postagens = resultado?.itens ?? [];

  // funcoes botoes
  // novo cadastro
  function novoRegistro() {
    setPostagemSelecionada(null);
    setModalAberto(true);
  }

  // editar
  function editarRegistro() {
    if (!postagemSelecionada) return;
    setModalAberto(true);
  }

  // Exclui a postagem selecionada
  async function excluirRegistro() {
    if (!postagemSelecionada || deletePostagemMutation.isPending) return;

    const exclusaoConfirmada = window.confirm(
      "Tem certeza que deseja excluir esta postagem? Esta ação não poderá ser desfeita.",
    );

    if (!exclusaoConfirmada) return;

    await deletePostagemMutation.mutateAsync(postagemSelecionada.id);
    setPostagemSelecionada(null);
  }

  // Seleciona linha
  function selecionarLinha(p: PostagemDto) {
    setPostagemSelecionada(p);
  }

  return (
    <div className="min-h-screen bg-[#F3F0E6]">

      <main className="px-6 lg:px-20 py-10">

        {/* Título e botões */}
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-semibold text-[#1E4636]"></h2>

          <div className="flex gap-3">
            <button className="btn-acao bg-[#2E5A47] hover:bg-[#244c3c]" onClick={novoRegistro}>
              NOVO
            </button>
            <button
              className="btn-acao bg-[#B5C6A6] text-[#1E4636] hover:bg-[#a5b996] disabled:cursor-not-allowed disabled:opacity-50"
              disabled={!postagemSelecionada}
              onClick={editarRegistro}
            >
              EDITAR
            </button>
            <button
              className="btn-acao bg-red-600 hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-50"
              disabled={!postagemSelecionada || deletePostagemMutation.isPending}
              onClick={excluirRegistro}
            >
              EXCLUIR
            </button>
          </div>
        </div>

        {/* Grid */}
        <div className="bg-white shadow rounded-lg overflow-hidden border border-[#d7d7d7]">
          <table className="w-full">
            <thead>
              <tr className="bg-[#eef2ed] text-[#1E4636] border-b border-[#c1c8bc]">
                <th className="th">Parque</th>
                <th className="th">Título</th>
                <th className="th">Tipo</th>
                <th className="th">Data</th>
              </tr>
            </thead>

            <tbody>
              {postagens.map((p) => (
                <tr key={p.id}
                  className={`linha cursor-pointer ${postagemSelecionada?.id === p.id ? "bg-green-100" : ""
                    }`}
                  onClick={() => selecionarLinha(p)}
                >
                  <td className="td">{getParque(p.parque)}</td>
                  <td className="td">{p.titulo}</td>
                  <td className="td">{getTipo(p.tipo)}</td>
                  <td className="td">
                    {new Date(p.dataPublicacao).toLocaleDateString("pt-BR")}
                  </td>
                </tr>
              ))}

              {postagens.length === 0 && (
                <tr>
                  <td className="td text-center py-10 text-gray-500" colSpan={4}>
                    Nenhum registro encontrado.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

      </main>

      {modalAberto && (
        <div className="fixed inset-0 bg-black/30 flex items-center justify-center">
          <PostagemForm
            postagem={postagemSelecionada}
            onCancel={() => setModalAberto(false)}
            onSaved={() => {
              setModalAberto(false);
              setPostagemSelecionada(null);
            }}
          />
        </div>
      )}

    </div>
  );
}
