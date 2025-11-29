using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SalesLedger.Interfaces;
using SalesLedger.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesLedger.Services
{
     public class ProductService : IProductService
    {
        private readonly SalesLedgerDbContext _context;

        public ProductService(SalesLedgerDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            if (product == null) 
               throw new ArgumentNullException(nameof(product));
              
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
       }

        public async Task<Product?> UpdateAsync(Product product)
       {
           var existing = await _context.Products.FindAsync(product.ProductId);
           if (existing == null) return null;
         
           _context.Entry(existing).CurrentValues.SetValues(product);
           await _context.SaveChangesAsync();
           return existing;
       }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
