# SEP3 Chat System — Architecture Overview

## Summary
This document describes the SEP3 chat system architecture. The design follows a 3-tier split: Presentation, Application, and Data tiers. It uses synchronous REST and asynchronous Kafka for events, plus search/analytics with Elasticsearch.
 Databases used: PostgreSQL (primary relational store) and MongoDB (document store for chat history / denormalized data). The Data Access Layer is implemented in .NET with Entity Framework and exposes a REST facade for the Application Tier.

## High-level 3-tier mapping
- Presentation Tier
  - Technology: Blazor (C#)
  - Role: UI, client-side logic, REST client for requests to the Application Tier. For freshness, simple REST polling can be used where needed.
  - Protocols: REST (primary sync)

- Application Tier
  - Technology: Java + Spring Boot
  - Role: Business logic, orchestrates calls to Data Tier, publishes events to Kafka, integrates with Elasticsearch for search indexing.
  - Protocols: REST (with Presentation & Data Tiers), Kafka (for async & event-driven flows), HTTP for integrations.

- Data Tier
  - Technology: .NET-based Data Access Layer (Entity Framework) + PostgreSQL primary DB
  - Role: Data persistence layer and domain repositories accessed via REST endpoints consumed by the Application Tier.
  - Protocols: REST (Application -> Data), EF Core (internal DB access to PostgreSQL), direct drivers for MongoDB where used by background indexing processes.

## Additional components and responsibilities
- Kafka
  - Use-case: high-throughput event streaming, analytics pipeline, connecting to Elasticsearch indexing and other consumers.
- Elasticsearch
  - Use-case: full-text search and fast retrieval for chat searching and analytics. Indexing consumers read events from Kafka (or the app directly) and push to ES.
- MongoDB
  - Use-case: flexible document storage for chat transcripts, user session snapshots, or denormalized read models.

## Communication Channels (detailed)
- Presentation ⇄ Application
  - REST (synchronous): Blazor calls Spring Boot REST endpoints for commands and queries. Typed contracts via OpenAPI.
- Application ⇄ Data
  - REST: Application Tier calls REST endpoints exposed by the .NET Data Access Layer. The DAL translates requests into EF Core operations against PostgreSQL.
- Asynchronous
  - Kafka: event streaming for analytics, indexing, and audit. Kafka consumers (indexers) feed Elasticsearch and MongoDB when required.
- Database interactions
  - Entity Framework (EF Core): used by the Data Access Layer to manage PostgreSQL schema, migrations, and CRUD.
  - MongoDB driver: used by indexing services or background workers for storing denormalized data.

## Sequence / Data Flow (example wire-up)
1. A user sends a message from the Blazor UI.
2. Blazor client calls a Spring Boot REST endpoint `POST /api/messages`.
3. Spring Boot validates and applies business rules, then calls the Data Tier REST endpoint `POST /messages` to persist via EF Core -> PostgreSQL.
4. After persisting, the Application Tier publishes an event to Kafka describing the new message.
5. Kafka consumer(s):
  - Indexer takes the event and updates Elasticsearch for search.
  - An archival consumer writes a copy to MongoDB for transcript retrieval.
6. Clients fetch updates via REST (polling or ETag/If-Modified-Since patterns as needed).

## Design contract (short)
- Inputs: REST requests from Blazor, REST calls from Application Tier to Data Tier, messages from Kafka.
- Outputs: HTTP responses to clients, events published to Kafka, updates inserted into PostgreSQL/MongoDB, indices in Elasticsearch.
- Error modes: network failures, consumer lag, partial failures during multi-step flows (persist->publish->notify). Use idempotency, retries, and dead-letter queues.

## Edge cases & how to handle them
- Partial failure during message handling (persist succeeded, publish failed): persist is authoritative. Use a retry mechanism or persistent outbox pattern (publish from DB transaction) to guarantee eventual publish.
- Duplicate events: consumers must be idempotent (e.g., using message IDs).
- Backpressure / consumer lag: monitor Kafka consumer lag, autoscale consumers.
- Schema evolution (REST): use versioning, additive changes, and API versioning for REST.

## Deployment notes
- Each tier can be independently deployed and scaled. Typical mapping:
  - Presentation: Blazor Server / WASM hosting.
  - Application: multiple Spring Boot instances behind load balancer.
  - Data: Data Access Layer (.NET service) behind internal load balancer; PostgreSQL as cluster.
  - Messaging infra: standalone Kafka cluster
  - Search: Elasticsearch cluster for index and query nodes.
  - MongoDB: replica set for availability.
- Secure channels: TLS for HTTP, ACLs for Kafka.
