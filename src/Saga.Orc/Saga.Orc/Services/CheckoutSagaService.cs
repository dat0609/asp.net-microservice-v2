using AutoMapper;
using Saga.Orc.HttpRepositories.Interfaces;
using Saga.Orc.Services.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;

namespace Saga.Orc.Services;

public class CheckoutSagaService : ICheckoutSagaService
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;

    public CheckoutSagaService(
        IOrderHttpRepository orderHttpRepository, 
        IBasketHttpRepository basketHttpRepository, 
        IInventoryHttpRepository inventoryHttpRepository, 
        IMapper mapper
        )
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _inventoryHttpRepository = inventoryHttpRepository;
        _mapper = mapper;
    }

    public async Task<bool> CheckoutOrder(string username, BasketCheckoutDto dto)
    {
        // get cart from basket http repository
        var cart = await _basketHttpRepository.GetBasket(username);
        if (cart == null) return false;
        
        // create order from order http repository
        var order = _mapper.Map<CreateOrderDto>(dto);
        order.TotalPrice = (long)cart.TotalPrice;
        
        var orderId = await _orderHttpRepository.CreateOrder(order);
        if (orderId < 0) return false;
        
        // get order by id
        var addedOrder = await _orderHttpRepository.GetOrderById(orderId);

        var inventoryDocNos = new List<string>();
        bool result;
        try
        {
            // sales items from inventory http repository
            foreach (var item in cart.Items)
            {
                var saleOrder = new SalesProductDto(addedOrder.InvoiceAddress, item.Quantity);
                saleOrder.SetItemNo(item.ItemNo);
                var docNo = await _inventoryHttpRepository.CreateSalesOrder(saleOrder);
                inventoryDocNos.Add(docNo);
            }

            result = await _basketHttpRepository.DeleteBasket(username);
        }
        catch (Exception e)
        {
            await RollbackOrder(username, addedOrder.Id, inventoryDocNos);
            result = false;
        }

        return result;
    }

    private async Task RollbackOrder(string username, long orderId, List<string> inventoryDocNo)
    {
        var deletedDocNo = new List<string>();
        // delete order by id
        foreach (var item in inventoryDocNo)
        {
            await _inventoryHttpRepository.DeleteOrderByDocumentNo(item);
            deletedDocNo.Add(item);
        }
    }
}