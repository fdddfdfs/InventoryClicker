using System;
using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class TradeUpPickableItemView : MonoBehaviour
    {
        [SerializeField] private Button _item;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Image _itemBackground;

        public event Action OnButtonClicked;

        public RectTransform RectTransform { get; private set; }

        public void ChangeIcon(Sprite icon)
        {
            _itemIcon.sprite = icon;
        }

        public void ChangeBackgroundColor(Color color)
        {
            _itemBackground.color = color;
        }

        public void ChangeAvailability(bool availability)
        {
            _item.enabled = availability;
        }
        
        private void Awake()
        {
            _item.onClick.AddListener(() => OnButtonClicked?.Invoke());
            RectTransform = GetComponent<RectTransform>();
        }
    }
}