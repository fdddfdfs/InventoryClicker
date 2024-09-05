using UnityEngine;

namespace HamsterCombat
{
    public class Counter : MenuWithView<CounterView>
    {
        private int _currentCount;        
        
        public Counter(Transform parent, string menuViewResourceName, int startCount) : base(parent, menuViewResourceName)
        {
            AddCount(startCount);
        }

        public void AddCount(int delta = 1)
        {
            _currentCount += delta;
            _view.UpdateCounter(_currentCount);
        }
    }
}