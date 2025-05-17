# Contacts Application User Manual

This document provides instructions to set up and run the full-stack Contacts application with .NET backend, PostgreSQL database, and two frontend clients (React & Vanilla JS).

## Table of Contents
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Accessing Applications](#accessing-applications)

## Prerequisites

- Docker & Docker Compose
- .NET SDK 9.0 (for backend development)
- Node.js v18+ & pnpm (for frontend development)
- (Optional) Scalar for API documentation

## Getting Started

### 1. Environment Setup
Create `.env` file in project root:
```bash
POSTGRES_USER=contacts_admin
POSTGRES_PASSWORD=strongpassword123
POSTGRES_DB=contacts_db
CONTRACTS_CONNECTION="Host=contacts-postgres;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};"
ASPNETCORE_ENVIRONMENT=Development
```

### 2. Start Services
```bash
# Build and start all containers
docker-compose up --build

# Or start specific services
docker-compose up contacts-postgres contacts-api
docker-compose up react-web-client
docker-compose up vanilla-web-client
```

## Accessing Applications

| Service              | URL                              | Port  |
|----------------------|----------------------------------|-------|
| React Client         | http://localhost:8000           | 8000  |
| Vanilla JS Client    | http://localhost:3500           | 3500  |
| Backend API          | http://localhost:5171           | 5171  |
| API Documentation    | http://localhost:5171/scalar    | -     |
| PostgreSQL Database  | postgres://contacts-postgres:5432 | 5432  |

## Troubleshooting

1. **Database Connection Issues**
    - Verify `.env` values match in backend and database services
    - Check container logs: `docker-compose logs contacts-postgres`