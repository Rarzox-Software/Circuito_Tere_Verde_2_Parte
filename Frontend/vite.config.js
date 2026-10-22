import path from "path";
import { fileURLToPath } from "url";
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import svgr from "vite-plugin-svgr";
import tailwindcss from "@tailwindcss/vite";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const apiProxyUrl = "http://localhost:44390";

// https://vite.dev/config/
export default defineConfig({
  plugins: [tailwindcss(), react(), svgr()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    port: 44391,
    proxy: {
      "/api": {
        // Mantém o navegador no endereço do frontend e encaminha chamadas para a API HTTP local.
        target: apiProxyUrl,
        changeOrigin: true,
        // Também aceita o certificado de desenvolvimento quando a API for iniciada pelo IIS Express.
        secure: false,
        ws: true,
        configure: (proxy) => {
          proxy.on("error", (err) => {
            console.log("Erro no Proxy do Vite:", err);
          });
        },
      },
    },
  },
});
