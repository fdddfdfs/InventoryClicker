using System.Collections.Generic;
using UnityEngine;

namespace HamsterCombat
{
    public class ItemReceived : MenuWithView<ItemReceivedView>
    {
        private readonly Dictionary<InventoryItemRarityType, Color> _rarityColors;

        private readonly Queue<InventoryItem> _newItems;

        private bool _isBlocked;

        public ItemReceived(
            Transform parent,
            string menuViewResourceName,
            InventorySteamworks inventorySteamworks,
            Dictionary<InventoryItemRarityType, Color> rarityColors) 
            : base(parent, menuViewResourceName)
        {
            _rarityColors = rarityColors;
            inventorySteamworks.InventoryAddItem+= ReceiveNewItem;

            _newItems = new Queue<InventoryItem>();

            _view.OnAccept += () =>
            {
                if (_newItems.Count == 0)
                {
                    ChangeMenuActive(false);
                }
                else
                {
                    InventoryItem item = _newItems.Dequeue();
                    _view.SetItem(
                        item.InventoryItemData.Icon,
                        _rarityColors[item.InventoryItemData.InventoryItemRarityType]);
                }
            };
        }

        public void BlockShowing()
        {
            _isBlocked = true;
        }

        public void UnblockShowing()
        {
            _isBlocked = false;

            if (_newItems.Count != 0)
            {
                ChangeMenuActive(true);
                InventoryItem item = _newItems.Dequeue();
                _view.SetItem(
                    item.InventoryItemData.Icon,
                    _rarityColors[item.InventoryItemData.InventoryItemRarityType]);
            }
        }

        private void ReceiveNewItem(List<InventoryItem> items)
        {
            if (items.Count == 0) return;

            var startIndex = 0;
            
            if (!IsActive && !_isBlocked)
            {
                ChangeMenuActive(true);
                _view.SetItem(
                    items[0].InventoryItemData.Icon,
                    _rarityColors[items[0].InventoryItemData.InventoryItemRarityType]);
                
                startIndex = 1;
            }

            for (int i = startIndex; i < items.Count; i++)
            {
                _newItems.Enqueue(items[i]);
            }
        }
    }
}