using System.ComponentModel.DataAnnotations;
using System.Net;
using Contracts.Messages;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders.Commands.CreateOrder;
using Ordering.Application.Features.V1.Orders.Commands.DeleteOrderByDocNo;
using Ordering.Application.Features.V1.Orders.Queries.GetOrderById;
using Ordering.Application.Features.V1.Orders.Queries.GetOrders;
using Ordering.Domain.Entities;
using Shared.Services.Email;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISmtpEmailService _emailService;
    private readonly IMessageProducer _messageProducer;
    public OrdersController(IMediator mediator, ISmtpEmailService emailService, IMessageProducer messageProducer)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _emailService = emailService;
        _messageProducer = messageProducer;
    }
    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
        public const string CreateOrder = nameof(CreateOrder);
    }
    
    [HttpGet("{username}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetOrdersByUserName([Required] string username)
    {
        var result = await _mediator.Send(new GetOrdersByUserNameQuery() {UserName = username});
        return Ok(result);

    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetOrdersBy(long id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost(RouteNames.CreateOrder)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateOrderAsync([Required] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("events")]
    public async Task<IActionResult> GetOrderEventsAsync([FromBody] Order order)
    {
        await _messageProducer.SendMessageAsync(order.ToString());
        return Ok();
    }
    
    [HttpDelete("document-no/{docNo}")]
    public async Task<IActionResult> GetOrdersBy(string docNo)
    {
        var query = new DeleteOrderByDocNoCommand(docNo);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Test([Required] CreateOrderCommand command)
    {
        return Ok(1);
    }
}