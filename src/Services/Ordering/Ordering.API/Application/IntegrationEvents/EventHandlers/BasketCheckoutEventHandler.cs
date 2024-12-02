using AutoMapper;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Features.V1.Orders.Commands.CreateOrder;
using Ordering.Domain.Entities;
using Ordering.Domain.Enums;

namespace Ordering.API.Application.IntegrationEvents.EventHandlers;

public class BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent>
{
    private readonly IMediator _mediator;
    private readonly Serilog.ILogger _logger;
    private readonly IOrderRepository _orderRepository;

    public BasketCheckoutEventHandler(IMediator mediator,Serilog.ILogger logger, IOrderRepository orderRepository)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _orderRepository = orderRepository;
    }


    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        var order = new Order()
        {
            UserName = context.Message.UserName,
            EmailAddress = context.Message.EmailAddress,
            ShippingAddress = context.Message.ShippingAddress,
            InvoiceAddress = context.Message.InvoiceAddress,
            TotalPrice = context.Message.TotalPrice,
            FirstName = context.Message.FirstName,
            LastName = context.Message.LastName,
            Status = EOrderStatus.New,
            CreatedDate = DateTimeOffset.UtcNow,
            DocumentNo = Guid.NewGuid()
        };

        var result = await _orderRepository.CreateAsync(order);
        _logger.Information("BasketCheckoutEvent consumed successfully. " +
                            "Order is created with Id: {newOrderId}", result);
    }
}