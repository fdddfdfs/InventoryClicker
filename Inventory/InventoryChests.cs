using System.Collections.Generic;
using System.Linq;
using HamsterCombat;
using Steamworks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public sealed class InventoryChests : InventoryGrid
{
    private readonly Image _requiredKeyImage;
    private readonly Button _requiredKeyButton;
    private readonly TMP_Text _requiredKeyText;
    private readonly Case _case;
    private bool _isChestOpened;

    private Dictionary<InventoryKeyItemData, List<InventoryItem>> _keys;

    public InventoryChests(
        InventorySteamworks inventorySteamworks,
        List<GameObject> inventoryCells,
        TMP_Text nameText,
        TMP_Text descriptionText,
        Button button,
        Button nextPageButton,
        Button previousPageButton,
        Sprite invisibleSprite,
        Image requiredKeyImage,
        TMP_Text requiredKeyText,
        List<int> defaultItemsIds,
        Case @case,
        Dictionary<InventoryItemRarityType, Color> rarityColors)
        : base(inventorySteamworks,
            inventoryCells,
            nameText,
            descriptionText,
            button,
            nextPageButton,
            previousPageButton,
            invisibleSprite,
            defaultItemsIds,
            rarityColors)
    {
        _requiredKeyImage = requiredKeyImage;
        _requiredKeyText = requiredKeyText;
        _case = @case;
        _inventoryItemsType = InventoryItemType.Chest;
        _keys = new Dictionary<InventoryKeyItemData, List<InventoryItem>>();

        _requiredKeyButton = _requiredKeyImage.gameObject.transform.parent.GetComponent<Button>();
        
        _requiredKeyButton.gameObject.SetActive(false);
        _requiredKeyText.gameObject.SetActive(false);
    }

    public override void CloseInventory()
    {
        _requiredKeyButton.gameObject.SetActive(false);
        _requiredKeyText.gameObject.SetActive(false);
        
        base.CloseInventory();
    }

    protected override void ShowItemInfo(int itemNumber)
    {
        base.ShowItemInfo(itemNumber);

        if (!_button.gameObject.activeSelf)
        {
            _button.gameObject.SetActive(true);
        }

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            if (_isChestOpened) return;

            OpenChest(itemNumber);
        });

        _buttonText.text = Localization.Instance["Open Chest"];

        var inventoryChestItemData = _items[itemNumber] as InventoryChestItemData;

        if (inventoryChestItemData.RequiredKey)
        {
            int requiredKeysCount =_keys.TryGetValue(inventoryChestItemData.RequiredKey, out List<InventoryItem> key) ?
                (int)key.Sum((item)=>item.Quantity) :
                0;
            
            _requiredKeyButton.gameObject.SetActive(true);
            _requiredKeyText.gameObject.SetActive(true);
            
            _requiredKeyImage.sprite = inventoryChestItemData.RequiredKey.Icon;
            _requiredKeyText.text = "Currently have " + requiredKeysCount + " keys \n Need 1 to open chest";

            _button.gameObject.SetActive(requiredKeysCount != 0);
            
            _requiredKeyButton.onClick.RemoveAllListeners();
            _requiredKeyButton.onClick.AddListener(() =>
            {
                SteamInventory.StartPurchase(
                    new SteamItemDef_t[] { new (inventoryChestItemData.RequiredKey.ID) },
                    new uint[] { 1 }, 
                    1);
            });
        }
        else
        {
            _requiredKeyButton.gameObject.SetActive(false);
            _requiredKeyText.gameObject.SetActive(false);
        }
    }

    protected override void RemoveItemFromInventory(List<InventoryItem> removedItems)
    {
        foreach (InventoryItem removedItem in removedItems)
        {
            if (removedItem.InventoryItemData is InventoryKeyItemData inventoryKeyItemData)
            {
                int index = _keys[inventoryKeyItemData].FindIndex((item) => item.SteamID == removedItem.SteamID);
                if (removedItem.Quantity == 0)
                {
                    _keys[inventoryKeyItemData].RemoveAt(index);
                }
                else
                {
                    _keys[inventoryKeyItemData][index] = removedItem;
                }
            }
        }
        
        base.RemoveItemFromInventory(removedItems);
    }

    protected override void InitializeInventory(List<InventoryItem> inventoryItems)
    {
        AddKeys(inventoryItems);
        
        base.InitializeInventory(inventoryItems);
    }

    protected override void AddItemInInventory(List<InventoryItem> addedItems)
    {
        _isChestOpened = false;

        AddKeys(addedItems);
        
        base.AddItemInInventory(addedItems);
    }

    protected override void ClearSelection()
    {
        _requiredKeyButton.gameObject.SetActive(false);
        _requiredKeyText.gameObject.SetActive(false);
        
        base.ClearSelection();
    }

    private void AddKeys(List<InventoryItem> addedItems)
    {
        foreach (InventoryItem inventoryItem in addedItems)
        {
            if (inventoryItem.InventoryItemData is InventoryKeyItemData inventoryKeyItemData)
            {
                if (!_keys.ContainsKey(inventoryKeyItemData))
                {
                    _keys[inventoryKeyItemData] = new List<InventoryItem>();
                }
                
                _keys[inventoryKeyItemData].Add(inventoryItem);
            }
        }
    }

    private void OpenChest(int chestNumber)
    {
        _isChestOpened = true;
        _button.gameObject.SetActive(false);
        _requiredKeyButton.gameObject.SetActive(false);
        _requiredKeyText.gameObject.SetActive(false);
        
        _case.OpenCase((InventoryChestItemData)_items[chestNumber]);
        
        InventoryKeyItemData requiredKey = ((InventoryChestItemData)_items[chestNumber]).RequiredKey;
        
        if (requiredKey)
        {
            _inventorySteamworks.OpenChest(_inventoryItems[chestNumber][^1].SteamID, _keys[requiredKey][^1].SteamID);
        }
        else
        {
            _inventorySteamworks.OpenChest(_inventoryItems[chestNumber][^1].SteamID);
        }
    }
}
