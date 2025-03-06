using MediatR;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.V1.Orders.Commands.DeleteOrderByDocNo;

public class DeleteOrderByDocNoCommandHandler : IRequestHandler<DeleteOrderByDocNoCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderByDocNoCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(DeleteOrderByDocNoCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByDocNoAsync(request.DocNo);
        
        if (order == null)
        {
            throw new NotFoundException(nameof(Order), request.DocNo);
        }
        
        await _orderRepository.DeleteAsync(order);    
        return true;
    }
}