namespace HamsterCombat.Storage
{
    public static class ProgressStorage
    {
        public static DataInt ClicksCount = new (nameof(ClicksCount), 0);

        public static DataInt InventorySize = new(nameof(InventorySize), 0);

        public static DataInt CurrentSkin = new (nameof(CurrentSkin), 0);
    }
}