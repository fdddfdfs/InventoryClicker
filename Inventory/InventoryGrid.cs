using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class InventoryGrid
{
    protected InventorySteamworks _inventorySteamworks;
    protected List<GameObject> _inventoryCells;
    protected TMP_Text _nameText;
    protected TMP_Text _descriptionText;
    protected Button _button;
    protected TMP_Text _buttonText;
    protected Button _nextPageButton, _previousPageButton;
    protected InventoryItemType _inventoryItemsType;

    protected List<Image> _inventoryCellsBackground;
    protected List<Image> _inventoryCellsImages;
    protected List<Button> _inventoryCellsButtons;
    protected List<TMP_Text> _inventoryCellsStackText;

    protected List<InventoryItemData> _items;
    protected List<uint> _itemsCount;
    //protected List<List<ulong>> _itemsIDs;
    protected List<List<InventoryItem>> _inventoryItems;

    protected List<int> _defaultItemsIDs;
    private readonly Dictionary<InventoryItemRarityType, Color> _rarityColors;

    private int _currentPage = 0;
    private int _numberOfPages = 1;

    private Sprite _invisibleSprite;
    private Color _defaultBackgroundColor;

    public bool IsActive { get; private set; } = false;

    public InventoryGrid(
        InventorySteamworks inventorySteamworks,
        List<GameObject> inventoryCells,
        TMP_Text nameText,
        TMP_Text descriptionText,
        Button button,
        Button nextPageButton,
        Button previousPageButton,
        Sprite invisibleSprite,
        List<int> defaultItemsIds,
        Dictionary<InventoryItemRarityType, Color> rarityColors)
    {
        _inventorySteamworks = inventorySteamworks;
        _inventoryCells = inventoryCells;
        _nameText = nameText;
        _descriptionText = descriptionText;
        _button = button;
        _nextPageButton = nextPageButton;
        _previousPageButton = previousPageButton;
        _invisibleSprite = invisibleSprite;
        _defaultItemsIDs = defaultItemsIds;
        _rarityColors = rarityColors;

        _buttonText = _button.GetComponentInChildren<TMP_Text>();

        _inventoryCellsBackground = new List<Image>();
        _inventoryCellsImages = new List<Image>();
        _inventoryCellsButtons = new List<Button>();
        _inventoryCellsStackText = new List<TMP_Text>();
        
        _items = new List<InventoryItemData>();
        _itemsCount = new List<uint>();
        //_itemsIDs = new List<List<ulong>>();
        _inventoryItems = new List<List<InventoryItem>>();
        
        for (int i = 0; i < _inventoryCells.Count; i++)
        {
            _inventoryCellsBackground.Add(_inventoryCells[i].transform.parent.GetComponent<Image>());
            _inventoryCellsImages.Add(_inventoryCells[i].GetComponent<Image>());
            _inventoryCellsButtons.Add(_inventoryCells[i].GetComponent<Button>());
            _inventoryCellsButtons[^1].enabled = false;
            _inventoryCellsStackText.Add(_inventoryCells[i].GetComponentInChildren<TMP_Text>());

            _inventoryCellsStackText[^1].text = string.Empty;
        }

        _defaultBackgroundColor = _inventoryCellsBackground[0].color;

        _inventorySteamworks.InventoryLoaded += InitializeInventory;
        _inventorySteamworks.InventoryAddItem += AddItemInInventory;
        _inventorySteamworks.InventoryRemoveItem += RemoveItemFromInventory;


        if (!SteamManager.Initialized)
        {
            Debug.Log("Not initialized");
            InitializeInventory(new List<InventoryItem>());
        }
    }

    public void ClearEvents()
    {
        _inventorySteamworks.InventoryLoaded -= InitializeInventory;
        _inventorySteamworks.InventoryAddItem -= AddItemInInventory;
        _inventorySteamworks.InventoryRemoveItem -= RemoveItemFromInventory;
    }

    public void OpenInventory()
    {
        IsActive = true;
        ShowItems();
    }

    public virtual void CloseInventory()
    {
        IsActive = false;
        ClearItems();
    }

    protected virtual void InitializeInventory(List<InventoryItem> inventoryItems)
    {
        AddDefaultItems();

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].InventoryItemData.InventoryItemType != _inventoryItemsType)
            {
                continue;
            }

            _items.Add(inventoryItems[i].InventoryItemData);
            _itemsCount.Add(inventoryItems[i].Quantity);
            _inventoryItems.Add(new List<InventoryItem>() { inventoryItems[i] });
            //_itemsIDs.Add(new List<ulong>() { inventoryItems[i].SteamID });
        }

        StackItems();

        _numberOfPages = Mathf.CeilToInt((float)_items.Count /_inventoryCells.Count);

        if (IsActive)
        {
            ShowItems();
        }
    }

    protected virtual void RemoveItemFromInventory(List<InventoryItem> removedItems)
    {
        if (IsActive)
        {
            ClearItems();
        }

        for (int i = 0; i < removedItems.Count; i++)
        {
            if (removedItems[i].InventoryItemData.InventoryItemType != _inventoryItemsType)
                continue;

            int removedItemIndex = _items.FindIndex(
                item => item.ID == removedItems[i].InventoryItemData.ID);

            if (removedItems[i].Quantity == 0 && _inventoryItems[removedItemIndex].Count == 1)
            {
                _items.RemoveAt(removedItemIndex);
                _itemsCount.RemoveAt(removedItemIndex);
                _inventoryItems.RemoveAt(removedItemIndex);
                
            }
            else
            {
                int decreasedQuantityItemIndex = _inventoryItems[removedItemIndex]
                    .FindIndex((removedItem) => removedItems[i].SteamID == removedItem.SteamID);

                InventoryItem decreasedQuantityItem = _inventoryItems[removedItemIndex][decreasedQuantityItemIndex];
                
                uint delta = decreasedQuantityItem.Quantity - removedItems[i].Quantity;
                _itemsCount[removedItemIndex] -= delta;

                if (removedItems[i].Quantity == 0)
                {
                    _inventoryItems[removedItemIndex].RemoveAt(decreasedQuantityItemIndex);
                }
                else
                {
                    _inventoryItems[removedItemIndex][decreasedQuantityItemIndex] = removedItems[i];
                }
            }
        }

        _numberOfPages = Mathf.CeilToInt((float)_items.Count / _inventoryCells.Count);

        if (IsActive)
        {
            ShowItems();
            ClearSelection();
        }
    }

    protected virtual void AddItemInInventory(List<InventoryItem> addedItems)
    {
        if(IsActive) ClearItems();

        for (int i = 0; i < addedItems.Count; i++)
        {
            if (addedItems[i].InventoryItemData.InventoryItemType != _inventoryItemsType)
                continue;

            int addedItemIndex = _items.FindIndex(
                item => item.ID == addedItems[i].InventoryItemData.ID);

            if (addedItemIndex == -1)
            {
                _items.Add(addedItems[i].InventoryItemData);
                _itemsCount.Add(addedItems[i].Quantity);
                _inventoryItems.Add(new List<InventoryItem>(){addedItems[i]});
                //_itemsIDs.Add(new List<ulong>{addedItems[i].SteamID});
            }
            else
            {
                _itemsCount[addedItemIndex] += addedItems[i].Quantity;
                _inventoryItems[addedItemIndex].Add(addedItems[i]);
                //_itemsIDs[addedItemIndex].Add(addedItems[i].SteamID);
            }
        }

        _numberOfPages = Mathf.CeilToInt((float)_items.Count / _inventoryCells.Count);

        if (IsActive)
        {
            ShowItems();
            ClearSelection();
        }
    }

    protected virtual void ClearSelection()
    {
        _button.gameObject.SetActive(false);
        _button.onClick.RemoveAllListeners();
        _nameText.text = "";
        _descriptionText.text = "";
    }

    private void ShowItems()
    {
        int numberOfItems = Mathf.Min(_items.Count - _currentPage * _inventoryCells.Count, _inventoryCells.Count);

        for (var i = 0; i < numberOfItems; i++)
        {
            _inventoryCellsImages[i].sprite = _items[i + _currentPage * _inventoryCells.Count].Icon;

            int itemNumber = i + _currentPage * _inventoryCells.Count;
            _inventoryCellsButtons[i].enabled = true;
            _inventoryCellsButtons[i].onClick.AddListener(() => ShowItemInfo(itemNumber));

            uint count = _itemsCount[i + _currentPage * _inventoryCells.Count];
            _inventoryCellsStackText[i].text = $"x{count.ToString()}";

            _inventoryCellsBackground[i].color =
                _rarityColors[
                    _items[i + _currentPage * _inventoryCells.Count].InventoryItemRarityType];
        }
        
        _nextPageButton.onClick.RemoveAllListeners();
        _previousPageButton.onClick.RemoveAllListeners();
        _nextPageButton.onClick.AddListener(() => ChangePage(1));
        _previousPageButton.onClick.AddListener(() => ChangePage(-1));

        ChangePageButtonsVisible();

        ClearSelection();
    }

    private void ClearItems()
    {
        int numberOfItems = Mathf.Min(_items.Count - _currentPage * _inventoryCells.Count, _inventoryCells.Count);

        for (int i = 0; i < numberOfItems; i++)
        {
            if (_inventoryCellsImages[i] != null)
            {
                _inventoryCellsImages[i].sprite = _invisibleSprite;
            }

            _inventoryCellsButtons[i].onClick.RemoveAllListeners();
            _inventoryCellsButtons[i].enabled = false;

            if (_inventoryCellsStackText[i] != null)
            {
                _inventoryCellsStackText[i].text = string.Empty;
            }

            _inventoryCellsBackground[i].color = _defaultBackgroundColor;
        }

        _nextPageButton.onClick.RemoveAllListeners();
        _previousPageButton.onClick.RemoveAllListeners();
    }

    private void StackItems()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            for (int j = _inventoryItems.Count - 1; j > i; j--)
            {
                if (_inventoryItems[j].Count != 1)
                {
                    throw new Exception("Inventory cannot have more then 1 item after initialization");
                }

                if (_items[i].ID == _inventoryItems[j][0].InventoryItemData.ID)
                {
                    _itemsCount[i] += _itemsCount[j];
                    _inventoryItems[i].Add(_inventoryItems[j][0]);
                    
                    _items.RemoveAt(j);
                    _inventoryItems.RemoveAt(j);
                    _itemsCount.RemoveAt(j);
                }
            }
        }
    }

    protected virtual void ShowItemInfo(int itemNumber)
    {
        _nameText.text = Localization.Instance[_items[itemNumber].Name];
        _descriptionText.text = Localization.Instance[_items[itemNumber].Description];
    }

    private void ChangePage(int dir)
    {
        ClearItems();
        
        _currentPage += dir;
        
        ShowItems();
    }

    private void ChangePageButtonsVisible()
    {
        if (_currentPage == 0)
        {
            _previousPageButton.gameObject.SetActive(false);
            _previousPageButton.enabled = false;
        }
        else if (!_previousPageButton.gameObject.activeSelf)
        {
            _previousPageButton.gameObject.SetActive(true);
            _previousPageButton.enabled = true;
        }

        if (_currentPage >= _numberOfPages - 1)
        {
            _nextPageButton.gameObject.SetActive(false);
            _nextPageButton.enabled = false;
        }
        else if (!_nextPageButton.gameObject.activeSelf)
        {
            _nextPageButton.gameObject.SetActive(true);
            _nextPageButton.enabled = true;
        }
    }

    protected virtual void AddDefaultItems()
    {
        if (_defaultItemsIDs == null)
            return;

        for (var i = 0; i < _defaultItemsIDs.Count; i++)
        {
            _items.Add(InventoryAllItems.Instance.Items[_defaultItemsIDs[i]]);
            _itemsCount.Add(1);
            _inventoryItems.Add(new List<InventoryItem>
                { new(0, InventoryAllItems.Instance.Items[_defaultItemsIDs[i]], 1) });
            //_itemsIDs.Add(new List<ulong>() { 0 });
        }
    }
}
