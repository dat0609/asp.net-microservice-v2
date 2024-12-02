using System.ComponentModel.DataAnnotations;
using System.Net;
using Contracts.Messages;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders.Commands.CreateOrder;
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

    [HttpGet]
    public async Task<IActionResult> GetOrdersBy()
    {
        var msg = new MailRequest()
        {
            Subject = "Test Email",
            Body = "This is a test email.",
            ToAddress = "lequocdat731@gmail.com"
        };
        await _emailService.SendEmailAsync(msg);
        return Ok();
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
}