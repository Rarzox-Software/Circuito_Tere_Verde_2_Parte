import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

import Rotas from './rotas'
import Cabecalho from './components/cabecalho'
import BotaoTopo from './components/ui/botaoTopo'
import './index.css'

const rootElement = document.getElementById('root');

if (!rootElement) {
  throw new Error('Elemento #root não foi encontrado.');
}

// Configuracao do React Query
const queryClient = new QueryClient();

createRoot(rootElement).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Cabecalho />
        <Rotas />
        <BotaoTopo />
      </BrowserRouter>
    </QueryClientProvider>
  </StrictMode>,
)
