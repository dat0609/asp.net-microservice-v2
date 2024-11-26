using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Shared.SeedWork;
using ILogger = Serilog.ILogger;

namespace Ordering.Application.Features.V1.Orders.Queries.GetOrders;

public class GetOrdersByUserNameQueryHandler : IRequestHandler<GetOrdersByUserNameQuery, List<Order>>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger _logger;
    
    public GetOrdersByUserNameQueryHandler(IOrderRepository repository, ILogger logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private const string MethodName = "GetOrdersQueryHandler";


    public async Task<List<Order>> Handle(GetOrdersByUserNameQuery request, CancellationToken cancellationToken)
    { 
        _logger.Information($"BEGIN: {MethodName} - Username: {request.UserName}");
        var order = await _repository.GetOrdersByUserNameAsync(request.UserName);
        _logger.Information($"END: {MethodName} - Username: {request.UserName}");
        
        return order.ToList();
    }
}