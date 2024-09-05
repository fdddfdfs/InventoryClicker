using System.Threading;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;

namespace HamsterCombat
{
    public class Drops
    {
        private const float DropCheckDelay = 60f;
        private const float PromoCheckDelay = 60f;
        private const float PlaytimeGeneratorDropDelay = 10f;
        
        private readonly Inventory _inventory;
        private readonly int _dlcPlaytimeGenerator;
        private readonly DLCValidation _dlcValidation;

        public Drops(Inventory inventory,  int dlcPlaytimeGenerator, DLCValidation dlcValidation)
        {
            _inventory = inventory;
            _dlcPlaytimeGenerator = dlcPlaytimeGenerator;
            _dlcValidation = dlcValidation;

            DropChecker();
            PromoChecker();
            ManualPromoCheck();
        }

        private async void DropChecker()
        {
            CancellationToken token = AsyncUtils.Instance.GetCancellationToken();
            
            if (SteamManager.Initialized)
            {
                foreach (int playtimeGeneratorID in InventoryAllItems.Instance.PlaytimeGeneratorsIDs)
                {
                    await AsyncUtils.Instance.Wait(PlaytimeGeneratorDropDelay, true);
                    
                    if (token.IsCancellationRequested) return;
                    
                    SteamInventory.TriggerItemDrop(
                        out SteamInventoryResult_t result,
                        new SteamItemDef_t(playtimeGeneratorID));
                    
                    Debug.Log("Dropped");
                    
                    _inventory.SetInventoryResult(result);
                }
            }

            await AsyncUtils.Instance.Wait(DropCheckDelay, true);

            if (token.IsCancellationRequested) return;

            DropChecker();
        }

        private async void PromoChecker()
        {
            CancellationToken token = AsyncUtils.Instance.GetCancellationToken();
            
            await AsyncUtils.Instance.Wait(PromoCheckDelay, true);
                
            if (token.IsCancellationRequested) return;
            
            if (SteamManager.Initialized)
            {
                CheckForPromoItems();
            }
        }

        private void CheckForPromoItems()
        {
            SteamInventory.GrantPromoItems(out SteamInventoryResult_t steamInventoryResult);
            
            _inventory.InventorySteamworks.SetInventoryResult(steamInventoryResult);
        }

        private async void ManualPromoCheck()
        {
            CancellationToken token = AsyncUtils.Instance.GetCancellationToken();
            
            await AsyncUtils.Instance.Wait(PromoCheckDelay, true);

            if (token.IsCancellationRequested) return;

            if (_dlcValidation.HasLicense)
            {
                SteamInventory.TriggerItemDrop(
                    out SteamInventoryResult_t result,
                    new SteamItemDef_t(_dlcPlaytimeGenerator));
                            
                _inventory.InventorySteamworks.SetInventoryResult(result);
                
                Debug.Log("DLC Dropped");
            }
            
            ManualPromoCheck();
        }
    }
}