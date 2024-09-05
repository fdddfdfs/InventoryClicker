using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HamsterCombat
{
    public class TradeUpPickItem : MenuWithView<TradeUpPickItemView>
    {
        private readonly Sprite _empty;
        private readonly HashSet<InventoryItemRarityType> _unusableRarities;
        private readonly Dictionary<InventoryItemRarityType, Color> _rarityColors;
        private const string TradeUpPickableItemViewResourceName = "UI/TradeUp/TradeUpPickableItemView";
        private const int AlwaysShownItemsSlots = 24;
        
        private readonly List<TradeUpPickableItem> _pickableItems;
        private readonly List<InventoryItem> _items;
        private readonly Dictionary<InventoryItemRarityType, List<InventoryItem>> _itemsByRarity;
        private readonly List<InventoryItem> _pickedItems;

        private InventoryItemRarityType _currentShownFilter;
        private bool _filterExist;
        
        public event Action<InventoryItem> OnItemPicked;

        public RectTransform ViewRectTransform { get; }

        public TradeUpPickItem(
            Transform parent,
            string menuViewResourceName,
            Inventory inventory,
            Sprite empty,
            HashSet<InventoryItemRarityType> unusableRarities,
            Dictionary<InventoryItemRarityType, Color> rarityColors) 
            : base(parent, menuViewResourceName)
        {
            _empty = empty;
            _unusableRarities = unusableRarities;
            _rarityColors = rarityColors;
            _pickableItems = new List<TradeUpPickableItem>();
            _items = new List<InventoryItem>();
            _pickedItems = new List<InventoryItem>();
            
            _itemsByRarity = new Dictionary<InventoryItemRarityType, List<InventoryItem>>();
            foreach (InventoryItemRarityType rarity in Enum.GetValues(typeof(InventoryItemRarityType)))
            {
                _itemsByRarity[rarity] = new List<InventoryItem>();
            }
            
            inventory.InventorySteamworks.InventoryLoaded += InitializeItems;
            inventory.InventorySteamworks.InventoryAddItem += AddItems;
            inventory.InventorySteamworks.InventoryRemoveItem += RemoveItems;

            ViewRectTransform = _view.GetComponent<RectTransform>();

            for (int i = 0; i < AlwaysShownItemsSlots; i++)
            {
                CreateNewPickableItem();
            }
        }

        public void ShowAllItems()
        {
            _filterExist = false;
            ShowItems(_items);
        }

        public void ShowItemsByFilter(InventoryItemRarityType filter)
        {
            _currentShownFilter = filter;
            _filterExist = true;
            ShowItems(_itemsByRarity[filter]);
        }

        public void ReturnItem(InventoryItem item)
        {
            bool result = _pickedItems.Remove(item);

            if (!result) throw new Exception("Trying return item which is not picked");
            
            _items.Add(item);
            _itemsByRarity[((InventoryClothesItemData)item.InventoryItemData).InventoryItemRarityType].Add(item);
            
            UpdateShownItems();
        }

        public void Move(Vector3 pos)
        {
            _view.transform.position = pos;
        }

        private void ShowItems(List<InventoryItem> items)
        {
            int delta = items.Count - _pickableItems.Count;
            
            for (int i = 0; i < delta; i++)
            {
                CreateNewPickableItem();
            }
            
            for (int i = 0; i < items.Count; i++)
            {
                _pickableItems[i].ChangeMenuActive(true);
                _pickableItems[i].ChangeItem(items[i]);
            }

            for (int i = items.Count; i < _pickableItems.Count; i++)
            {
                if (i < AlwaysShownItemsSlots)
                {
                    _pickableItems[i].ChangeMenuActive(true);
                    _pickableItems[i].ClearItem();
                }
                else
                {
                    _pickableItems[i].ChangeMenuActive(false);
                }
            }

            int width = Mathf.FloorToInt(ViewRectTransform.rect.width / _view.ItemsParentGridLayout.cellSize.x);
            ChangeParentSize(Mathf.CeilToInt((float)items.Count / width));
        }

        private void UpdateShownItems()
        {
            if (IsActive)
            {
                if (_filterExist)
                {
                    ShowItemsByFilter(_currentShownFilter);
                }
                else
                {
                    ShowAllItems();
                }
            }
        }

        private void MoveItemToPicked(InventoryItem pickedItem)
        {
            bool result = _items.Remove(pickedItem);
            result &= _itemsByRarity[pickedItem.InventoryItemData.InventoryItemRarityType]
                .Remove(pickedItem);

            if (!result) throw new Exception("Trying pick item which is not exist in items");

            _pickedItems.Add(pickedItem);
            
            UpdateShownItems();
        }

        private void RemoveItems(List<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                if (item.InventoryItemData is InventoryClothesItemData data)
                {
                    if (_unusableRarities.Contains(data.InventoryItemRarityType)) continue;
                    if (item.Quantity != 0) continue;

                    Predicate<InventoryItem> steamIDEqual = (storedItem) => item.SteamID == storedItem.SteamID;

                    int itemIndex = _items.FindIndex(steamIDEqual);
                    if (itemIndex >= 0)
                    {
                        _items.RemoveAt(itemIndex);
                    }
                    else
                    {
                        int pickedItemIndex = _pickedItems.FindIndex(steamIDEqual);
                        _pickedItems.RemoveAt(pickedItemIndex);
                    }

                    List<InventoryItem> itemsByRarity = _itemsByRarity[data.InventoryItemRarityType];
                    int itemByRarityIndex = itemsByRarity.FindIndex(steamIDEqual);
                    itemsByRarity.RemoveAt(itemByRarityIndex);
                }
            }

            UpdateShownItems();
        }

        private void AddItems(List<InventoryItem> items)
        {
            int clothesCount = 0;
            
            foreach (InventoryItem item in items)
            {
                if (item.InventoryItemData is InventoryClothesItemData data)
                {
                    if (_unusableRarities.Contains(data.InventoryItemRarityType)) continue;
                    
                    _items.Add(item);
                    clothesCount++;
                    _itemsByRarity[data.InventoryItemRarityType].Add(item);
                }
            }
            
            UpdateShownItems();
        }

        private void InitializeItems(List<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                if (item.InventoryItemData is InventoryClothesItemData data)
                {
                    if (_unusableRarities.Contains(data.InventoryItemRarityType)) continue;
                    
                    _items.Add(item);
                    _itemsByRarity[data.InventoryItemRarityType].Add(item);
                }
            }
        }

        private void CreateNewPickableItem()
        {
            TradeUpPickableItem pickableItem = 
                new(_view.ItemsParent, TradeUpPickableItemViewResourceName, _empty, _rarityColors);
            _pickableItems.Add(pickableItem);
            pickableItem.OnItemPicked += (item) => OnItemPicked?.Invoke(item);
            pickableItem.OnItemPicked += MoveItemToPicked;
        }

        private void ChangeParentSize(int newCount)
        {
            _view.ItemsParent.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                (_pickableItems[0].ViewRectTransform.rect.height + 
                 _view.ItemsParentGridLayout.spacing.y) *
                newCount +
                _view.ItemsParentGridLayout.padding.bottom +
                _view.ItemsParentGridLayout.padding.top);
        }
    }
}