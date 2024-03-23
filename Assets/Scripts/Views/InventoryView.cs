using TMPro;
using UnityEngine;

namespace Views {
    public class InventoryView : MonoBehaviour {
        [SerializeField] private InventorySlotView[] _slots;
        [SerializeField] private TMP_Text _textOwner;

        public string OwnerId {
            get => _textOwner.text;
            set => _textOwner.text = value;
        }

        public InventorySlotView GetInventorySlotView(int index) {
            return _slots[index];
        }
    }
}