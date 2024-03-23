using System;
using Data;
using ReadOnly;

public class InventorySlot : IReadOnlyInventorySlot {
    public event Action<string> ItemIdChanged;
    public event Action<int> ItemAmountChanged;

    public string ItemId {
        get=>_data.ItemId;
        set
        {
            if (_data.ItemId == value) return;
            _data.ItemId = value;
            ItemIdChanged?.Invoke(value);
        }
    }

    public int Amount {
        get => _data.Amount;
        set
        {
            if (_data.Amount == value) return;
            _data.Amount = value;
            ItemAmountChanged?.Invoke(value);
        }
    }

    public bool IsEmpty => Amount == 0 && string.IsNullOrEmpty(ItemId);
    
    private readonly InventorySlotData _data;

    public InventorySlot(InventorySlotData data) {
        _data = data;
    }
}