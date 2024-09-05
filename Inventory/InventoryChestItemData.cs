using System.Collections.Generic;
using HamsterCombat;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryChestItemData", menuName = "Inventory/InventoryChestItemData")]
public sealed class InventoryChestItemData : InventoryItemData
{
    [SerializeField] private int _chestGeneratorID;
    [SerializeField] private List<InventoryClothesItemData> _possibleOutcomes;
    [SerializeField] private InventoryKeyItemData _requiredKey;

    public override InventoryItemType InventoryItemType => InventoryItemType.Chest;

    public int ChestGeneratorID => _chestGeneratorID;
    
    public List<InventoryClothesItemData> PossibleOutcomes => _possibleOutcomes;
    
    public InventoryKeyItemData RequiredKey => _requiredKey;
}