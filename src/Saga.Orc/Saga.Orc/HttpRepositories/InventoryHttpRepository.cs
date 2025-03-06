using Infrastructure.Extensions;
using Saga.Orc.HttpRepositories.Interfaces;
using Shared.DTOs.Inventory;

namespace Saga.Orc.HttpRepositories;

public class InventoryHttpRepository : IInventoryHttpRepository
{
    private readonly HttpClient _httpClient;

    public InventoryHttpRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CreateSalesOrder(SalesProductDto model)
    {
        var response = await _httpClient.PostAsJsonAsync($"inventory/sales/{model.ItemNo}", model);
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        {
            throw new Exception($"Create sales order failed" + model.ItemNo);
        }

        var inventory = await response.ReadContentAs<InventoryEntryDto>();
        return inventory.DocumentNo;
    }

    public async Task<bool> DeleteOrderByDocumentNo(string documentNo)
    {
        var response = await _httpClient.DeleteAsync($"inventory/document-no/{documentNo}");
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        {
            throw new Exception($"Delete failed" + documentNo);
        }

        return true;
    }
}