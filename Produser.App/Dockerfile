FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Produser.App/Produser.App.csproj Produser.App/
COPY Common.IoC/Common.IoC.csproj Common.IoC/
COPY Consumer.Domain/Consumer.Domain.csproj Consumer.Domain/
COPY Domain.Core/Domain.Core.csproj Domain.Core/
COPY Consumer.Application/Consumer.Application.csproj Consumer.Application/
COPY Infra.DataLayer/Infra.DataLayer.csproj Infra.DataLayer/
COPY RabbitMqBus/RabbitMqBus.csproj RabbitMqBus/
COPY Producer.Application/Producer.Application.csproj Producer.Application/
COPY Producer.Domain/Producer.Domain.csproj Producer.Domain/
COPY RedisControle/RedisControle.csproj RedisControle/
RUN dotnet restore Produser.App/Produser.App.csproj
COPY . .
WORKDIR /src/Produser.App
RUN dotnet build Produser.App.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Produser.App.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Produser.App.dll"]
