using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace Data {
    public class GameStatePlayerPrefsProvider : IGameStateProvider, IGameStateSaver {
        private const string KEY = "GAME STATE";
        
        public GameStateData GameState { get; private set; }
        
        public void SaveGameState() {
            var json = JsonUtility.ToJson(GameState);
            PlayerPrefs.SetString(KEY, json);
        }

        public void LoadGameState() {
            if (PlayerPrefs.HasKey(KEY)) {
                var json = PlayerPrefs.GetString(KEY);
                GameState = JsonUtility.FromJson<GameStateData>(json);
            }
            else {
                GameState = InitFromSettings();
                SaveGameState();
            }
        }

        private GameStateData InitFromSettings() {
            return new GameStateData {
                Inventories = new List<InventoryGridData> {
                    CreateTestInventory("Sava"),
                    CreateTestInventory("BlaBla")
                }
            };
        }
        
        private InventoryGridData CreateTestInventory(string ownerId) {
            var size = new Vector2Int(3, 4);
            var createdInventorySlots = new List<InventorySlotData>();
            for (int i = 0; i < size.x * size.y; i++) {
                createdInventorySlots.Add(new InventorySlotData());
            }

            var createdInventoryData = new InventoryGridData {
                OwnerId = ownerId,
                Size = size,
                Slots = createdInventorySlots
            };
            return createdInventoryData;
        }
    }
}