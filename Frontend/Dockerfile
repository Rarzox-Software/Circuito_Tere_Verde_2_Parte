# Compila os arquivos estáticos do React.
FROM node:24-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

RUN npm run build

# Publica o site e encaminha as chamadas /api para o container da API.
FROM nginx:alpine

COPY nginx/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist /usr/share/nginx/html

ENV API_URL=http://circuito-tereverde-api:8080

EXPOSE 80
