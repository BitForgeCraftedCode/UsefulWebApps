# Database Migration & Framework Upgrade Policy

## Scope

This project uses: - EF Core → Identity system only - Dapper → all
business data - MySQL → persistent storage - ASP.NET Core → web
framework - Identity → authentication/authorization

The Identity database schema is treated as infrastructure, not
application data.

## Core Principles

1.  Framework upgrades do NOT require migrations.
2.  Migrations are for schema changes only.
3.  Identity schema is infrastructure.

## Forbidden Actions

-   Running migrations after framework upgrades
-   Auto-running migrations on startup
-   Sync/version migrations
-   Production schema changes without backup

## Allowed Actions

-   Migrations for real schema changes
-   Intentional Identity schema extensions
-   Explicit migration reviews
-   Manual migration application

## Upgrade Workflow

Framework upgrade: 1. Upgrade .NET / EF Core / Identity 2. Build 3. Test
4. Deploy 5. Do NOT run migrations

Schema change workflow: 1. Modify model 2. Generate migration 3. Review
migration 4. Backup DB 5. Apply locally 6. Apply staging 7. Apply
production

## Philosophy

Frameworks evolve faster than databases. Databases should be stable.
Schema should be intentional. Migrations should be rare. Upgrades should
be boring.
