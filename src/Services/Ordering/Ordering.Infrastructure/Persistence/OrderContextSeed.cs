using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence;

public class OrderContextSeed
{
    private readonly OrderContext _context;

    public OrderContextSeed(OrderContext context)
    {
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (!_context.Orders.Any())
        {
            await _context.Orders.AddRangeAsync(
                new Order
                {
                    DocumentNo = Guid.NewGuid(),
                    UserName = "customer1", FirstName = "customer1", LastName = "customer",
                    EmailAddress = "customer1@local.com",
                    ShippingAddress = "Wollongong", InvoiceAddress = "Australia", TotalPrice = 250
                });
        }
    }
}