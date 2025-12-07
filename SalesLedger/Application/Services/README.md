# Services
*This folder contains concrete implementations of service interfaces, providing business logic and orchestrating operations across multiple repositories.*

---

## 🎯 Purpose
- Coordinating multiple repository operations
- Enforcing business rules and validations
- Managing transactions through Unit of Work
- Translating between domain operations and data access

---

## 📁 Contents
- **CustomerService.cs:** *Customer management implementation*
- **ProductService.cs:** *Product catalog management implementation*
- **OrderService.cs:** *Order processing and line item coordination*
- **OrderItemService.cs:** *Order item operations implementation*

---

## 🔄 Transaction Management
***Services use the Unit of Work pattern for transactions:***

```csharp
// Add entity to context (tracked but not persisted)
await _orderRepository.AddAsync(order, ct);

// Commit all changes in a single transaction
await _orderRepository.UnitOfWork.SaveChangesAsync(ct);
```

---

## ✅ Best Practices

- **Validate dependencies first:** *Check all required entities exist before creating*
- **Use Unit of Work:** *Save changes explicitly after all operations*
- **Return domain entities:** *Don't create DTOs in services (leave that to controllers)*
- **Support cancellation:** *Pass CancellationToken through to repositories*
- **Keep methods focused:** *Each method does one business operation*
- **Use readonly collections:** *Return IReadOnlyCollection<T> to prevent modification*

---
