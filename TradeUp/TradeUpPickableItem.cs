using System;
using System.Collections.Generic;
using UnityEngine;

namespace HamsterCombat
{
    public class TradeUpPickableItem : MenuWithView<TradeUpPickableItemView>
    {
        private readonly Sprite _empty;
        private readonly Dictionary<InventoryItemRarityType, Color> _rarityColors;

        private InventoryItem _inventoryItem;
        
        public event Action<InventoryItem> OnItemPicked;

        public RectTransform ViewRectTransform => _view.RectTransform;
        
        public TradeUpPickableItem(
            Transform parent,
            string menuViewResourceName,
            Sprite empty,
            Dictionary<InventoryItemRarityType, Color> rarityColors) 
            : base(parent, menuViewResourceName)
        {
            _view.OnButtonClicked += () => OnItemPicked?.Invoke(_inventoryItem);
            _empty = empty;
            _rarityColors = rarityColors;
            _view.ChangeIcon(empty);
            _view.ChangeBackgroundColor(Color.white);
        }

        public void ChangeItem(InventoryItem item)
        {
            _inventoryItem = item;
            
            _view.ChangeIcon(item.InventoryItemData.Icon);
            _view.ChangeAvailability(true);
            _view.ChangeBackgroundColor(_rarityColors[item.InventoryItemData.InventoryItemRarityType]);
        }

        public void ClearItem(bool availability = false)
        {
            _view.ChangeIcon(_empty);
            _view.ChangeAvailability(availability);
            _inventoryItem = default;
            _view.ChangeBackgroundColor(Color.white);
        }
    }
}