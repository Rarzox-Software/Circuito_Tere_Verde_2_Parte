import { useState, useEffect } from "react";
import type { PostagemDto } from "@/services/postagem.service";
import type { SalvarPostagemDto } from "@/services/postagem-admin.service";
import { useCreatePostagem, useUpdatePostagem } from "@/api/useAdminPostagem";
import { useParques, useTiposPostagem } from "@/api/useCatalogo";

interface PostagemFormProps {
  postagem: PostagemDto | null;
  onCancel: () => void;
  onSaved: () => void;
}

const converterParaBase64 = (file: File): Promise<string> => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result as string);
    reader.onerror = (error) => reject(error);
  });
};

export default function PostagemForm({
  postagem,
  onCancel,
  onSaved,
}: PostagemFormProps) {
  const createPostagemMutation = useCreatePostagem();
  const updatePostagemMutation = useUpdatePostagem();
  const { data: parques = [] } = useParques();
  const { data: tipos = [] } = useTiposPostagem();

  // Estado do formulario
  const [form, setForm] = useState<SalvarPostagemDto>({
    parque: "",
    tipo: "",
    titulo: "",
    descricao: "",
    foto: "",
    dataInicio: "",
    dataFim: "",
  });

  const [erro, setErro] = useState("");
  const [fotoArquivo, setFotoArquivo] = useState<File | null>(null);

  // Carrega dados da edicao
  useEffect(() => {
    if (postagem) {
      setForm({
        parque: parques.find((parque) => parque.nome === postagem.parque)?.codigo ?? postagem.parque,
        tipo: tipos.find((tipo) => tipo.nome === postagem.tipo)?.codigo ?? postagem.tipo,
        titulo: postagem.titulo,
        descricao: postagem.descricao,
        foto: postagem.foto,
        dataInicio: postagem.dataInicio,
        dataFim: postagem.dataFim ?? "",
      });
      setFotoArquivo(null);
      return;
    }

    setForm((formAtual) => ({
      ...formAtual,
      parque: formAtual.parque || parques[0]?.codigo || "",
      tipo: formAtual.tipo || tipos[0]?.codigo || "",
    }));
    setFotoArquivo(null);
  }, [postagem, parques, tipos]);

  // Atualiza campos do formulario
  function atualizar<K extends keyof SalvarPostagemDto>(
    campo: K,
    valor: SalvarPostagemDto[K],
  ): void {
    setForm({ ...form, [campo]: valor });
  }

  // FOTO
  function handleFoto(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;

    const url = URL.createObjectURL(file);
    setForm({ ...form, foto: url });
    setFotoArquivo(file);
  }

  // Salva a postagem
  async function salvar() {
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

    if (!postagem && !fotoArquivo) {
      setErro("Selecione uma foto.");
      return;
    }

    setErro("");

    try {
      if (postagem) {
        await updatePostagemMutation.mutateAsync({
          id: postagem.id,
          postagem: form,
          foto: fotoArquivo ?? undefined,
        });
      } else {
        await createPostagemMutation.mutateAsync({
          ...form,
          foto: await converterParaBase64(fotoArquivo!),
        });
      }

      onSaved();
    } catch (error) {
      setErro(error instanceof Error ? error.message : "Nao foi possivel salvar a postagem.");
    }
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
            <label className="text-[#1E4636] font-semibold text-sm">
              Parque
            </label>
            <select
              className="input"
              value={form.parque}
              onChange={(e) => atualizar("parque", e.target.value)}
            >
              {parques.map((parque) => (
                <option key={parque.id} value={parque.codigo}>{parque.nome}</option>
              ))}
            </select>
          </div>

          {/* TÍTULO */}
          <div className="flex flex-col gap-1">
            <label className="text-[#1E4636] font-semibold text-sm">
              Título
            </label>
            <input
              type="text"
              className="input"
              value={form.titulo}
              onChange={(e) => atualizar("titulo", e.target.value)}
            />
          </div>

          {/* DESCRIÇÃO */}
          <div className="flex flex-col gap-1">
            <label className="text-[#1E4636] font-semibold text-sm">
              Descrição
            </label>
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
              {tipos.map((tipo) => (
                <option key={tipo.id} value={tipo.codigo}>{tipo.nome}</option>
              ))}
            </select>
          </div>

          <div className="flex gap-4 w-full">
            {/* DATA INÍCIO */}
            <div className="flex flex-col gap-1 w-1/2">
              <label className="text-[#1E4636] font-semibold text-sm">
                Data Início
              </label>
              <input
                type="date"
                className="input"
                value={form.dataInicio}
                onChange={(e) => atualizar("dataInicio", e.target.value)}
              />
            </div>

            {/* DATA FINAL */}
            <div className="flex flex-col gap-1 w-1/2">
              <label className="text-[#1E4636] font-semibold text-sm">
                Data Final
              </label>
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
              onClick={() => document.getElementById("fotoInput")?.click()}
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
