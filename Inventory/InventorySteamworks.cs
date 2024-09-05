using System.Collections.Generic;
using Steamworks;
using System;
using HamsterCombat;
using HamsterCombat.Storage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class InventorySteamworks
{
    private readonly Error _error;
    private readonly List<SteamItemDetails_t> _inventoryItems;
    private readonly List<InventoryItem> _inventory;
    private readonly Callback<SteamInventoryResultReady_t> _inventoryItemsResult;
    private readonly uint[] _consumedItemsQuantity = { 1 };
    private readonly uint[] _tradeUpItemsQuantity = new uint[HamsterCombat.TradeUp.PickableItemsCount];
    private readonly SteamItemInstanceID_t[] _consumedItem = new SteamItemInstanceID_t[1];
    private readonly SteamItemDef_t[] _openedItems = new SteamItemDef_t[1];
    private readonly uint[] _keyChestConsumedItemsQuantity = { 1, 1 };
    private readonly SteamItemInstanceID_t[] _keyChestConsumedItem = new SteamItemInstanceID_t[2];

    private readonly SteamItemInstanceID_t[] _tradeUp =
        new SteamItemInstanceID_t[HamsterCombat.TradeUp.PickableItemsCount];

    public Action<List<InventoryItem>> InventoryLoaded;
    public Action<List<InventoryItem>> InventoryAddItem;
    public Action<List<InventoryItem>> InventoryRemoveItem;

    private SteamInventoryResult_t _steamInventoryResult;

    private Queue<SteamInventoryResult_t> _steamInventoryResultQueue;

    private bool _initialized;

    public InventorySteamworks(Error error)
    {
        _error = error;
        
        if (!SteamManager.Initialized)
        {
            return;
        }

        _inventoryItems = new List<SteamItemDetails_t>();
        _inventory = new List<InventoryItem>();

        _inventoryItemsResult = Callback<SteamInventoryResultReady_t>.Create(OnGetInventoryItems);

        SteamInventory.GetAllItems(out _steamInventoryResult);

        for (int i = 0; i < HamsterCombat.TradeUp.PickableItemsCount; i++)
        {
            _tradeUpItemsQuantity[i] = 1;
        }
    }

    public void OnDestroy()
    {
        _inventoryItemsResult.Dispose();
    }

    public void SetInventoryResult(SteamInventoryResult_t inventoryResult)
    {
        _steamInventoryResult = inventoryResult;
    }

    public void RemoveAllItems()
    {
        for (int i = _inventoryItems.Count - 1; i >=0; i--)
        {
            SteamInventory.ConsumeItem(out _steamInventoryResult, _inventoryItems[i].m_itemId, 1);
        }
    }

    public void AddItems()
    {
        SteamInventory.GenerateItems(
            out _steamInventoryResult,
            new SteamItemDef_t[] {new SteamItemDef_t(3006),  new SteamItemDef_t(5102), new SteamItemDef_t(5203),  new SteamItemDef_t(5301),new SteamItemDef_t(5302),},
            new uint[]{1,1,1,1,1},
            5);
    }

    private void OnGetInventoryItems(SteamInventoryResultReady_t resultReady)
    {
        if (resultReady.m_result != EResult.k_EResultOK)
        {
            //_error.ShowError();
            Debug.LogError("Failed getting inventory: " + resultReady.m_result);
            return;
        }

        uint length = 0;
        bool getItemsResult = SteamInventory.GetResultItems(resultReady.m_handle, null, ref length);

        if (!getItemsResult)
        {
            //_error.ShowError();
            Debug.LogError("Failed getting items length");
            return;
        }

        var changedItemsDetails = new SteamItemDetails_t[length];
        getItemsResult = SteamInventory.GetResultItems(resultReady.m_handle, changedItemsDetails, ref length);

        if (!getItemsResult)
        {
            //_error.ShowError();
            Debug.LogError("Failed getting items");
            return;
        }
        
        List<InventoryItem> changedItems = new();
        for (var i = 0; i < changedItemsDetails.Length; i++)
        {
            if (changedItemsDetails[i].m_iDefinition.m_SteamItemDef == 0)
            {
                SteamInventory.DestroyResult(resultReady.m_handle);
                return;
            }

            int key = changedItemsDetails[i].m_iDefinition.m_SteamItemDef;

            if (!InventoryAllItems.Instance.Items.ContainsKey(key))
            {
                Debug.LogWarning($"Cannot find item definition for key: {key}");
                continue;
            }

            InventoryItemData data = 
                InventoryAllItems.Instance.Items[changedItemsDetails[i].m_iDefinition.m_SteamItemDef];
            ulong steamID = changedItemsDetails[i].m_itemId.m_SteamItemInstanceID;
            ushort quantity = changedItemsDetails[i].m_unQuantity;
            
            var item = new InventoryItem(steamID, data, quantity);
            changedItems.Add(item);
        }

        if (!_initialized)
        {
            for (var i = 0; i < changedItemsDetails.Length; i++)
            {
                _inventoryItems.Add(changedItemsDetails[i]);
                _inventory.Add(changedItems[i]);
            }

            InventoryLoaded?.Invoke(changedItems);

            //CheckForPromoItems();

            _initialized = true;
            
            UpdateInventorySizeProgress();
        }
        else if(changedItems.Count != 0)
        {
            List<InventoryItem> removedItems = new ();
            List<InventoryItem> addedItems = new ();

            for (var i = 0; i < changedItems.Count; i++)
            {
                int changedItemIndex = FindItemIndexBySteamID(changedItems[i].SteamID);

                bool isRemoved = changedItemIndex > 0 &&
                                 (int)_inventory[changedItemIndex].Quantity - changedItems[i].Quantity > 0;
                
                if (isRemoved)
                {
                    if (changedItems[i].Quantity == 0)
                    {
                        _inventoryItems.RemoveAt(changedItemIndex);
                        _inventory.RemoveAt(changedItemIndex);
                    }
                    else
                    {
                        _inventory[changedItemIndex] = changedItems[i];
                        _inventoryItems[changedItemIndex] = changedItemsDetails[i];
                    }
                    
                    removedItems.Add(changedItems[i]);
                    Debug.Log($"Removed:  {changedItems[i].InventoryItemData.ID}");
                }
                else
                {
                    if (changedItemIndex < 0)
                    {
                        _inventoryItems.Add(changedItemsDetails[i]);
                        _inventory.Add(changedItems[i]);
                    }
                    else
                    {
                        Debug.LogError("Adding items to stack not properly handled");
                        _inventoryItems[changedItemIndex] = changedItemsDetails[i];
                        _inventory[changedItemIndex] = changedItems[i];
                    }

                    addedItems.Add(changedItems[i]);
                    Debug.Log($"Added:  {changedItems[i].InventoryItemData.ID}");
                }
                
                UpdateInventorySizeProgress();
            }

            if (addedItems.Count != 0)
            {
                InventoryAddItem?.Invoke(addedItems);
            }

            if (removedItems.Count != 0)
            {
                InventoryRemoveItem?.Invoke(removedItems);
            }
        }
        
        SteamInventory.DestroyResult(resultReady.m_handle);
    }

    public void OpenChest(ulong chestSteamID)
    {
        int chestIndex = FindItemIndexBySteamID(chestSteamID);
        SteamItemDetails_t chest = _inventoryItems[chestIndex];
        _consumedItem[0] = chest.m_itemId;
        
        _openedItems[0] = new SteamItemDef_t(
            ((InventoryChestItemData)_inventory[chestIndex].InventoryItemData).ChestGeneratorID);

        SteamInventory.ExchangeItems(
            out _steamInventoryResult,
            _openedItems,
            _consumedItemsQuantity,
            (uint)_consumedItemsQuantity.Length,
            _consumedItem,
            _consumedItemsQuantity,
            (uint)_consumedItemsQuantity.Length);
    }

    public void OpenChest(ulong chestSteamID, ulong keySteamID)
    {
        int chestIndex = FindItemIndexBySteamID(chestSteamID);
        SteamItemDetails_t chest = _inventoryItems[chestIndex];

        SteamItemDetails_t key = FindItemBySteamID(keySteamID);
        
        _keyChestConsumedItem[0] = chest.m_itemId;
        _keyChestConsumedItem[1] = key.m_itemId;
        
        _openedItems[0] = new SteamItemDef_t(
            ((InventoryChestItemData)_inventory[chestIndex].InventoryItemData).ChestGeneratorID);

        SteamInventory.ExchangeItems(
            out _steamInventoryResult,
            _openedItems,
            _consumedItemsQuantity,
            (uint)_consumedItemsQuantity.Length,
            _keyChestConsumedItem,
            _keyChestConsumedItemsQuantity,
            (uint)_keyChestConsumedItemsQuantity.Length);
    }

    public void TradeUp(List<ulong> items, int tradeUpID)
    {
        _openedItems[0] = new SteamItemDef_t(tradeUpID);

        for (int i = 0; i < items.Count; i++)
        {
            _tradeUp[i] = FindItemBySteamID(items[i]).m_itemId;
        }
        
        SteamInventory.ExchangeItems(
            out _steamInventoryResult,
            _openedItems,
            _consumedItemsQuantity,
            (uint)_consumedItemsQuantity.Length,
            _tradeUp,
            _tradeUpItemsQuantity,
            (uint)_tradeUpItemsQuantity.Length);
    }

    private SteamItemDetails_t FindItemBySteamID(ulong steamID)
    {
        return _inventoryItems.Find((item) => item.m_itemId.m_SteamItemInstanceID == steamID);
    }

    private int FindItemIndexBySteamID(ulong steamID)
    {
        return _inventoryItems.FindIndex((item) => item.m_itemId.m_SteamItemInstanceID == steamID);
    }

    private void CheckForPromoItems()
    {
        SteamInventory.GrantPromoItems(out _steamInventoryResult);
    }

    private void UpdateInventorySizeProgress()
    {
        uint size = 0;
        for (int i = 0; i < _inventory.Count; i++)
        {
            size += _inventory[i].Quantity;
        }

        ProgressStorage.InventorySize.Value = (int)size;
    }
}
