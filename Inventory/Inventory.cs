using System;
using System.Collections.Generic;
using HamsterCombat;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Inventory : MonoBehaviour, IMenu
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private List<GameObject> _inventoryCells;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Button _button;
    [SerializeField] private Button _nextPageButton, _previousPageButton;

    [SerializeField] private TMP_Text _requiredKeysCount;
    [SerializeField] private Image _requiredKeyIcon;
    
    [SerializeField] private Sprite _invisibleSprite;

    [SerializeField] private Button _chest;
    [SerializeField] private Button _clothes;
    [SerializeField] private Button _keys;

    private Dictionary<Type, InventoryGrid> _inventories;
    private InventoryGrid _currentInventoryGrid;

    private Case _case;
    private ItemReceived _itemReceived;
    private Error _error;

    public InventorySteamworks InventorySteamworks { get; private set; }
    
    private void Start()
    {
        _itemReceived.SetAsLastSibling();
        _error.SetAsLastSibling();
    }

    public void ChangeMenuActive(bool state)
    {
        _menu.SetActive(state);
    }

    public void ChangeMenuActive()
    {
        _menu.SetActive(!_menu.activeSelf);
    }

    public void Init(
        Target target,
        Transform parent,
        Dictionary<InventoryItemRarityType, Color> rarityColors,
        Error error)
    {
        _error = error;
        
        InventorySteamworks = new InventorySteamworks(error);
        
        gameObject.transform.SetAsLastSibling();
        
        _itemReceived = new ItemReceived(
            parent,
            "UI/ItemReceivedView",
            InventorySteamworks,
            rarityColors);
        
        _itemReceived.ChangeMenuActive(false);
        
        _case = new Case(parent, "UI/Case", InventorySteamworks, rarityColors, _itemReceived, error);
        _case.ChangeMenuActive(false);
        
        InventoryChests inventoryChests = new (
            InventorySteamworks,
            _inventoryCells,
            _nameText,
            _descriptionText,
            _button,
            _nextPageButton,
            _previousPageButton,
            _invisibleSprite,
            _requiredKeyIcon,
            _requiredKeysCount,
            null,
            _case,
            rarityColors);
        
        InventoryClothes inventoryClothes = new (
            InventorySteamworks,
            _inventoryCells,
            _nameText,
            _descriptionText,
            _button,
            _nextPageButton,
            _previousPageButton,
            _invisibleSprite,
            new List<int>(){101},
            target,
            rarityColors);
        
        InventoryKeys inventoryKeys = new(
            InventorySteamworks,
            _inventoryCells,
            _nameText,
            _descriptionText,
            _button,
            _nextPageButton,
            _previousPageButton,
            _invisibleSprite,
            null,
            rarityColors);
        
        _inventories = new Dictionary<Type, InventoryGrid>
        {
            [typeof(InventoryClothes)] = inventoryClothes,
            [typeof(InventoryChests)] = inventoryChests,
            [typeof(InventoryKeys)] = inventoryKeys,
        };

        _clothes.onClick.AddListener(() => OpenNewInventory(inventoryClothes));
        _chest.onClick.AddListener(() => OpenNewInventory(inventoryChests));
        _keys.onClick.AddListener(() => OpenNewInventory(inventoryKeys));
        
        OpenInventory(typeof(InventoryClothes));
    }
    
    public void OpenInventory(Type inventoryType)
    {
        OpenNewInventory(_inventories[inventoryType]);
    }

    public void SetInventoryResult(SteamInventoryResult_t inventoryResult)
    {
        InventorySteamworks.SetInventoryResult(inventoryResult);
    }

    private void OpenNewInventory(InventoryGrid newInventory)
    {
        if (_currentInventoryGrid == newInventory)
            return;

        _currentInventoryGrid?.CloseInventory();

        newInventory.OpenInventory();
        _currentInventoryGrid = newInventory;
    }

    private void OnDestroy()
    {
        foreach (KeyValuePair<Type,InventoryGrid> inventoryGrid in _inventories)
        {
            inventoryGrid.Value.ClearEvents();
        }
        
        InventorySteamworks.OnDestroy();
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //InventorySteamworks.RemoveAllItems();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            InventorySteamworks.AddItems();
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            _case.RollCaseWithPredefinedItems(new List<InventoryItemData>()
            {
                InventoryAllItems.Instance.Items[2402],
                InventoryAllItems.Instance.Items[1203],
                InventoryAllItems.Instance.Items[1301],
                InventoryAllItems.Instance.Items[2204],
                InventoryAllItems.Instance.Items[1402],
                InventoryAllItems.Instance.Items[1201],
                InventoryAllItems.Instance.Items[1203],
                InventoryAllItems.Instance.Items[2204],
                InventoryAllItems.Instance.Items[2401],
                InventoryAllItems.Instance.Items[2201],
                InventoryAllItems.Instance.Items[2302],
                InventoryAllItems.Instance.Items[2205],
                InventoryAllItems.Instance.Items[2303],
                InventoryAllItems.Instance.Items[2202],
                InventoryAllItems.Instance.Items[2204],
                InventoryAllItems.Instance.Items[2202],
                InventoryAllItems.Instance.Items[2402],
                InventoryAllItems.Instance.Items[1203],
                InventoryAllItems.Instance.Items[1301],
                InventoryAllItems.Instance.Items[2204],
                InventoryAllItems.Instance.Items[1402],
                InventoryAllItems.Instance.Items[1201],
                InventoryAllItems.Instance.Items[1201],
            });
        }
    }*/
}
