using Shared.DTOs.Order;

namespace Saga.Orc.HttpRepositories.Interfaces;

public interface IOrderHttpRepository
{
    Task<long> CreateOrder(CreateOrderDto request);
    Task<OrderDto> GetOrderById(long orderId);
    Task<bool> DeleteOrderById(long orderId);
    Task<bool> DeleteOrderByDocumentNo(string documentNo);
}