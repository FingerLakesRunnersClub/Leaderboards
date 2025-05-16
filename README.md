# FLRC Leaderboards

[![CI](https://github.com/FingerLakesRunnersClub/Leaderboards/actions/workflows/CI.yml/badge.svg)](https://github.com/FingerLakesRunnersClub/Leaderboards/actions/workflows/CI.yml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_Leaderboards&metric=coverage)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_Leaderboards)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_Leaderboards&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_Leaderboards)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_Leaderboards&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_Leaderboards)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_Leaderboards&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_Leaderboards)

This repository contains the code for the various leaderboard apps that the Finger Lakes Runners Club uses:
- [FLRC Challenge](https://challenge.fingerlakesrunners.org)
  (
    [2023 results](https://2023.challenge.fingerlakesrunners.org),
    [2022 results](https://2022.challenge.fingerlakesrunners.org),
    [2021 results](https://2021.challenge.fingerlakesrunners.org)
  )
- [FLRC Trail Circuit](https://trailcircuit.fingerlakesrunners.org)
  (
    [2023 results](https://2023.trailcircuit.fingerlakesrunners.org),
    [2022 results](https://2022.trailcircuit.fingerlakesrunners.org),
    [2021 results](https://2021.trailcircuit.fingerlakesrunners.org)
  )

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Installation

- Run `dotnet restore` to restore dependencies

## Usage

- Run `dotnet watch --project Challenge run` to start the Challenge server
  - or `dotnet watch --project TrailCircuit run` to start the Trail Circuit server

- Browse to `http://localhost:5000`
- Run `dotnet test` to run tests before committing/pushing

### Via Docker Compose

- Run `./dcd up -d --build` to start both apps simultaneously
- Browse to `http://localhost:5001` for the Challenge, or `http://localhost:5002` for the Trail Circuit
- Run `docker compose exec dashboard dotnet test` to run tests before committing/pushing

(`dcd` stands for Docker Compose Dev, and is simple Bash script that translates Docker Compose commands to always use the `dev` config file)