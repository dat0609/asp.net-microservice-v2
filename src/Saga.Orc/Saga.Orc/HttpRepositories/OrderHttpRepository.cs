using Infrastructure.Extensions;
using Saga.Orc.HttpRepositories.Interfaces;
using Shared.DTOs.Order;

namespace Saga.Orc.HttpRepositories;

public class OrderHttpRepository : IOrderHttpRepository
{
    private readonly HttpClient _httpClient;

    public OrderHttpRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<long> CreateOrder(CreateOrderDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("orders", request);
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode) return -1;
        
        var id = await response.ReadContentAs<long>();
         
        return id;
    }

    public async Task<OrderDto> GetOrderById(long orderId)
    {
        var response = await _httpClient.GetFromJsonAsync<OrderDto>($"orders/{orderId.ToString()}");
        return response;
    }

    public async Task<bool> DeleteOrderById(long orderId)
    {
        var response = await _httpClient.DeleteAsync($"orders/{orderId.ToString()}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteOrderByDocumentNo(string documentNo)
    {
        var response = await _httpClient.DeleteAsync($"document-no/{documentNo}");
        return response.IsSuccessStatusCode;
    }
}