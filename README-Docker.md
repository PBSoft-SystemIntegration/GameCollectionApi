# GameCollection API – ApiKey i Docker

Byg og kør fra repositoryets rod på denne branch:

```powershell
docker build -t gamecollectionapi:api-key .
docker run --rm -p 5000:80 gamecollectionapi:api-key
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

Start databasen først:

```powershell
docker compose up -d db
docker compose logs -f db
```

Vent, til den afsluttende opstart skriver `database system is ready to accept connections`.
Ved første opstart initialiserer PostgreSQL først databasen med en midlertidig server;
vent til initialiseringen og den efterfølgende genstart er færdig.
Tryk Ctrl+C for at afslutte logvisningen. Databasen fortsætter med at køre.
Start derefter API'et:

```powershell
docker compose up --build -d api
```

Stop begge containere med `docker compose down`.

Compose bruger Host=db og eksponerer kun API'et. `depends_on` styrer rækkefølgen,
men venter ikke på, at PostgreSQL kan modtage forbindelser.
Hvis API'et blev startet for tidligt, kan det startes igen med `docker compose restart api`, når databasen er klar.
Databasen opretter sit skema gennem den eksisterende EnsureCreatedAsync ved API-opstart.
API_PORT kan ændre værtsporten, og POSTGRES_PASSWORD kan erstatte det lokale undervisningspassword.
Compose opretter et navngivet volume. Brug ikke samme databasevolume til forskellige branches med forskellige skemaer.
Vælg eventuelt et særskilt projektnavn pr. branch med `-p`, fx `docker compose -p gamecollection-api-key up -d db`. Brug samme `-p` i alle efterfølgende kommandoer for den branch.

SQLite med et vedvarende volume:

```powershell
docker run --rm -p 5000:80 -e Repository__Provider=Sqlite -v gamecollection-sqlite:/data gamecollectionapi:api-key
```

Brug denne branchs image-navn i stedet for gamecollectionapi:api-key. /data er skrivbar for containerens app-bruger.

Authentication er bevaret. Et beskyttet endpoint kan derfor svare 401 uden de nødvendige credentials. Opret en API key gennem API’et og angiv den i Scalar.
