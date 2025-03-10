using Contracts.Saga.OrderManager;
using Microsoft.AspNetCore.Mvc;
using Saga.Orc.OrderManager;
using Saga.Orc.Services.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orc.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutSagaService _checkoutSagaService;
    private readonly ISagaOrderManager<BasketCheckoutDto, OrderResponse> _sagaOrderManager;
    public CheckoutController(ICheckoutSagaService checkoutSagaService, ISagaOrderManager<BasketCheckoutDto, OrderResponse> sagaOrderManager)
    {
        _checkoutSagaService = checkoutSagaService;
        _sagaOrderManager = sagaOrderManager;
    }

    [HttpPost]
    public async Task<IActionResult> CheckoutOrder(string username, [FromBody] BasketCheckoutDto dto)
    {
        var result = await _checkoutSagaService.CheckoutOrder(username, dto);
        return Ok(result);
    }
    
    [HttpPost("saga")]
    public IActionResult CheckoutOrderSaga(string username, [FromBody] BasketCheckoutDto dto)
    {
        dto.UserName = username;
        var result = _sagaOrderManager.CreateOrder(dto);
        return Ok(result);
    }
}