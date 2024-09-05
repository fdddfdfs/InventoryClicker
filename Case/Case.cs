using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace HamsterCombat
{
    public class Case : MenuWithView<CaseView>
    {
        private const float WaitBeforeClose = 1f;
        private const float PreventInfiniteRollAfterDuration = 20f;
        
        private readonly InventorySteamworks _inventorySteamworks;
        private readonly ItemReceived _itemReceived;
        private readonly Error _error;

        private CancellationTokenSource _preventInfiniteRoll;

        public Case(
            Transform parent,
            string menuViewResourceName,
            InventorySteamworks inventorySteamworks,
            Dictionary<InventoryItemRarityType, Color> rarityColors,
            ItemReceived itemReceived,
            Error error) 
            : base(parent, menuViewResourceName)
        {
            _inventorySteamworks = inventorySteamworks;
            _itemReceived = itemReceived;
            _error = error;

            _view.Init(rarityColors);
            _view.OnStopRolling += StopRolling;
            
            _preventInfiniteRoll =
                CancellationTokenSource.CreateLinkedTokenSource(AsyncUtils.Instance.GetCancellationToken());
        }

        public void RollCaseWithPredefinedItems(List<InventoryItemData> inventoryChestItemData)
        {
            ChangeMenuActive(true);
            _view.StartAnimationWithPredefinedItems(inventoryChestItemData);
        }

        public void OpenCase(InventoryChestItemData openedCase)
        {
            ChangeMenuActive(true);
            _inventorySteamworks.InventoryAddItem += InventoryAddItem;
            _view.StartAnimation(openedCase);
            _itemReceived.BlockShowing();

            PreventInfiniteRoll();
        }
        
        private async void StopRolling()
        {
            CancellationToken token = AsyncUtils.Instance.GetCancellationToken();

            await AsyncUtils.Instance.Wait(WaitBeforeClose);

            if (token.IsCancellationRequested) return;
            
            ChangeMenuActive(false);
            _itemReceived.UnblockShowing();
        }

        private void InventoryAddItem(List<InventoryItem> inventoryItems)
        {
            _inventorySteamworks.InventoryAddItem -= InventoryAddItem;
            _view.StopAnimation(inventoryItems[0].InventoryItemData);
            
            _preventInfiniteRoll.Cancel();
        }

        private async void PreventInfiniteRoll()
        {
            if (_preventInfiniteRoll.IsCancellationRequested)
            {
                _preventInfiniteRoll =
                    CancellationTokenSource.CreateLinkedTokenSource(AsyncUtils.Instance.GetCancellationToken());
            }

            CancellationToken token = _preventInfiniteRoll.Token;

            await AsyncUtils.Instance.Wait(PreventInfiniteRollAfterDuration, token);

            if (token.IsCancellationRequested) return;
            
            _error.ShowError();
        }
    }
}