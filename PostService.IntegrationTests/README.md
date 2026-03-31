# PostService Integration Tests

Suite de pruebas de integracion para `PostService` usando:

- `xUnit` para la ejecucion de pruebas.
- `Bogus` para sembrar usuarios y posts de prueba.
- `Respawn` para limpiar `user_db`, `post_db` y `notification_db` entre pruebas.
- `docker-compose` como entorno real de APIs, PostgreSQL y RabbitMQ.

## Requisitos

1. Levantar la infraestructura:

```powershell
docker compose up -d --build
```

2. Ejecutar la suite:

```powershell
dotnet test .\PostService.IntegrationTests\PostService.IntegrationTests.csproj
```

## Puertos usados por la suite

- `UserService`: `http://localhost:5050`
- `PostService`: `http://localhost:5137`
- `NotificationService`: `http://localhost:5281`
- `user_db`: `localhost:5433`
- `post_db`: `localhost:5434`
- `notification_db`: `localhost:5435`

## Notas

- Las contraseñas de PostgreSQL se toman desde el archivo raiz `.env`.
- Las pruebas obtienen tokens reales llamando a `UserService` despues de sembrar usuarios con Bogus.
- Las pruebas de eventos validan el flujo `PostService -> RabbitMQ -> NotificationService`.
