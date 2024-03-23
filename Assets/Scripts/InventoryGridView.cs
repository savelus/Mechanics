using ReadOnly;
using UnityEngine;

public class InventoryGridView : MonoBehaviour {
    private IReadOnlyInventoryGrid _inventory;
    
    public void Setup(IReadOnlyInventoryGrid inventory) {
        _inventory = inventory;
        Print();
    }

    public void Print() {
        var slots = _inventory.GetSlots();
        var size = _inventory.Size;
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                var slot = slots[x, y];
                Debug.Log($"Slot ({x},{y}. Item: {slot.ItemId}, amount: {slot.Amount})");
            }
        }
    }
}