# DBアクセスAPI。EF Core / Oracle 直結。Sample.sln 非参加の完全独立サーバー。
# 設定キー ConnectionStrings:Bom(= Oracle 接続文字列)を読む。
# 形は baseline の Dockerfile と同じ。待受ポートだけ 5136(Sample.Web の 8080 と重複しない)。

# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Sample.Api/Sample.Api.csproj -c Release -o /app

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:5136
EXPOSE 5136
ENTRYPOINT ["dotnet", "Sample.Api.dll"]
