using System;
using HamsterCombat.Storage;
using UnityEngine;

namespace HamsterCombat
{
    public class Target : MenuWithView<TargetView>
    {
        public event Action OnTargetClick;
        
        public Target(Transform parent, string menuViewResourceName) : base(parent, menuViewResourceName)
        {
            _view.OnTargetClick += () => OnTargetClick?.Invoke();
        }

        public void ChangeTarget(InventoryItemData newTarget)
        {
            _view.ChangeSprite(newTarget.Icon);
            ProgressStorage.CurrentSkin.Value = newTarget.ID;
        }
    }
}