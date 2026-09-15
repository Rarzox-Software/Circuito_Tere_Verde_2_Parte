import React from 'react';

export default function BotaoParque({ logo, label, selecionado, ...props }) {
  return (
    <div
      className="h-full flex flex-col items-center cursor-pointer"
      {...props}
    >
      <div
        className={`
      rounded-full transition-all duration-200
      ${selecionado ? "scale-102" : "scale-100"}
    `}
      >
        <img src={logo}
          alt={`Logo: ${label}`}
          className={`
            rounded-full transition-all duration-200
            ${selecionado ? "opacity-100" : "opacity-40"}
          `}
        />
      </div>

      <span className="mt-1 text-center items-end font-bold text-sm md:text-lg text-primary">{label}</span>

      <div
        className={`
      mt-2
      h-2 w-2/3 rounded-full transition-all duration-200
      ${selecionado ? "bg-primary opacity-100" : "opacity-0"}
    `}
      />
    </div>
  );
}