namespace Saga.Orc.OrderManager;

public class OrderResponse
{
    public bool Success { get; set; }
    
    public OrderResponse(bool success)
    {
        Success = success;
    }
}