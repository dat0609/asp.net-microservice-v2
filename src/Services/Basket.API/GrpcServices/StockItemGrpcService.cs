using Grpc.Core;
using Inventory.Grpc.Protos;
using Polly;
using Polly.Retry;

namespace Basket.API.GrpcServices;

public class StockItemGrpcService
{
    private readonly StockProtoService.StockProtoServiceClient _protoServiceClient;
    private readonly AsyncRetryPolicy<StockModel> _retryPolicy;

    public StockItemGrpcService(StockProtoService.StockProtoServiceClient protoServiceClient)
    {
        _protoServiceClient = protoServiceClient;
        _retryPolicy = Policy<StockModel>.Handle<RpcException>()
            .RetryAsync(3);
    }

    public async Task<StockModel> GetStock(string itemNo)
    {
        try
        {
            var request = new GetStockRequest
            {
                ItemNo = itemNo
            };

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _protoServiceClient.GetStockAsync(request);
                if (response != null)
                {
                    
                }
                return response;
            });
        }
        catch (Exception e)
        {
            Console.WriteLine("Errorrrrrrrrrrrrrr");
            return new StockModel();
        }
    }
}