using TMPro;
using UnityEngine;

namespace HamsterCombat
{
    public class CounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _counter;
        
        public void UpdateCounter(int newValue)
        {
            _counter.text = newValue.ToString();
        }
    }
}