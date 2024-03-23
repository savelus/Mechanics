using System.Collections.Generic;
using ReadOnly;
using Views;

namespace Controllers {
    public class InventoryGridController {
        private readonly List<InventorySlotController> _slotControllers = new();

        public InventoryGridController(IReadOnlyInventoryGrid inventory, InventoryView view) {
            var size = inventory.Size;
            var slots = inventory.GetSlots();
            var lineLength = size.y;

            for (var x = 0; x < size.x; x++) {
                for (var y = 0; y < size.y; y++) {
                    var index = x * lineLength + y;
                    var slotView = view.GetInventorySlotView(index);
                    var slot = slots[x, y];
                    _slotControllers.Add(new InventorySlotController(slot, slotView));
                }
            }
        }
    }
}