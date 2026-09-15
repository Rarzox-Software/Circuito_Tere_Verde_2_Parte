import React from "react"
import Carousel from "./carousel"
import Listagem from "./listagem"
import Container from "@/components/container"

export default function Principal() {
  return (
    <Container>
      <div className="flex flex-col items-center justify-center gap-3 sm:gap-5 mt-5">
        <Carousel />
        <Listagem />
      </div>
    </Container>
  )
}