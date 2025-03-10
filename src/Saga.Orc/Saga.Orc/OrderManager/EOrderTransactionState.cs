namespace Saga.Orc.OrderManager;

public enum EOrderTransactionState
{
    NoStarted,
    BasketGot,
    BasketGetFailed,
    OrderCreated,
    OrderCreatedFailed,
    OrderGot,
    OrderGetFailed,
    InventoryUpdated,
    InventoryUpdateFailed,
    RollbackInventory,
    InventoryRollback,
    InventoryRollbackFailed,
    BasketDeleted,
    OrderDeleted,
    OrderDeletedFailed
}