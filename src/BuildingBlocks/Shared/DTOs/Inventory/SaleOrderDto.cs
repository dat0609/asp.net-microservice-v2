namespace Shared.DTOs.Inventory;

public class SaleOrderDto
{
    public string OrderDocNo { get; set; }
    public List<SaleItemDto> SaleItems { get; set; }
}