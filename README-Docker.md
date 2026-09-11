# GameCollection API – FullCrud i Docker

API til REST-konsolklientøvelsen. Data ligger i hukommelsen og nulstilles, når containeren genstartes.

## Kør fra Docker Hub

```powershell
docker run --rm --name gamecollectionapi -p 5000:80 emilstephansen/gamecollectionapi:fullcrud-2026
```

- API: http://localhost:5000/api/games
- Scalar: http://localhost:5000/scalar
- OpenAPI: http://localhost:5000/openapi/v1.json

Konsolklientens BaseAddress skal være `http://localhost:5000/api/games/`.
Containeren lytter på HTTP-port 80. Skift venstre side af `5000:80`, hvis en anden port ønskes.
Slides med `/swagger/index.html` skal opdateres til `/scalar` for denne version.

## Byg lokalt

Kør fra repositoryets rod på `FullCrud`:

```powershell
docker build -t emilstephansen/gamecollectionapi:fullcrud-2026 .
```

## Udgiv

```powershell
docker login
docker push emilstephansen/gamecollectionapi:fullcrud-2026
```

Hvis det eksisterende image-navn uden tag i slides også skal pege på denne version:

```powershell
docker tag emilstephansen/gamecollectionapi:fullcrud-2026 emilstephansen/gamecollectionapi:latest
docker push emilstephansen/gamecollectionapi:latest
```

`latest` erstatter dermed standardversionen, som eksisterende øvelser henter. Brug det eksplicitte `fullcrud-2026`-tag til dette års øvelse.

## Kontrakt

| Metode | Route | Succes |
|---|---|---|
| GET | /api/games | 200 med liste |
| GET | /api/games/{id} | 200 med spil |
| POST | /api/games | 201 med spil og Location |
| PUT | /api/games/{id} | 204 |
| PATCH | /api/games/{id} | 204 |
| DELETE | /api/games/{id} | 204 |

POST og PUT modtager `title`, `genre`, `releaseYear`. PATCH modtager de felter, som skal ændres. Ukendt id giver 404, og ugyldige requests giver 400.

Imaget bruger .NET 10 og kører som den indbyggede app-bruger. Scalar/OpenAPI aktiveres med `ApiDocumentation__Enabled=true`. HTTP bruges direkte i containeren uden HTTPS-redirect.
