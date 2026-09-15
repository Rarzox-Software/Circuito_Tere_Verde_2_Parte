import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

function formatText(text) {
  return text.split("\n").map((line, i) => (
    <>
      {line}
      <br key={i} />
    </>
  ));
}

export default function CardPostagem({ postagem, className, ...props }) {
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
        <p> Data da Publicação {postagem.dataPublicacao.toLocaleString('pt-BR', { timezone: 'UTC' })}</p>
      </CardFooter>
      
    </Card>
  )
}