# -----------------------
# BASE RUNTIME
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# -----------------------
# BUILD STAGE
# -----------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["WebApplication1/WebApplication1.csproj", "WebApplication1/"]
RUN dotnet restore "WebApplication1/WebApplication1.csproj"

COPY . .
WORKDIR "/src/WebApplication1"

RUN dotnet build "WebApplication1.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -----------------------
# PUBLISH
# -----------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApplication1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -----------------------
# FINAL IMAGE
# -----------------------
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

# ✅ IMPORTANT FOR RENDER
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "WebApplication1.dll"]