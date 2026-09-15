import { useState, useEffect } from "react";
import { MockStore } from "../../dados/MockStore.ts";
import { ListaParques, ListaTipos } from "../../dados/Enumerations.ts";

export default function PostagemForm({ postagem, onCancel, onSaved }) {

  const store = MockStore.getInstance();

  // ----------------- ESTADO -----------------
  const [form, setForm] = useState({
    parque: ListaParques[0],
    tipo: ListaTipos[0],
    titulo: "",
    descricao: "",
    foto: "",
    dataInicio: "",
    dataFim: ""
  });

  const [erro, setErro] = useState("");

  // ----------------- CARREGAR DADOS DA EDIÇÃO -----------------
  useEffect(() => {
    if (postagem) {
      setForm({
        parque: postagem.parque,
        tipo: postagem.tipo,
        titulo: postagem.titulo,
        descricao: postagem.descricao,
        foto: postagem.foto,
        dataInicio: postagem.dataInicio
          ? postagem.dataInicio.toISOString().substr(0, 10)
          : "",
        dataFim: postagem.dataFim
          ? postagem.dataFim.toISOString().substr(0, 10)
          : ""
      });
    }
  }, [postagem]);

  // ----------------- ATUALIZAR CAMPOS -----------------
  function atualizar(campo, valor) {
    setForm({ ...form, [campo]: valor });
  }

  // FOTO
  function handleFoto(e) {
    const file = e.target.files[0];
    if (!file) return;

    const url = URL.createObjectURL(file);
    setForm({ ...form, foto: url });
  }

  // ----------------- SALVAR -----------------
  function salvar() {

    // ---------- VALIDAÇÕES ----------
    if (!form.titulo.trim()) {
      setErro("Informe um título.");
      return;
    }

    if (!form.descricao.trim()) {
      setErro("Informe uma descrição.");
      return;
    }

    if (!form.dataInicio) {
      setErro("Informe a data de início.");
      return;
    }

    if (!form.foto) {
      setErro("Selecione uma foto.");
      return;
    }

    // limpar erro
    setErro("");

    // montar dados finais
    const dados = {
      parque: form.parque,
      tipo: form.tipo,
      titulo: form.titulo,
      descricao: form.descricao,
      foto: form.foto,
      dataInicio: form.dataInicio ? new Date(form.dataInicio) : undefined,
      dataFim: form.dataFim ? new Date(form.dataFim) : undefined,
    };

    if (postagem) {
      store.editarPostagem(postagem.id, dados);
    } else {
      store.addPostagem(dados);
    }

    if (onSaved) onSaved();
  }

  // ----------------- UI -----------------
  return (
    <div className="bg-[#f6f3e7] rounded-xl shadow-md w-[600px] overflow-hidden">
      
      <div className="p-6 max-h-[90vh] overflow-y-auto">

        <h2 className="text-2xl font-semibold text-[#1E4636] mb-5">
          {postagem ? "Editar Postagem" : "Incluir Postagem"}
        </h2>

        <div className="flex flex-col gap-3">


          {/* PARQUE */}
          <div className="flex flex-col gap-1">
            <label className="text-[#1E4636] font-semibold text-sm">Parque</label>
            <select
              className="input"
              value={form.parque}
              onChange={(e) => atualizar("parque", e.target.value)}
            >
              {ListaParques.map((p) => (
                <option key={p}>{p}</option>
              ))}
            </select>
          </div>

          {/* TÍTULO */}
          <div className="flex flex-col gap-1">
            <label className="text-[#1E4636] font-semibold text-sm">Título</label>
            <input
              type="text"
              className="input"
              value={form.titulo}
              onChange={(e) => atualizar("titulo", e.target.value)}
            />
          </div>

          {/* DESCRIÇÃO */}
          <div className="flex flex-col gap-1">
            <label className="text-[#1E4636] font-semibold text-sm">Descrição</label>
            <textarea
              className="input h-24"
              value={form.descricao}
              onChange={(e) => atualizar("descricao", e.target.value)}
            />
          </div>

          {/* TIPO */}
          <div className="flex flex-col gap-1">
            <label className="text-[#1E4636] font-semibold text-sm">Tipo</label>
            <select
              className="input"
              value={form.tipo}
              onChange={(e) => atualizar("tipo", e.target.value)}
            >
              {ListaTipos.map((t) => (
                <option key={t}>{t}</option>
              ))}
            </select>
          </div>

          <div className="flex gap-4 w-full">

            {/* DATA INÍCIO */}
            <div className="flex flex-col gap-1 w-1/2">
              <label className="text-[#1E4636] font-semibold text-sm">Data Início</label>
              <input
                type="date"
                className="input"
                value={form.dataInicio}
                onChange={(e) => atualizar("dataInicio", e.target.value)}
              />
            </div>

            {/* DATA FINAL */}
            <div className="flex flex-col gap-1 w-1/2">
              <label className="text-[#1E4636] font-semibold text-sm">Data Final</label>
              <input
                type="date"
                className="input"
                value={form.dataFim}
                onChange={(e) => atualizar("dataFim", e.target.value)}
              />
            </div>

          </div>

          {/* FOTO */}
          <div className="flex flex-col gap-1">
            <label className="text-[#1E4636] font-semibold text-sm">Foto</label>

            <input
              id="fotoInput"
              type="file"
              accept="image/*"
              className="hidden"
              onChange={handleFoto}
            />

            <button
              onClick={() => document.getElementById("fotoInput").click()}
              className="px-4 py-2 bg-secondary text-primary rounded-md font-semibold hover:bg-accent transition"
            >
              Selecionar Foto
            </button>

            {form.foto && (
              <img
                src={form.foto}
                className="w-32 h-32 object-cover rounded-md self-center border mt-2"
                alt="preview"
              />
            )}
          </div>

        </div>

        {/* ERRO */}
        {erro && (
          <div className="text-red-600 font-semibold text-sm text-center mt-3">
            {erro}
          </div>
        )}

        <div className="flex justify-around mt-6">
          <button
            onClick={onCancel}
            className="font-semibold text-[#94a88a] hover:text-[#6f7f6a]"
          >
            CANCELAR
          </button>

          <button
            onClick={salvar}
            className="px-8 py-2 bg-[#1E4636] text-white rounded-md font-semibold hover:bg-[#163a2c] transition"
          >
            {postagem ? "SALVAR" : "GRAVAR"}
          </button>
        </div>

      </div>
    </div>
  );
}
