# FLRC 2021 Club Challenge Dashboard

Throughout 2021, the Finger Lakes Runners Club Challenge will open 10 race courses to local runners to test their mettle against one another and as part of 10-year age-group-based teams, all while staying safe from the coronavirus. Awards and bragging rights won’t just go to the swift, but also to the consistent, the persistent, and the clever.

## Running locally

- Install the [.NET SDK v5.0.100 or greater](https://dotnet.microsoft.com/download/dotnet/5.0)
- Run `dotnet watch --project ChallengeDashboard run` to start the server
- Browse to `http://localhost:5000`
- Run `dotnet test` to run tests before committing/pushing

## Running via Docker

- Run `./dcd up` (where `dcd` is short for Docker Compose Dev, a simple Bash script that translates Docker Compose commands to always use )
- Browse to `http://localhost:5000`
- Run `docker-compose exec dashboard dotnet test` to run tests before committing/pushing