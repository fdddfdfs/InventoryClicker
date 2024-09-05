using UnityEngine;

namespace HamsterCombat
{
    public class Settings : MenuWithView<SettingsView>
    {
        public Settings(Transform parent, string menuViewResourceName) : base(parent, menuViewResourceName)
        {
            
        }
    }
}