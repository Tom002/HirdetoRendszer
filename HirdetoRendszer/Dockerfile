#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["HirdetoRendszer/HirdetoRendszer.Api.csproj", "HirdetoRendszer/"]
COPY ["HirdetoRendszer.Bll/HirdetoRendszer.Bll.csproj", "HirdetoRendszer.Bll/"]
COPY ["HirdetoRendszer.Dal/HirdetoRendszer.Dal.csproj", "HirdetoRendszer.Dal/"]
COPY ["HirdetoRendszer.Common/HirdetoRendszer.Common.csproj", "HirdetoRendszer.Common/"]
RUN dotnet restore "HirdetoRendszer/HirdetoRendszer.Api.csproj"
COPY . .
WORKDIR "/src/HirdetoRendszer"
RUN dotnet build "HirdetoRendszer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HirdetoRendszer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HirdetoRendszer.Api.dll"]
