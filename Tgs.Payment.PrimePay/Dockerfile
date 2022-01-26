#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.Payment.PrimePay/Tgs.Payment.PrimePay.csproj", "Tgs.Payment.PrimePay/"]
COPY ["PrimePay.Core/PrimePay.Core.csproj", "PrimePay.Core/"]
COPY ["Tgs.Payment.PrimePay.Client/Tgs.Payment.PrimePay.Client.csproj", "Tgs.Payment.PrimePay.Client/"]
RUN dotnet restore "Tgs.Payment.PrimePay/Tgs.Payment.PrimePay.csproj"
COPY . .
WORKDIR "/src/Tgs.Payment.PrimePay"
RUN dotnet build "Tgs.Payment.PrimePay.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.Payment.PrimePay.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Payment.PrimePay.dll"]