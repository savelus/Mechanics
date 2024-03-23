using System;
using TMPro;
using UnityEngine;

namespace Views {
    public class InventorySlotView : MonoBehaviour {
        [SerializeField] private TMP_Text _textTitle;
        [SerializeField] private TMP_Text _textAmount;

        public string Title {
            get => _textTitle.text;
            set => _textTitle.text = value;
        }
        
        public int Amount {
            get => Convert.ToInt32(_textAmount.text);
            set => _textAmount.text = value == 0 ? "" : value.ToString();
        }
    }
}