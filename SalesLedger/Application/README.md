# Application Layer
*The Application layer contains the business logic and orchestrates domain operations. It defines interfaces (contracts) and their implementations (services) that coordinate between the domain and infrastructure layers.*

---

## 📁 Structure
```
Application/
├── Interfaces/      # Service and repository interface definitions
└── Services/        # Business logic service implementations
```
---

## 🎯 Purpose

- Defines service contracts through interfaces
- Implements business logic and workflows
- Orchestrates operations across multiple repositories
- Validates business rules before data persistence
- Provides a clean API for the presentation layer

---

## 🔑 Key Concepts

- Application layer depends on Domain entities
- Application layer defines repository interfaces (Dependency Inversion)
- Infrastructure layer implements these interfaces

**Services in this layer handle:**

- Complex business operations
- Multi-entity coordination
- Transaction management via Unit of Work
- Data validation and business rule enforcement

---

## 📋 Services Overview

- ICustomerService / CustomerService: *Customer management operations*
- IProductService / ProductService: *Product catalog management*
- IOrderService / OrderService: *Order processing and orchestrater*
- IOrderItemService / OrderItemService: *many‑to‑many join entity operations*

---

# 🔗 Dependencies

- **Domain:** References domain entities (Customers, Products, Orders, OrderItem)
- **Infrastructure:** Service implementations depend on repository interfaces

---
