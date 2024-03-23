public readonly struct RemoveItemsFromInventoryGridResult { 
    public readonly string InventoryOwnerId;
    public readonly int ItemsToRemoveAmount;
    public readonly bool Success;

    public RemoveItemsFromInventoryGridResult(string inventoryOwnerId, 
        int itemsToRemoveAmount, 
        bool success) {
        InventoryOwnerId = inventoryOwnerId;
        ItemsToRemoveAmount = itemsToRemoveAmount;
        Success = success;
    }
}