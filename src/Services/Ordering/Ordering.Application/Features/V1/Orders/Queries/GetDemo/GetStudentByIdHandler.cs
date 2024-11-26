using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.V1.Orders.Queries.GetDemo;

public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, Order>
{
    private readonly IOrderRepository _orderRepository;

    public GetStudentByIdHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await _orderRepository.GetByIdAsync(request.Id);
        return a;
    }
}