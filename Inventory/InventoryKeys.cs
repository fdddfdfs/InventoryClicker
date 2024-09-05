using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class InventoryKeys : InventoryGrid
    {
        public InventoryKeys(
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
            : base(
                inventorySteamworks,
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
            _inventoryItemsType = InventoryItemType.Key;
        }
    }
}