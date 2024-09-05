public readonly struct InventoryItem
{
    public ulong SteamID { get; }
    public InventoryItemData InventoryItemData { get; }
    
    public uint Quantity { get; }

    public InventoryItem(ulong steamID, InventoryItemData inventoryItemData, uint quantity)
    {
        InventoryItemData = inventoryItemData;
        SteamID = steamID;
        Quantity = quantity;
    }
}
