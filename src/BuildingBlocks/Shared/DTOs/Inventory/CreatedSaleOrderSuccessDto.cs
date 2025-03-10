namespace Shared.DTOs.Inventory;

public class CreatedSaleOrderSuccessDto
{
    public CreatedSaleOrderSuccessDto(string docNo)
    {
        DocNo = docNo;
    }

    public string DocNo { get; set; }
}