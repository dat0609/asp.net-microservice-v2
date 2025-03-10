using Shared.Enums.Inventory;

namespace Shared.DTOs.Inventory;

public class SaleItemDto
{
    public EDocumentType Type { get; set; } = EDocumentType.Sale;
    public int Quantity { get; set; }
    public string ItemNo { get; set; }
}