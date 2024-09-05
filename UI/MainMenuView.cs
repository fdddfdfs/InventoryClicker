using System;
using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _inventory;
        [SerializeField] private Button _settings;
        [SerializeField] private Button _leaderboard;
        [SerializeField] private Button _tradeUp;

        public event Action OnInventoryClick;
        public event Action OnSettingsClick;
        public event Action OnLeaderboardClick;
        public event Action OnTradeUpClick;

        private void Awake()
        {
            _inventory.onClick.AddListener(() => OnInventoryClick?.Invoke());
            _settings.onClick.AddListener(() => OnSettingsClick?.Invoke());
            _leaderboard.onClick.AddListener(()=> OnLeaderboardClick?.Invoke());
            _tradeUp.onClick.AddListener(()=>OnTradeUpClick?.Invoke());
        }
    }
}