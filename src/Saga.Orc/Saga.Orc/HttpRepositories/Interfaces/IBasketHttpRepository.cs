using Shared.DTOs.Basket;

namespace Saga.Orc.HttpRepositories.Interfaces;

public interface IBasketHttpRepository
{
    Task<CartDto> GetBasket(string username);
    Task<bool> DeleteBasket(string username);
}