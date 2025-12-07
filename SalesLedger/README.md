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

---

## 💻 CRUD Operations by Entity

| Entity         | Create | Read   | Update                  | Delete                  | Notes / Current Status                              | Severity |
|----------------|--------|--------|-------------------------|-------------------------|-----------------------------------------------------|----------|
| **Customer**   | Yes    | Yes    | Yes Full (all fields)   | Yes Cascade            | Fully implemented & granular                        | Low      |
| **Product**    | Yes    | Yes    | Yes Full (Name + Price) | Yes Safe (Restrict if in orders) | Fully implemented                                   | Low      |
| **Order**      | Yes    | Yes    | No updates allowed   | No Soft-delete only     | Immutable by design – once placed, order cannot be modified | High     |
| **OrderItem**  | Yes (with Order) | Yes | No Disabled by design   | No Disabled by design   | Created together with Order, never changed afterward | Low      |

#### Key
- Yes = Fully supported and granular  
- No = Intentionally not allowed (business rule)  Missing / not yet implemented  
- Severity = How big a problem it would be if we suddenly needed that operation tomorrow
