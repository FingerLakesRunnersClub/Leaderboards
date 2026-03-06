# FLRC Leaderboards

[![CI](https://github.com/FingerLakesRunnersClub/Leaderboards/actions/workflows/CI.yml/badge.svg)](https://github.com/FingerLakesRunnersClub/Leaderboards/actions/workflows/CI.yml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_Leaderboards&metric=coverage)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_Leaderboards)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_Leaderboards&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_Leaderboards)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_Leaderboards&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_Leaderboards)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_Leaderboards&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_Leaderboards)

This repository contains the code for the various leaderboard apps that the Finger Lakes Runners Club uses:
- [FLRC Challenge](https://challenge.fingerlakesrunners.org)
  (
    [2025 results](https://2025.challenge.fingerlakesrunners.org),
    [2024 results](https://2024.challenge.fingerlakesrunners.org),
    [2023 results](https://2023.challenge.fingerlakesrunners.org),
    [2022 results](https://2022.challenge.fingerlakesrunners.org),
    [2021 results](https://2021.challenge.fingerlakesrunners.org)
  )
- [FLRC Trail Circuit](https://trailcircuit.fingerlakesrunners.org)
  (
    [2025 results](https://2025.trailcircuit.fingerlakesrunners.org),
    [2024 results](https://2024.trailcircuit.fingerlakesrunners.org),
    [2023 results](https://2023.trailcircuit.fingerlakesrunners.org),
    [2022 results](https://2022.trailcircuit.fingerlakesrunners.org),
    [2021 results](https://2021.trailcircuit.fingerlakesrunners.org)
  )
- [FLRC Track Bests](https://track.fingerlakesrunners.org)

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Installation

- Run `dotnet restore` to restore dependencies

## Usage

- Run `dotnet watch --project Web run -- Challenge` to start the Challenge server
  - or `dotnet watch --project Web run -- TrailCircuit` to start the Trail Circuit server
  - or `dotnet watch --project Web run -- Track` to start the Track server

- Browse to `http://localhost:5000`
- Run `dotnet test` to run tests before committing/pushing
