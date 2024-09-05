using System;
using System.Collections.Generic;
using HamsterCombat.Storage;
using Leaderboard;
using Steamworks;
using UnityEngine;

namespace HamsterCombat
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private LeaderboardController _leaderboardController;
        [SerializeField] private List<TradeUpID> _tradeUps;
        [SerializeField] private List<InventoryRarityColor> _rarityColors;
        [SerializeField] private InventoryClothesItemData _startSkin;
        [SerializeField] private uint _dlcID;
        [SerializeField] private int _dlcPlaytimeGeneratorID;
        
        private Clicker _clicker;
        private Drops _drops;
        private Error _error;
        private SteamRichPresence _richPresence;
        private DLCValidation _dlcValidation;

        private void Start()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;

            _dlcValidation = new DLCValidation(_dlcID);
            
            Dictionary<InventoryItemRarityType, Color> rarityColors = new();

            foreach (InventoryRarityColor rarityColor in _rarityColors)
            {
                rarityColors[rarityColor.Rarity] = rarityColor.Color;
            }
            
            _clicker = new Clicker(_canvas, _startSkin.ID);
            _error = new Error(_canvas.transform, "UI/ErrorView");
            _error.SetMenuActive(false, false);

            _inventory.Init(_clicker.Target, _canvas.transform, rarityColors, _error);

            _drops = new Drops(_inventory, _dlcPlaytimeGeneratorID, _dlcValidation);

            Dictionary<InventoryItemRarityType, int> tradeUps = new();
            foreach (TradeUpID tradeUp in _tradeUps)
            {
                tradeUps[tradeUp.Rarity] = tradeUp.ID;
            }

            UI ui = new UI(
                _canvas,
                _inventory,
                _leaderboardController,
                tradeUps,
                new HashSet<InventoryItemRarityType>()
                {
                    InventoryItemRarityType.Legendary,
                    InventoryItemRarityType.Unique,
                    InventoryItemRarityType.Mythic,
                },
                rarityColors);

            _leaderboardController.Init();
            ProgressStorage.InventorySize.OnValueChanged += (value) =>
                _leaderboardController.UploadResult(
                    value,
                    ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            _leaderboardController.SetLeaderboardName("BestCollectors");

            _richPresence = new SteamRichPresence();
        }
        
        private void OnDestroy()
        {
            _dlcValidation.OnDestroy();
        }
    }
}
