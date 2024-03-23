using ReadOnly;
using Views;

namespace Controllers {
    public class InventorySlotController {
        private readonly InventorySlotView _view;

        public InventorySlotController(IReadOnlyInventorySlot slot, InventorySlotView view) {
            _view = view;

            slot.ItemIdChanged += OnSlotItemIdChanged;
            slot.ItemAmountChanged += OnSlotItemAmountChanged;

            view.Title = slot.ItemId;
            view.Amount = slot.Amount;
        }

        private void OnSlotItemIdChanged(string newItemId) {
            _view.Title = newItemId;
        }

        private void OnSlotItemAmountChanged(int amount) {
            _view.Amount = amount;
        }
    }
}