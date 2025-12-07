# Infrastructure Layer
*The Infrastructure layer handles all external concerns including data persistence, database configuration, and repository implementations. This layer depends on the Domain and implements interfaces defined in the Application layer.*

## 🎯 Purpose
**The Infrastructure layer:**
```
-> Implements data access using Entity Framework Core
-> Configures entity mappings and relationships
-> Provides concrete repository implementations
-> Handles database connections and transactions
```
---

## 📁 Structure
```
Infrastructure/
├── Data/                # DbContext and design-time factory
├── Configurations/      # Entity Framework Fluent API configurations
├── Repositories/        # Repository pattern implementations
```

---

## ✅ Best Practices

- Separation of Concerns: Keep entity configurations separate from entities
- Eager Loading: Use .Include() for related data to avoid N+1 queries
- Async Operations: All database operations are asynchronous
- Unit of Work: Explicit SaveChangesAsync() calls control transactions
- Repository Pattern: Abstract data access behind interfaces
