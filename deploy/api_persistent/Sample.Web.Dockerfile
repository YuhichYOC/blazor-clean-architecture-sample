# Blazor ホスト。IBomDataAccess は HttpClient 実装で、DB は一切触らない。
# 設定キー ConnectionStrings:BomApi(= DBアクセスAPI の URL)を読む。
# baseline の Sample.Web.Dockerfile と同形(待受ポートも 8080 のまま)。

# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Sample.Web/Sample.Web.csproj -c Release -o /app

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sample.Web.dll"]
