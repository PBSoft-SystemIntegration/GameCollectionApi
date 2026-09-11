# GameCollection API – codex/jwt-auth-solution i Docker

Byg og kør fra repositoryets rod på denne branch:

```powershell
docker build -t gamecollectionapi:jwt .
docker run --rm -p 5000:80 gamecollectionapi:jwt
```

- API: http://localhost:5000/api/games
- Scalar: http://localhost:5000/scalar
- OpenAPI: http://localhost:5000/openapi/v1.json

Containeren bruger .NET 10 og intern HTTP-port 80. Skift 5000, hvis værtsporten er optaget.
Scalar aktiveres med ApiDocumentation__Enabled=true. HTTPS-redirect bruges kun uden for containeren.

## Vælg datalager

`docker run` bruger InMemory som standard, så det ikke kræver en ekstern database.
Data nulstilles ved genstart. Den lokale appsettings-fil er uændret.

PostgreSQL med et vedvarende volume:

```powershell
docker compose up --build -d
docker compose down
```

Compose bruger Host=db, starter først API'et efter databasen er klar og eksponerer kun API'et.
Databasen opretter sit skema gennem den eksisterende EnsureCreatedAsync ved API-opstart.
API_PORT kan ændre værtsporten, og POSTGRES_PASSWORD kan erstatte det lokale undervisningspassword.
Compose opretter et navngivet volume. Brug ikke samme databasevolume til forskellige branches med forskellige skemaer.
Start eksempelvis med `docker compose -p gamecollection-api-key up --build -d` og vælg et særskilt projektnavn pr. branch.

SQLite med et vedvarende volume:

```powershell
docker run --rm -p 5000:80 -e Repository__Provider=Sqlite -v gamecollection-sqlite:/data gamecollectionapi:jwt
```

Brug denne branchs image-navn i stedet for gamecollectionapi:jwt. /data er skrivbar for containerens app-bruger.

Authentication er bevaret. Et beskyttet endpoint kan derfor svare 401 uden de nødvendige credentials. Opret en API key gennem API’et og angiv den i Scalar.

JWT bruger branchens eksisterende undervisningskonfiguration. Jwt__Secret, Jwt__Issuer og Jwt__Audience kan overskrives med miljøvariabler ved docker run.
