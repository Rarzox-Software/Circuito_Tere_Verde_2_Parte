import * as React from "react"
import Autoplay from "embla-carousel-autoplay"
import { MockStore } from "../../dados/MockStore.ts";

import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel"

export default function CustomCarousel() {

  const store = MockStore.getInstance(); // acessa dados mocado
  const postagens = store.getUltimasPostagens();  // pega as 5 últimas Postagens

  return (
    <Carousel
      className="w-full relative overflow-hidden"
      plugins={[
        Autoplay({
          delay: 6000,
        }),
      ]}>
      <CarouselContent>
        {postagens.map((post) => (
          <CarouselItem key={post.id}>
           <div className="bg-white rounded-xl shadow-md flex gap-4 items-center px-8 py-4 h-[160px]">

            {/* Imagem à esquerda */}
            <img 
              src={post.foto} 
              className="w-28 h-28 object-cover rounded-lg"
              alt={post.titulo}
            />

            {/* Texto à direita */}
            <div className="flex flex-col">
              <h2 className="text-xl font-bold text-primary">
                {post.titulo}
              </h2>

              <h2 className="text-xs font-bold text-primary">
                {post.parque}
              </h2>

              <p className="text-gray-700 mt-1 text-sm leading-tight line-clamp-3 overflow-hidden">
                {post.descricao}
              </p>
            </div>

        </div>
          </CarouselItem>
        ))}
      </CarouselContent>
      <CarouselPrevious className="absolute left-0 top-1/2 -translate-y-1/2 z-50" />
      <CarouselNext className="absolute right-0 top-1/2 -translate-y-1/2 z-50" />
    </Carousel>
  )
}
