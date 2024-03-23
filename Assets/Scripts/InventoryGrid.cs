using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using ReadOnly;
using UnityEngine;

public class InventoryGrid : IReadOnlyInventoryGrid{
    public event Action<string, int> ItemsAdded;
    public event Action<string, int> ItemsRemoved;
    public event Action<Vector2Int> SizeChanged;

    public string OwnerId => _data.OwnerId;

    public Vector2Int Size {
        get=> _data.Size;
        set 
        {
            if (_data.Size == value) return;
            _data.Size = value;
            SizeChanged?.Invoke(value);
        }
    }
    
    private readonly InventoryGridData _data;
    private readonly Dictionary<Vector2Int, InventorySlot> _slotsMap = new();
    
    public InventoryGrid(InventoryGridData data) {
        _data = data;

        FillSlotsMap(data);
    }

    public AddItemsToInventoryGridResult AddItems(string itemId, int amount = 1) {
        var remainingAmount = amount;
        var itemsAddedToSlotsWithSameItemsAmount = AddToSlotsWithSameItems(itemId, remainingAmount, out remainingAmount);

        if (remainingAmount <= 0)
            return new AddItemsToInventoryGridResult(OwnerId, amount, itemsAddedToSlotsWithSameItemsAmount);

        var itemsAddedToAvailableSlotsAmount = AddToFirstAvailableSlots(itemId, remainingAmount, out remainingAmount);
        var totalAddedItemsAmount = itemsAddedToSlotsWithSameItemsAmount + itemsAddedToAvailableSlotsAmount;
        
        return new AddItemsToInventoryGridResult(OwnerId, amount, totalAddedItemsAmount);
    }

    
    public AddItemsToInventoryGridResult AddItems(Vector2Int slotCoords, string itemId, int amount = 1) {
        var slot = _slotsMap[slotCoords];
        var newValue = slot.Amount + amount;
        var itemsAddedAmount = 0;

        if (slot.IsEmpty) {
            slot.ItemId = itemId;
        }

        var itemSlotCapacity = GetItemSlotCapacity(itemId);

        if (newValue > itemSlotCapacity) {
            var remainingItems = newValue - itemSlotCapacity;
            itemsAddedAmount += itemSlotCapacity - slot.Amount;
            slot.Amount = itemSlotCapacity;

            var result = AddItems(itemId, remainingItems);
            itemsAddedAmount += result.ItemsAddedAmount;
        }
        else {
            itemsAddedAmount = amount;
            slot.Amount = newValue;
        }

        return new AddItemsToInventoryGridResult(OwnerId, amount, itemsAddedAmount);
    }

    public RemoveItemsFromInventoryGridResult RemoveItems(string itemId, int amount = 1) {
        if (!Has(itemId, amount))
            return new RemoveItemsFromInventoryGridResult(OwnerId, amount, false);

        var amountToRemove = amount;
        for (int x = 0; x < Size.x; x++) {
            for (int y = 0; y < Size.y; y++) {
                var slotCoords = new Vector2Int(x, y);
                var slot = _slotsMap[slotCoords];
                
                if(slot.ItemId!=itemId) continue;

                if (amountToRemove > slot.Amount) {
                    amountToRemove -= slot.Amount;
                    RemoveItems(slotCoords, itemId, slot.Amount);

                }
                else {
                    RemoveItems(slotCoords, itemId, amountToRemove);
                    return new RemoveItemsFromInventoryGridResult(OwnerId, amount, true);
                }
            }
        }

        throw new Exception("Something went wrong, couldn't remove some items");
    }

    public RemoveItemsFromInventoryGridResult RemoveItems(Vector2Int slotCoords, string itemId, int amount = 1) {
        var slot = _slotsMap[slotCoords];

        if (slot.IsEmpty || slot.ItemId != itemId || slot.Amount < amount) {
            return new RemoveItemsFromInventoryGridResult(OwnerId, amount, false);
        }

        slot.Amount -= amount;

        if (slot.Amount == 0) {
            slot.ItemId = null;
        }

        return new RemoveItemsFromInventoryGridResult(OwnerId, amount, true);
    }

