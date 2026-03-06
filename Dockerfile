FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
ENV TZ=America/New_York
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet build --no-restore
RUN dotnet test --no-build

RUN dotnet publish Web --no-restore -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get install -y locales
RUN sed -i '/en_US.UTF-8/s/^# //g' /etc/locale.gen
RUN locale-gen
ENV LANG=en_US.UTF-8
ENV LANGUAGE=en_US:en
ENV LC_ALL=en_US.UTF-8
ENV TZ=America/New_York

WORKDIR /app

COPY --from=builder /app/publish .
ENTRYPOINT [ "dotnet", "Web.dll" ]