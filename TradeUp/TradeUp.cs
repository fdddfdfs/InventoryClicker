using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HamsterCombat
{
    public class TradeUp : MenuWithView<TradeUpView>
    {
        public const int PickableItemsCount = 9;
        
        private const string TradeUpPickItemViewResourceName = "UI/TradeUp/TradeUpPickItemView";
        private const string TradeUpPickableItemViewResourceName = "UI/TradeUp/TradeUpPickableItemView";
        
        private readonly List<TradeUpPickableItem> _pickableItems;
        private readonly TradeUpPickItem _tradeUpPickItem;
        private readonly List<InventoryItem> _currentTradeUp;
        private readonly Inventory _inventory;
        private readonly Dictionary<InventoryItemRarityType, int> _tradeUpIDs;

        private TradeUpPickableItem _currentChangingPickableItemItem;
        private InventoryItem _currentChangingItem;
        private InventoryItemRarityType _currentRarity;
        
        public TradeUp(Transform parent,
            string menuViewResourceName,
            Inventory inventory,
            Sprite empty,
            Dictionary<InventoryItemRarityType, int> tradeUpIDs,
            HashSet<InventoryItemRarityType> unusableRarities,
            Dictionary<InventoryItemRarityType, Color> rarityColors) 
            : base(parent, menuViewResourceName)
        {
            _inventory = inventory;
            _tradeUpIDs = tradeUpIDs;
            _tradeUpPickItem = new TradeUpPickItem(
                _view.transform,
                TradeUpPickItemViewResourceName,
                inventory,
                empty,
                unusableRarities,
                rarityColors);
            _tradeUpPickItem.OnItemPicked += PickItem;
            _tradeUpPickItem.ChangeMenuActive(false);
            
            _pickableItems = new List<TradeUpPickableItem>();
            
            for (int i = 0; i < PickableItemsCount; i++)
            {
                TradeUpPickableItem pickableItem = 
                    new(_view.ItemsParent, TradeUpPickableItemViewResourceName, empty, rarityColors);
                _pickableItems.Add(pickableItem);
                pickableItem.OnItemPicked += (item) => ChangeItem(item, pickableItem);
            }

            _currentTradeUp = new List<InventoryItem>();
            
            _view.Init(PickableItemsCount);

            _view.OnClear += ClearTradeUp;
            _view.OnTradeUp += TryTradeUp;
        }

        private void ClearTradeUp()
        {
            foreach (TradeUpPickableItem pickableItem in _pickableItems)
            {
                pickableItem.ClearItem(true);
            }

            foreach (InventoryItem inventoryItem in (_currentTradeUp))
            {
                _tradeUpPickItem.ReturnItem(inventoryItem);
            }

            _currentTradeUp.Clear();
            _view.SetProgress(_currentTradeUp.Count);
            _view.ChangeTradeUpButtonActive(_currentTradeUp.Count == PickableItemsCount);
            
            if(_tradeUpPickItem.IsActive) _tradeUpPickItem.ShowAllItems();
        }

        private void TryTradeUp()
        {
            if (_currentTradeUp.Count != PickableItemsCount) return;

            _inventory.InventorySteamworks.TradeUp(
                _currentTradeUp.Select((item) => item.SteamID).ToList(),
                _tradeUpIDs[_currentRarity]);
            
            ClearTradeUp();
        }

        private void ChangeItem(InventoryItem item, TradeUpPickableItem tradeUpPickableItem)
        {
            if (_currentChangingPickableItemItem == tradeUpPickableItem)
            {
                _tradeUpPickItem.ChangeMenuActive(false);
                _currentChangingItem = default;
                _currentChangingPickableItemItem = null;

                return;
            }
            
            _tradeUpPickItem.ChangeMenuActive(true);
            if (_currentTradeUp.Count == 0 || 
                (_currentTradeUp.Count == 1 && _currentTradeUp[0].Equals(item)))
            {
                _tradeUpPickItem.ShowAllItems();
            }
            else
            {
                _tradeUpPickItem.ShowItemsByFilter(_currentRarity);
            }
            
            _currentChangingItem = item;
            _currentChangingPickableItemItem = tradeUpPickableItem;

            float scaleX = Screen.width / 1920f;
            float scaleY = Screen.height / 1080f;

            Rect cellRect = tradeUpPickableItem.ViewRectTransform.rect;
            Vector3 cellOffset = new (-cellRect.width/2 * scaleX, cellRect.height/2 * scaleY);
            Vector3 pos = tradeUpPickableItem.ViewRectTransform.transform.position + cellOffset;

            Rect rect = _tradeUpPickItem.ViewRectTransform.rect;
            pos -= new Vector3(rect.width/2 * scaleX, rect.height / 2 * scaleY);

            if (pos.y - rect.height / 2 * scaleY < 0) pos += new Vector3(0, rect.height / 2 * scaleY, 0);
            
            _tradeUpPickItem.Move(pos);
        }

        private void PickItem(InventoryItem item)
        {
            _tradeUpPickItem.ChangeMenuActive(false);
            
            if (!_currentChangingItem.Equals(default(InventoryItem)))
            {
                _tradeUpPickItem.ReturnItem(_currentChangingItem);
                _currentTradeUp.Remove(_currentChangingItem);
            }
            
            _currentTradeUp.Add(item);
            _currentChangingPickableItemItem.ChangeItem(item);
            
            _view.SetProgress(_currentTradeUp.Count);

            _view.ChangeTradeUpButtonActive(_currentTradeUp.Count == PickableItemsCount);
            _currentRarity = item.InventoryItemData.InventoryItemRarityType;
        }
    }
}