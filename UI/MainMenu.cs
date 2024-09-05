using Leaderboard;
using UnityEngine;

namespace HamsterCombat
{
    public class MainMenu : MenuWithView<MainMenuView>
    {
        private IMenu _currentMenu;

        public MainMenu(Transform parent, string menuViewResourceName, Inventory inventory, LeaderboardController leaderboard, TradeUp tradeUp, Settings settings) : base(parent,
            menuViewResourceName)
        {
            _view.OnSettingsClick += () => ChangeMenu(settings);
            _view.OnLeaderboardClick += () => ChangeMenu(leaderboard);
            _view.OnTradeUpClick += () => ChangeMenu(tradeUp);
            _view.OnInventoryClick += () => ChangeMenu(inventory);
            
            inventory.ChangeMenuActive(false);
            leaderboard.ChangeMenuActive(false);
            settings.ChangeMenuActive(false);
            tradeUp.ChangeMenuActive(false);
        }

        private void ChangeMenu(IMenu newMenu)
        {
            if (_currentMenu == newMenu)
            {
                _currentMenu.ChangeMenuActive();
                return;
            }
            
            _currentMenu?.ChangeMenuActive(false);
            
            _currentMenu = newMenu;
            
            _currentMenu.ChangeMenuActive(true);
        }
    }
}