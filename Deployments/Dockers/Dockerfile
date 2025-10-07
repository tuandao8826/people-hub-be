# ==========================
# 1. Build Stage
# ==========================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution file và restore các project dependencies
COPY ../../Template.sln ./
COPY ../../Src ./Src

RUN dotnet restore ./Src/Api/Api.csproj

# Build ứng dụng
RUN dotnet publish ./Src/Api/Api.csproj -c Release -o /app/out

# ==========================
# 2️. Runtime Stage
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 5442

# Command khởi chạy ứng dụng
ENTRYPOINT ["dotnet", "Api.dll"]