# Windows-контейнеры (только для Windows Server/Windows 10/11 Pro с Hyper-V)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-nanoserver-ltsc2022 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Для Windows используем mkdir без -p
RUN mkdir \app\Data

FROM mcr.microsoft.com/dotnet/sdk:10.0-nanoserver-ltsc2022 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TeacherPortal.Web/TeacherPortal.Web.csproj", "TeacherPortal.Web/"]
RUN dotnet restore "./TeacherPortal.Web/TeacherPortal.Web.csproj"
COPY . .
WORKDIR "/src/TeacherPortal.Web"

# Исправляем синтаксис: %BUILD_CONFIGURATION% -> $env:BUILD_CONFIGURATION (PowerShell)
RUN dotnet build "./TeacherPortal.Web.csproj" -c $env:BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TeacherPortal.Web.csproj" -c $env:BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir \app\Data
ENTRYPOINT ["dotnet", "TeacherPortal.Web.dll"]