using AutoMapper;
using Contracts.Saga.OrderManager;
using Saga.Orc.HttpRepositories.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;
using Stateless;

namespace Saga.Orc.OrderManager;

public class SagaOrderManager : ISagaOrderManager<BasketCheckoutDto, OrderResponse>
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;

    public SagaOrderManager(
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
    public OrderResponse CreateOrder(BasketCheckoutDto input)
    {
        var orderStateMachine = new StateMachine<EOrderTransactionState, EOrderAction>(EOrderTransactionState.NoStarted);

        long orderId = -1;
        CartDto cart = null;
        OrderDto addedOrder = null;
        string? inventoryDocNo = "";

        orderStateMachine.Configure(EOrderTransactionState.NoStarted)
            .PermitDynamic(EOrderAction.GetBasket, () =>
            {
                cart = _basketHttpRepository.GetBasket(input.UserName).Result;
                return cart != null ? EOrderTransactionState.BasketGot : EOrderTransactionState.BasketGetFailed;
            });

        orderStateMachine.Configure(EOrderTransactionState.BasketGot)
            .PermitDynamic(EOrderAction.CreateOrder, () =>
            {
                var order = _mapper.Map<CreateOrderDto>(input);
                orderId = _orderHttpRepository.CreateOrder(order).Result;
                return orderId > 0 ? EOrderTransactionState.OrderCreated : EOrderTransactionState.OrderCreatedFailed;
            })
            .OnEntry(() =>
            {
                orderStateMachine.Fire(EOrderAction.CreateOrder);
            });

        orderStateMachine.Configure(EOrderTransactionState.OrderCreated)
            .PermitDynamic(EOrderAction.GetOrder, () =>
            {
                addedOrder = _orderHttpRepository.GetOrderById(orderId).Result;
                return addedOrder != null ? EOrderTransactionState.OrderGot : EOrderTransactionState.OrderGetFailed;
            })
            .OnEntry(() =>
            {
                orderStateMachine.Fire(EOrderAction.GetOrder);
            });

        orderStateMachine.Configure(EOrderTransactionState.OrderGot)
            .PermitDynamic(EOrderAction.UpdateInventory, () =>
            {
                var saleOrder = new SaleOrderDto
                {
                    OrderDocNo = addedOrder.EmailAddress,
                    SaleItems = _mapper.Map<List<SaleItemDto>>(cart.Items),
                };
                inventoryDocNo = _inventoryHttpRepository.CreateOrderSale(addedOrder.ShippingAddress, saleOrder).Result;

                return inventoryDocNo != null
                    ? EOrderTransactionState.InventoryUpdated : EOrderTransactionState.InventoryUpdateFailed;
            })
            .OnEntry(() =>
            {
                orderStateMachine.Fire(EOrderAction.UpdateInventory);
            });

        orderStateMachine.Configure(EOrderTransactionState.InventoryUpdated)
            .PermitDynamic(EOrderAction.DeleteBasket, () =>
            {
                var result = _basketHttpRepository.DeleteBasket(input.UserName).Result;
                return result ? EOrderTransactionState.BasketDeleted : EOrderTransactionState.InventoryUpdateFailed;
            })
            .OnEntry(() =>
            {
                orderStateMachine.Fire(EOrderAction.DeleteBasket);
            });

        orderStateMachine.Configure(EOrderTransactionState.InventoryRollbackFailed)
            .PermitDynamic(EOrderAction.DeleteInventory, () =>
            {
                RollbackOrder(input.UserName, inventoryDocNo, orderId);
                return EOrderTransactionState.RollbackInventory;
            });
        
        orderStateMachine.Fire(EOrderAction.GetBasket);
        
        return new OrderResponse(orderStateMachine.State == EOrderTransactionState.InventoryUpdated);
    }

    public OrderResponse RollbackOrder(string username, string docNo, long orderId)
    {
        return new OrderResponse(false);
    }
}