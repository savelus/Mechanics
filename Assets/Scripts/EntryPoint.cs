using System;
using System.Collections.Generic;
using Controllers;
using Data;
using UnityEngine;
using Views;
using Random = UnityEngine.Random;

public class EntryPoint : MonoBehaviour {
    [SerializeField] private ScreenView _screenView;

    private const string OWNER_1 = "Sava";
    private const string OWNER_2 = "BlaBla";
    private readonly string[] _itemIds = { "Apple", "Seed", "Armor", "Potion" };
    
    private InventoryService _inventoryService;
    private ScreenController _screenController;
    private string _cashedOwnerId;

    private void Start() {
        var gameStateProvider = new GameStatePlayerPrefsProvider();
        gameStateProvider.LoadGameState();

        _inventoryService = new InventoryService(gameStateProvider);

        var gameState = gameStateProvider.GameState;
        foreach (var inventoryData in gameState.Inventories) {
            _inventoryService.RegisterInventory(inventoryData);
        }
        
        _screenController = new ScreenController(_inventoryService, _screenView);
        _screenController.OpenInventory(OWNER_1);
        _cashedOwnerId = OWNER_1;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            _screenController.OpenInventory(OWNER_1);
            _cashedOwnerId = OWNER_1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            _screenController.OpenInventory(OWNER_2);
            _cashedOwnerId = OWNER_2;
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            var rIndex = Random.Range(0, _itemIds.Length);
            var rItemId = _itemIds[rIndex];
            var rAmount = Random.Range(1, 50);
            var result = _inventoryService.AddItemsToInventory(_cashedOwnerId, rItemId, rAmount);
            
            print($"Item added : {rItemId}. Amount added: {result.ItemsAddedAmount}");
        }
        
        if (Input.GetKeyDown(KeyCode.R)) {
            var rIndex = Random.Range(0, _itemIds.Length);
            var rItemId = _itemIds[rIndex];
            var rAmount = Random.Range(1, 50);
            var result = _inventoryService.RemoveItems(_cashedOwnerId, rItemId, rAmount);
            
            print($"Item added : {rItemId}. Trying to remove: {result.ItemsToRemoveAmount}, Success: {result.Success}");
        }
    }
}