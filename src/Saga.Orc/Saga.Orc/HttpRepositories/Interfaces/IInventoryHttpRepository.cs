using Shared.DTOs.Inventory;

namespace Saga.Orc.HttpRepositories.Interfaces;

public interface IInventoryHttpRepository
{
    Task<string> CreateSalesOrder(SalesProductDto model);
    Task<string> CreateOrderSale(string orderDocNo, SaleOrderDto model);
    Task<bool> DeleteOrderByDocumentNo(string documentNo);
}