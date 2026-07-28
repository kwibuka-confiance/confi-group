# DevOps and CI/CD

## Local Development

Use Docker Compose for:

- PostgreSQL
- Redis
- Backend
- Supporting services

## CI Pipeline

On pull request:

1. Restore dependencies
2. Build
3. Run formatting checks
4. Run analyzers
5. Run unit tests
6. Run integration tests
7. Scan dependencies
8. Build containers

## CD

Initial environments:

- Development
- Staging
- Production

## Deployment Principles

- Automated migrations with safeguards
- Rollback plan
- Health checks
- Structured logs
- Metrics and traces
- Secrets from secure environment storage
