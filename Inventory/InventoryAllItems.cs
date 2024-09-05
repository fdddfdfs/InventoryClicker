using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class InventoryAllItems : MonoBehaviour
{
    [SerializeField] private List<InventoryItemData> _inventoryItemsData;
    [SerializeField] private List<int> _playtimeGeneratorsIDs;

    public Dictionary<int, InventoryItemData> Items { get; private set; }

    public List<InventoryClothesItemData> ClothesItemData { get; private set; }
    
    public IReadOnlyList<int> PlaytimeGeneratorsIDs => _playtimeGeneratorsIDs;
    
    public static InventoryAllItems Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        Items = new Dictionary<int, InventoryItemData>();

        foreach (InventoryItemData inventoryItemData in _inventoryItemsData)
        {
            Items.Add(inventoryItemData.ID, inventoryItemData);
        }

        ClothesItemData = _inventoryItemsData.OfType<InventoryClothesItemData>().ToList();
    }
}
