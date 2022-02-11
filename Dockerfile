FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
ENV TZ=America/New_York
WORKDIR /app
ARG app
ENV app_name=$app

COPY . ./
RUN dotnet restore
RUN dotnet build --no-restore
RUN dotnet test --no-build

RUN dotnet publish $app --no-restore -c Release -o publish
RUN mv publish/wwwroot/_content/Web/* publish/wwwroot

FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV TZ=America/New_York
WORKDIR /app
ARG app
ENV app_name=$app

COPY --from=builder /app/publish .
ENTRYPOINT dotnet $app_name.dll