using Shared.Enums.Inventory;

namespace Shared.DTOs.Inventory;

public record SalesProductDto(string ExternalDocNo, int Quantity)
{
    public EDocumentType DocumentType { get; set; } = EDocumentType.Sale;
    public string ItemNo { get; set; }

    public void SetItemNo(string itemNo)
    {
        ItemNo = itemNo;
    }
}