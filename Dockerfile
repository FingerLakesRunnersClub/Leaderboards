FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder
ENV TZ=America/New_York
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet build --no-restore
RUN dotnet test --no-build

RUN dotnet publish --no-restore -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
ENV TZ=America/New_York
WORKDIR /app
COPY --from=builder /app/publish .
ENTRYPOINT ["dotnet", "ChallengeDashboard.dll"]