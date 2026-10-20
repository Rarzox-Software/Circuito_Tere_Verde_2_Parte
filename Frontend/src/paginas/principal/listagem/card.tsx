import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Fragment, type HTMLAttributes } from "react";
import type { PostagemDto } from "@/services/postagem.service";

interface CardPostagemProps extends HTMLAttributes<HTMLDivElement> {
  postagem: PostagemDto;
}

function formatText(text: string) {
  return text.split("\n").map((line, i) => (
    <Fragment key={i}>
      {line}
      <br />
    </Fragment>
  ));
}

export default function CardPostagem({ postagem, className, ...props }: CardPostagemProps) {
  return (
    <Card
      className={className}
      {...props}
    >
      <CardHeader>
        <CardTitle>
          {postagem.titulo}
        </CardTitle>

        <CardContent>
          <img src={postagem.foto} className="w-full h-100 object-cover rounded-xl"/>
        </CardContent>

        <CardDescription>
          {formatText(postagem.descricao)}
        </CardDescription>

        {/* <CardAction>Card Action</CardAction> */}

      </CardHeader>

      <CardFooter>
        <p> Data da Publicação {new Date(postagem.dataPublicacao).toLocaleString("pt-BR", { timeZone: "UTC" })}</p>
      </CardFooter>
      
    </Card>
  )
}
