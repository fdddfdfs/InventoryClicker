using UnityEngine;

namespace HamsterCombat
{
    [CreateAssetMenu(fileName = "InventoryKeyItemData", menuName = "Inventory/InventoryKeyItemData")]
    public class InventoryKeyItemData : InventoryItemData
    {
        public override InventoryItemType InventoryItemType => InventoryItemType.Key;
    }
}