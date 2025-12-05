using SalesLedger.Application.Interfaces;
using SalesLedger.Domain;
using SalesLedger.Infrastructure.Repositories;

namespace SalesLedger.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<Customers?> GetByIdAsync(Guid customerId, CancellationToken ct = default)
        => await _repository.GetByIdAsync(customerId, ct);

    public async Task<IReadOnlyCollection<Customers>> GetAllAsync(CancellationToken ct = default)
        => await _repository.GetAllAsync(ct);

    public async Task<Customers> CreateAsync(Customers customer, CancellationToken ct = default)
    {
        await _repository.AddAsync(customer, ct);
        await _repository.UnitOfWork.SaveChangesAsync(ct);
        return customer;
    }

    public async Task UpdateAsync(Customers customer, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(customer.CustomerId, ct)
            ?? throw new KeyNotFoundException($"Customer {customer.CustomerId} not found.");

        existing.FirstName = customer.FirstName;
        existing.LastName = customer.LastName;
        existing.Email = customer.Email;

        await _repository.UnitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid customerId, CancellationToken ct = default)
    {
        var customer = await _repository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException($"Customer {customerId} not found.");

        _repository.Remove(customer);
        await _repository.UnitOfWork.SaveChangesAsync(ct);
    }

    public async Task<Customers?> GetWithOrdersAsync(Guid customerId, CancellationToken ct = default)
        => await _repository.GetWithOrdersAsync(customerId, ct);
}