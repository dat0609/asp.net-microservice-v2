using System.ComponentModel.DataAnnotations;
using System.Net;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketsController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;

    public BasketsController(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }
    
    [HttpGet("{username}", Name = "GetBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetBasket([Required] string username)
    {
        var result = await _basketRepository.GetBasketByUserName(username);

        return Ok(result ?? new Cart(username));
    }
    
    [HttpPost(Name = "UpdateBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> UpdateBasket([FromBody] Cart cart)
    {
        // Communicate with Inventory.Product.Grpc and check quantity available of products
        /*foreach (var item in model.Items)
        {
            var stock = await _stockItemGrpcService.GetStock(item.ItemNo);
            item.SetAvailableQuantity(stock.Quantity);
        }*/
        
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(10));
        //     .SetSlidingExpiration(TimeSpan.FromMinutes(10));
        
        //var cart = _mapper.Map<Cart>(model);
        var updateCart = await _basketRepository.UpdateBasket(cart, options);
        //var result = _mapper.Map<CartDto>(updateCart);
        
        return Ok(updateCart);
    }
    
    [HttpDelete("{username}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteBasket([Required] string username)
    {
        var result = await _basketRepository.DeleteBasketFromUserName(username);
        return Ok(result);
    }
}