import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useUsuarioAtual } from "@/api/useLogin";

interface RotaProtegidaProps {
  children: ReactNode;
}

export default function RotaProtegida({ children }: RotaProtegidaProps) {
  const { data: usuario, isPending } = useUsuarioAtual();

  if (isPending) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-[#F3F0E6] text-[#1E4636]">
        Verificando acesso...
      </div>
    );
  }

  if (!usuario) {
    return <Navigate to="/login" replace />;
  }

  return children;
}
