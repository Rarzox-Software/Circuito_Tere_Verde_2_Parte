import { FileBlob, SpreadsheetFile } from "file:///C:/Users/RAFAEL%20MENDES/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/@oai/artifact-tool/dist/artifact_tool.mjs";

const caminho = "C:/PROJETOS-TRADE/MVP/Circuito_Tere_Verde_2_Parte/requisitos/Requisitos_Backend.xlsx";
const arquivo = await FileBlob.load(caminho);
const workbook = await SpreadsheetFile.importXlsx(arquivo);
const planilhas = [];

for (let indice = 0; indice < workbook.worksheets.getSheetCount(); indice += 1) {
  const planilha = workbook.worksheets.getItemAt(indice);
  const dados = await workbook.inspect({
    select: `$.sheets[?(@.name == '${planilha.name}')].cells[?(@.row <= 40 && @.column <= 4)]`,
    include: ["address", "value", "text", "computedStyle.fillColor", "computedStyle.font.color"]
  });
  planilhas.push({ nome: planilha.name, dados: dados.records });
}

console.log(JSON.stringify(planilhas));
