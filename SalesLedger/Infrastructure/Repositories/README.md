# Repositories
*This folder contains repository pattern implementations that abstract data access operations. Repositories provide a collection-like interface for working with domain entities while encapsulating EF Core specifics.*

---

## 🎯 Purpose
```
-> Abstract database operations behind clean interfaces
-> Provide entity-specific query methods
-> Enable unit testing through interface mocking
-> Centralize data access logic
-> Support the Unit of Work pattern
```

---

## 📁 Contents

### Repository Interfaces

- **IRepository.cs:** *Base repository interface with generic CRUD*
- **IUnitOfWork:** *Transaction Management*
- **ICustomerRepository.cs**
- **IProductRepository.cs**
- **IOrderRepository.cs**


### Repository Implementations

- **Repository.cs:** *Base repository implementation*
- **CustomerRepository.cs:** *Customer repository with related data loading*
- **ProductRepository.cs:** *Product repository implementation*
- **OrderRepository.cs:** *Order repository with items loading*

---

## ✅ Best Practices

- **Virtual Methods:** *Base methods are virtual for override flexibility*
- **Protected Context:** *_context is protected for derived class access*
- **Async All the Way:** *All I/O operations are async*
- **Cancellation Support:** *CancellationToken on async methods*
- **Explicit Loading:** *Use .Include() when related data is needed*
- **Read-Only Collections:** *Return IReadOnlyCollection<T> from queries*
- **No SaveChanges in Repos:** *Let services control transactions*

