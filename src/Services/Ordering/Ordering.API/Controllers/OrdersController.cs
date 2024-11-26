using System.ComponentModel.DataAnnotations;
using System.Net;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders.Queries.GetDemo;
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
    public OrdersController(IMediator mediator, ISmtpEmailService emailService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _emailService = emailService;
    }
    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
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
}