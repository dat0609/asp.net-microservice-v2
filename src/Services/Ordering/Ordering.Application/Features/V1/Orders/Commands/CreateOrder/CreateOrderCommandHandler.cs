using Contracts.Common.Interfaces;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.V1.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, long>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<long> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var newOrder = new Order
        {
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailAddress = request.EmailAddress,
            ShippingAddress = request.ShippingAddress,
            InvoiceAddress = request.InvoiceAddress,
        };
        _orderRepository.Create(newOrder);
        newOrder.AddedOrder();
        //newOrder.DeletedOrder();
        await _orderRepository.SaveChangesAsync();
        
        return newOrder.Id;
    }
}