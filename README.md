# TrustGate

**Identity, User, Access & Entitlement Management - built as a reusable Trust Shared Service**

TrustGate is a workflow-driven access management platform: users request entitlements, authorized approvers review and decide through a controlled workflow, access is provisioned atomically with the decision, and every action produces tamper-evident audit and evidence records. It is designed as a platform capability - secure, API-first, versioned, observable, and deployable through a controlled pipeline - not a one-off IAM app.

The project doubles as a learning artifact: it was built in ten deliberate stages, starting from a naive single-project implementation and refactoring toward production grade. The git history (one branch/PR per part) shows each engineering discipline being introduced and why.

---

## What it does

- **Access requests** - users submit requests for entitlements (system + permission, risk-rated) with a mandatory business justification
- **Workflow engine** - an explicit, declarative state machine: `Draft → Submitted → UnderReview → Approved/Rejected → Closed`, with `Escalated` (automatic on SLA breach) and `Exception → Remediation` paths; illegal transitions are impossible by construction
- **Controls in the domain** - segregation of duties (no self-approval), mandatory rejection reasons, no duplicate grants, no grants to inactive users - enforced in the domain model, backed by HTTP authorization, proven by tests
- **Provisioning with provenance** - every entitlement a user holds links back to the request, approver, justification, and timestamps that granted it
- **Audit & evidence** - append-only, hash-chained audit stream (who/what/when/why/outcome, including denied attempts); on-demand evidence packs per request and per-user access-review views
- **AuthN/AuthZ** - JWT authentication, fine-grained permission-based authorization (`requests.approve`, `evidence.read`, …) - the platform's own security model _is_ an entitlement model
- **Observability** - structured JSON logging (Serilog), end-to-end correlation IDs (UI → API → DB → background jobs → audit), liveness/readiness health checks, OpenTelemetry metrics, CloudWatch alarms, runbook

## Architecture

```
TrustGate.Domain/          Entities, workflow state machine, domain rules, domain events. Zero dependencies.
TrustGate.Application/     Use cases (services) and ports (repository/UoW interfaces)
TrustGate.Infrastructure/  EF Core (SQL Server), auditing interceptor, AWS adapters
TrustGate.Api/             Versioned REST API (v1/v2), auth, middleware, DI wiring
trustgate-ui/              Angular 16 - permission-aware UI, generated API client, evidence viewer
```

Dependency rule: everything points inward; the domain references nothing. HTTP, the SLA escalation worker, and future queue consumers all drive the same use cases - one rule set, many entry points.

**Runtime (AWS, af-south-1):** ALB (HTTPS) → ECS Fargate (2+ tasks) → RDS SQL Server in private subnets · Secrets Manager (task-role scoped) · CloudWatch Logs/Metrics/Alarms · Angular on S3 + CloudFront.

## Tech stack

| Layer    | Technology                                                                                                             |
| -------- | ---------------------------------------------------------------------------------------------------------------------- |
| Backend  | .NET 10, ASP.NET Core, EF Core, FluentValidation, Asp.Versioning, Serilog, OpenTelemetry                               |
| Frontend | Angular 16 (standalone components, signals), OpenAPI-generated client                                                  |
| Data     | SQL Server (AWS RDS)                                                                                                   |
| Cloud    | ECS Fargate, ALB, ECR, RDS, Secrets Manager, CloudWatch, S3 + CloudFront, IAM (OIDC deploy role)                       |
| Pipeline | GitHub Actions - build, tests + coverage gate, dependency audit, CodeQL, Gitleaks, Trivy image scan, gated prod deploy |
| Testing  | xUnit, FluentAssertions, NSubstitute, WebApplicationFactory, Testcontainers (MsSql)                                    |

## API at a glance

```
POST   /api/v1/auth/login                      → JWT (15-min, permission claims)
POST   /api/v1/requests                        → submit access request
POST   /api/v1/requests/{id}/actions           → { action: StartReview|Approve|Reject|Escalate|RaiseException|Remediate|Close, comment }
GET    /api/v1/requests?status=&page=          → paged list
GET    /api/v1/requests/{id}/evidence          → evidence pack (timeline, actors, outcome)
GET    /api/v1/users/{id}/access-evidence      → access review: every grant with its provenance
GET    /health/live | /health/ready            → orchestrator / load-balancer probes
```

Full contract: Swagger UI at `/swagger`, and `openapi-v1.json` is exported as a CI artifact on every build (with a diff-against-baseline gate - breaking changes can't ship silently).

## Running locally

Prereqs: .NET 10 SDK, Docker, Node 18+.

```bash
# 1. Database
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<your-local-pw>" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# 2. Secrets (never in appsettings)
cd TrustGate.Api
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Database=TrustGate;User Id=sa;Password=<your-local-pw>;TrustServerCertificate=True;"
dotnet user-secrets set "Jwt:SigningKey" "$(openssl rand -base64 48)"

# 3. Migrate + run API
dotnet ef database update --project ../TrustGate.Infrastructure
dotnet run                                # https://localhost:5001/swagger

# 4. UI
cd ../trustgate-ui && npm ci && npm start # http://localhost:4200
```

Tests: `dotnet test` (integration tests spin up a throwaway SQL Server via Testcontainers - Docker must be running).

## Engineering notes

- `SECURITY-CONTROLS.md` - every control mapped to its threat, code location, proving test, and emitted evidence
- `RUNBOOK.md` - symptom → signal → mitigation, one entry per observable failure mode
- `docs/book/` - the ten-part build narrative (Parts 1–10), from naive build to production grade
- Regression policy: every defect gets a failing test (named with the ticket ID) before the fix

## Roadmap

- Two-level approval for High-risk entitlements (second-approver state)
- Refresh-token rotation (httpOnly cookie, hashed at rest, single-use)
- Scheduled access-review campaigns with attestation workflow
- Evidence pack export to PDF
- SCIM-style provisioning connectors to downstream systems
