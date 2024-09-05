using System.Collections.Generic;
using Leaderboard;
using UnityEngine;

namespace HamsterCombat
{
    public class UI
    {
        public UI(
            Canvas canvas,
            Inventory inventory,
            LeaderboardController leaderboard,
            Dictionary<InventoryItemRarityType, int> tradeUps,
            HashSet<InventoryItemRarityType> unusableRarities,
            Dictionary<InventoryItemRarityType, Color> rarityColors)
        {
            Transform canvasTransform = canvas.transform;
            Settings settings = new (canvasTransform, "UI/Settings");
            TradeUp tradeUp = new (
                canvasTransform, 
                "UI/TradeUp/TradeUpView",
                inventory,
                null,
                tradeUps,
                unusableRarities,
                rarityColors);
            MainMenu mainMenu = new (
                canvasTransform,
                "UI/MainMenu",
                inventory,
                leaderboard,
                tradeUp,
                settings);
        }
    }
}