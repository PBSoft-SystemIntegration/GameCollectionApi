# GameCollection API – ServiceLayer i Docker

Byg og kør fra repositoryets rod på denne branch:

```powershell
docker build -t gamecollectionapi:service-layer .
docker run --rm -p 5000:80 gamecollectionapi:service-layer
```

- API: http://localhost:5000/api/games
- Scalar: http://localhost:5000/scalar
- OpenAPI: http://localhost:5000/openapi/v1.json

Containeren bruger .NET 10 og intern HTTP-port 80. Skift 5000, hvis værtsporten er optaget.
Scalar aktiveres med ApiDocumentation__Enabled=true. HTTPS-redirect bruges kun uden for containeren.

Denne branch gemmer data i hukommelsen. Data nulstilles ved genstart.
