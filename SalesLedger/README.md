# 🎯 Architecture Overview
*This project follows Clean Architecture principles with clear separation*

### Domain Layer: Core business entities (Customers, Products, Orders, OrderItems)
### Application Layer: Service interfaces and implementations, business logic
### Infrastructure Layer: Database access, Entity Framework configurations, repositories

---

## 🔧 Technology Stack

- **.NET 8.0:** *Latest .NET framework*
- **Entity Framework Core 8.0:** *Object-relational mapper*
- **SQL Server:** *Primary database (supports named instances)*
- **GUID-based Primary Keys:** *For distributed systems compatibility*

---

## 📦 Key Features
```
-> Customer management with unique email constraints
-> Product catalog management
-> Order processing with order items
-> Integrated security with connection string encryption
-> Repository pattern for data access
-> Service layer for business logic
-> Entity configurations using Fluent API
```

---

## 🚀 Getting Started
**Prerequisites**
```
-> .NET 8.0 SDK or later
-> SQL Server (2017 or later)
-> Visual Studio 2022 or VS Code
```

---

## 📖 Database Setup

*Configure your connection string using user secrets*

```powershell
dotnet user-secrets set "ConnectionStrings:SalesLedgerDb" "Server=YOUR_SERVER;Database=SalesLedgerDb;Integrated Security=True;TrustServerCertificate=True"
```

*Create the initial migration* **(migrations are not included in the repository)**

```powershell
dotnet ef migrations add InitialCreate
```

*Apply migrations to create the database*

```powershell
dotnet ef database update
```

**Note:** *Migration files are excluded from source control. Each developer must generate their own initial migration based on the domain models.*

---

## 📝 Database Schema

- **Customers:** *Customer information with unique email addresses*
- **Products:** *Product catalog with pricing*
- **Orders:** *Order headers linked to customers*
- **OrderItems:** *Order line items with quantity and pricing*

---

## 🔐 Configuration
Connection strings are managed through:

**Development:** *User Secrets*

**Production:** *Environment variables or secure configuration providers*

---

## 🧪 Development
*Adding a New Migration*
### When you modify domain entities, create a new migration

```powershell
dotnet ef migrations add YourMigrationName
dotnet ef database update
```
