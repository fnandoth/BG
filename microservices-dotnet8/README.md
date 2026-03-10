# BG Microservices (.NET 8 + YARP)

Base de microservicios sin lógica de negocio, usando DDD y API mínima para arrancar.

## Servicios incluidos (10)
- IdentityService
- UserProfileService
- PostService
- FeedService
- SocialGraphService
- InteractionService
- NotificationService
- SearchService
- TopicService
- MediaService

## Gateway
- YARP Reverse Proxy en `gateway/BG.Gateway`.

## Estructura DDD por servicio
- `Domain`: Entidades base + contratos de repositorio.
- `Application`: Casos de uso/servicios de aplicación base.
- `Infrastructure`: Implementaciones iniciales (in-memory).
- `Api`: Endpoints health y placeholder.

## Ejecutar con Docker
```bash
docker compose up --build
```

Gateway disponible en `http://localhost:8080`.
