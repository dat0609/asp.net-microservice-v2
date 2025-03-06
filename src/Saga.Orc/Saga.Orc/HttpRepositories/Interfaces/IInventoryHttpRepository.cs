using Shared.DTOs.Inventory;

namespace Saga.Orc.HttpRepositories.Interfaces;

public interface IInventoryHttpRepository
{
    Task<string> CreateSalesOrder(SalesProductDto model);
    Task<bool> DeleteOrderByDocumentNo(string documentNo);
}