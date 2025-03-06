using Saga.Orc.HttpRepositories.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orc.HttpRepositories;

public class BasketHttpRepository : IBasketHttpRepository
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public BasketHttpRepository(HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CartDto> GetBasket(string username)
    {
        var cart = await _httpClient.GetFromJsonAsync<CartDto>($"baskets/{username}");
        if (cart == null || !cart.Items.Any()) return null;

        return cart;
    }

    public async Task<bool> DeleteBasket(string username)
    {
        var http =  _httpClientFactory.CreateClient($"baskets/{username}");
        var cart = await http.GetFromJsonAsync<CartDto>($"baskets/{username}");
        if (cart == null || !cart.Items.Any()) return false;

        return true;
    }
}