using Lab6TestTask.Data;
using Lab6TestTask.Enums;
using Lab6TestTask.Models;
using Lab6TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Lab6TestTask.Services.Implementations;

/// <summary>
/// WarehouseService.
/// Implement methods here.
/// </summary>
public class WarehouseService : IWarehouseService
{
    private readonly ApplicationDbContext _dbContext;

    public WarehouseService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Warehouse> GetWarehouseAsync()
    {
        var warehouseId = await _dbContext.Warehouses
       .Select(w => new
       {
           w.WarehouseId,
           TotalValue = w.Products
               .Where(p => p.Status == ProductStatus.ReadyForDistribution)
               .Sum(p => p.Quantity * p.Price)
       })
       .OrderByDescending(x => x.TotalValue)
       .Select(x => x.WarehouseId)
       .FirstOrDefaultAsync();

        return await _dbContext.Warehouses
            .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId);
    }

    public async Task<IEnumerable<Warehouse>> GetWarehousesAsync()
    {
        return await _dbContext.Warehouses
        .Where(w => w.Products.Any(p =>p.ReceivedDate >= new DateTime(2025, 4, 1) &&p.ReceivedDate <= new DateTime(2025, 6, 30))).AsNoTracking()
         .ToListAsync();
    }
}
