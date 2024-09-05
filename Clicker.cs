using HamsterCombat.Storage;
using UnityEngine;

namespace HamsterCombat
{
    public class Clicker
    {
        private Target _target;
        private Counter _counter;

        public Target Target => _target;
        
        public Clicker(Canvas canvas, int startTarget)
        {
            Transform parent = canvas.transform;
            _target = new Target(parent, "UI/TargetView");
            _target.ChangeTarget(ProgressStorage.CurrentSkin.Value != 0 ? 
                InventoryAllItems.Instance.Items[ProgressStorage.CurrentSkin.Value] :
                InventoryAllItems.Instance.Items[startTarget]);
            _counter = new Counter(parent, "UI/Counter", ProgressStorage.ClicksCount.Value);

            _target.OnTargetClick += () => ProgressStorage.ClicksCount.Value += 1;
            _target.OnTargetClick += () => _counter.AddCount();
            _target.OnTargetClick += () => Achievements.Instance.GetAchievement("FirstClick");
        }
    }
}