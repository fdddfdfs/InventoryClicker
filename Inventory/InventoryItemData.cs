using UnityEngine;

public abstract class InventoryItemData : ScriptableObject, IWeightable
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _weight;
    [SerializeField] private InventoryItemRarityType _inventoryItemRarityType;

    public int ID => _id;
    public string Name => _name;
    public string Description => _description;
    public abstract InventoryItemType InventoryItemType { get; }
    public Sprite Icon => _icon;
    public int Weight => _weight;
    public InventoryItemRarityType InventoryItemRarityType => _inventoryItemRarityType;
}