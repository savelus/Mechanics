using System.Collections.Generic;
using Data;
using ReadOnly;
using UnityEngine;

public class InventoryService {
    private readonly IGameStateSaver _gameStateSaver;
    private Dictionary<string, InventoryGrid> _inventoriesMap = new();

    public InventoryService(IGameStateSaver gameStateSaver) {
        _gameStateSaver = gameStateSaver;
    }

    public InventoryGrid RegisterInventory(InventoryGridData inventoryData) {
        var inventoryGrid = new InventoryGrid(inventoryData);
        _inventoriesMap[inventoryData.OwnerId] = inventoryGrid;
        return inventoryGrid;
    }

    public AddItemsToInventoryGridResult AddItemsToInventory(string ownerId, string itemId, int amount = 1) {
        var inventory = _inventoriesMap[ownerId];
        var result = inventory.AddItems(itemId, amount);
        _gameStateSaver.SaveGameState();
        return result;
    }

    public AddItemsToInventoryGridResult AddItemsToInventory(string ownerId,
        Vector2Int slotCoords,
        string itemId,
        int amount) {
        var inventory = _inventoriesMap[ownerId];
        var result = inventory.AddItems(slotCoords, itemId, amount);
        _gameStateSaver.SaveGameState();
        return result;
    }

    public RemoveItemsFromInventoryGridResult RemoveItems(string ownerId, string itemId, int amount = 1) {
        var inventory = _inventoriesMap[ownerId];
        var result = inventory.RemoveItems(itemId, amount);
        _gameStateSaver.SaveGameState();
        return result;
    }

    public RemoveItemsFromInventoryGridResult RemoveItems(string ownerId, Vector2Int slotCoords, string itemId,
        int amount = 1) {
        var inventory = _inventoriesMap[ownerId];
        var result = inventory.RemoveItems(slotCoords, itemId, amount);
        _gameStateSaver.SaveGameState();
        return result;
    }

    public bool Has(string ownerId, string itemId, int amount) {
        var inventory = _inventoriesMap[ownerId];
        return inventory.Has(itemId, amount);
    }

    public IReadOnlyInventoryGrid GetInventory(string ownerId) {
        return _inventoriesMap[ownerId];
    }
}