using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class TradeUpView : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemsParent;

        [SerializeField] private Button _clear;
        [SerializeField] private Button _tradeUp;
        [SerializeField] private TMP_Text _progress;

        private int _maxProgress;

        public event Action OnClear;
        public event Action OnTradeUp;
        
        public RectTransform ItemsParent => _itemsParent;

        public void SetProgress(int progress)
        {
            _progress.text = $"{progress}/{_maxProgress}";
        }

        public void Init(int maxProgress)
        {
            _maxProgress = maxProgress;
            SetProgress(0);
        }

        public void ChangeTradeUpButtonActive(bool active)
        {
            _tradeUp.gameObject.SetActive(active);
        }
        
        private void Awake()
        {
            _clear.onClick.AddListener(()=> OnClear?.Invoke());
            _tradeUp.onClick.AddListener(()=>OnTradeUp?.Invoke());
        }
    }
}