using Shared.DTOs.Basket;

namespace Saga.Orc.Services.Interfaces;

public interface ICheckoutSagaService
{
    Task<bool> CheckoutOrder(string username, BasketCheckoutDto dto);
}