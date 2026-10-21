# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER root
WORKDIR /app
# Mantém o banco fora dos arquivos publicados e permite escrita pelo usuário da aplicação.
RUN mkdir -p /data && chown $APP_UID /data
USER $APP_UID
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CircuitoTereVerdeWebApi/CircuitoTereVerdeWebApi.csproj", "CircuitoTereVerdeWebApi/"]
RUN dotnet restore "./CircuitoTereVerdeWebApi/CircuitoTereVerdeWebApi.csproj"
COPY . .
WORKDIR "/src/CircuitoTereVerdeWebApi"
RUN dotnet build "./CircuitoTereVerdeWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CircuitoTereVerdeWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CircuitoTereVerdeWebApi.dll"]
