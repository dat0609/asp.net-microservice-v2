using Microsoft.AspNetCore.Mvc;
using Saga.Orc.Services.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orc.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutSagaService _checkoutSagaService;
    public CheckoutController(ICheckoutSagaService checkoutSagaService)
    {
        _checkoutSagaService = checkoutSagaService;
    }

    [HttpPost]
    public async Task<IActionResult> CheckoutOrder(string username, [FromBody] BasketCheckoutDto dto)
    {
        var result = await _checkoutSagaService.CheckoutOrder(username, dto);
        return Ok(result);
    }
}