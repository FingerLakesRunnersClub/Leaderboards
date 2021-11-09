# 2021 FLRC Challenge Dashboard

[![CI](https://github.com/FingerLakesRunnersClub/ChallengeDashboard/actions/workflows/CI.yml/badge.svg)](https://github.com/FingerLakesRunnersClub/ChallengeDashboard/actions/workflows/CI.yml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_ChallengeDashboard&metric=coverage)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_ChallengeDashboard)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_ChallengeDashboard&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_ChallengeDashboard)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_ChallengeDashboard&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_ChallengeDashboard)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=FingerLakesRunnersClub_ChallengeDashboard&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=FingerLakesRunnersClub_ChallengeDashboard)

Throughout 2021, the Finger Lakes Runners Club Challenge will open 10 race courses to local runners to test their mettle against one another and as part of 10-year age-group-based teams, all while staying safe from the coronavirus. Awards and bragging rights won’t just go to the swift, but also to the consistent, the persistent, and the clever.

## Requirements

- [.NET SDK v5.0.100 or greater](https://dotnet.microsoft.com/download/dotnet/5.0)

## Installation

- Run `dotnet restore` to restore dependencies

## Usage

- Run `dotnet watch --project ChallengeDashboard run` to start the server
- Browse to `http://localhost:5000`
- Run `dotnet test` to run tests before committing/pushing

### Via Docker Compose

- Run `./dcd up` (where `dcd` is short for Docker Compose Dev, a simple Bash script that translates Docker Compose commands to always use the `dev` config file)
- Browse to `http://localhost:5000`
- Run `docker-compose exec dashboard dotnet test` to run tests before committing/pushing