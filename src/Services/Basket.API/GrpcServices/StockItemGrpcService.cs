using Inventory.Grpc.Protos;

namespace Basket.API.GrpcServices;

public class StockItemGrpcService
{
    private readonly StockProtoService.StockProtoServiceClient _protoServiceClient;

    public StockItemGrpcService(StockProtoService.StockProtoServiceClient protoServiceClient)
    {
        _protoServiceClient = protoServiceClient;
    }

    public async Task<StockModel> GetStock(string itemNo)
    {
        try
        {
            var request = new GetStockRequest
            {
                ItemNo = itemNo
            };
            var response = await _protoServiceClient.GetStockAsync(request);

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}