    public int GetAmount(string itemId) {
        var slots = _data.Slots;

        return slots.Where(slot => slot.ItemId == itemId).Sum(slot => slot.Amount);
    }

    public bool Has(string itemId, int amount) {
        return GetAmount(itemId) >= amount;
    }

    public void SwitchSlots(Vector2Int firstSlotCoords, Vector2Int secondSlotCoords) {
        var firstSlot = _slotsMap[firstSlotCoords];
        var secondSlot = _slotsMap[secondSlotCoords];

        var tempSlotItemId = firstSlot.ItemId;
        var tempSlotItemAmount = firstSlot.Amount;
        
        firstSlot.ItemId = secondSlot.ItemId;
        firstSlot.Amount = secondSlot.Amount;
        secondSlot.ItemId = tempSlotItemId;
        secondSlot.Amount = tempSlotItemAmount;
    }

    public void SetSize(Vector2Int size) {
        throw new NotImplementedException();
    }

    public IReadOnlyInventorySlot[,] GetSlots() {
        var array = new IReadOnlyInventorySlot[Size.x, Size.y];

        for (int x = 0; x < Size.x; x++) {
            for (int y = 0; y < Size.y; y++) {
                array[x, y] = _slotsMap[new Vector2Int(x, y)];
            }
        }

        return array;
    }

    private void FillSlotsMap(InventoryGridData data) {
        var size = data.Size;
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                var index = x * size.y + y;
                var slotData = data.Slots[index];
                var slot = new InventorySlot(slotData);
                var position = new Vector2Int(x, y);
                _slotsMap[position] = slot;
            }
        }
    }

    private int GetItemSlotCapacity(string itemId) {
        return 99;
    }
    
    private int AddToSlotsWithSameItems(string itemId, 
        int amount, 
        out int remainingAmount) {
        var itemsAddedAmount = 0;
        remainingAmount = amount;

        for (int x = 0; x < Size.x; x++) {
            for (int y = 0; y < Size.y; y++) {
                var coords = new Vector2Int(x,y);
                var slot = _slotsMap[coords];

                if (slot.IsEmpty) continue;

                var slotItemCapacity = GetItemSlotCapacity(slot.ItemId);
                if (slot.Amount >= slotItemCapacity) continue;

                if(slot.ItemId != itemId) continue;
                
                var newValue = slot.Amount + remainingAmount;

                if (newValue > slotItemCapacity) {
                    remainingAmount = newValue - slotItemCapacity;
                    itemsAddedAmount += slotItemCapacity - slot.Amount;
                    slot.Amount = slotItemCapacity;

                    if (remainingAmount == 0) return itemsAddedAmount;
                }
                else {
                    itemsAddedAmount += remainingAmount;
                    slot.Amount = newValue;
                    remainingAmount = 0;
                    return itemsAddedAmount;
                }
            }
        }

        return itemsAddedAmount;
    }

    private int AddToFirstAvailableSlots(string itemId, int amount, out int remainingAmount) {
        var itemsAddedAmount = 0;
        remainingAmount = amount;

        for (int x = 0; x < Size.x; x++) {
            for (int y = 0; y < Size.y; y++) {
                var coords = new Vector2Int(x, y);
                var slot = _slotsMap[coords];
                
                if(!slot.IsEmpty) continue;

                slot.ItemId = itemId;
                var newValue = remainingAmount;
                var slotItemCapacity = GetItemSlotCapacity(slot.ItemId);

                if (newValue > slotItemCapacity) {
                    remainingAmount = newValue - slotItemCapacity;
                    itemsAddedAmount += slotItemCapacity;
                    slot.Amount = slotItemCapacity;
                }
                else {
                    itemsAddedAmount += remainingAmount;
                    slot.Amount = newValue;
                    remainingAmount = 0;

                    return itemsAddedAmount;
                }
            }
        }

        return itemsAddedAmount;
    }
}