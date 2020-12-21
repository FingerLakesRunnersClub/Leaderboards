FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder
ENV COMPlus_EnableDiagnostics=0 
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet build --no-restore
RUN dotnet test --no-build

RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=builder /app/publish .
ENTRYPOINT ["dotnet", "ChallengeDashboard.dll"]