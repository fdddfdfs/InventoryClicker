using UnityEngine;

[CreateAssetMenu(fileName = "InventoryClothesItemData", menuName = "Inventory/InventoryClothesItemData")]
public sealed class InventoryClothesItemData : InventoryItemData
{
    public override InventoryItemType InventoryItemType => InventoryItemType.Clothes;
}