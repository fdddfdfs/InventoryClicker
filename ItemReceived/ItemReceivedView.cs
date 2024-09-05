using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class ItemReceivedView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _header;
        [SerializeField] private Image _background;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _accept;
        [SerializeField] private TMP_Text _acceptText;

        public event Action OnAccept;

        private void Awake()
        {
            _accept.onClick.AddListener(() => OnAccept?.Invoke());
        }

        public void SetItem(Sprite item, Color rarity)
        {
            _icon.sprite = item;
            _background.color = rarity;
        }
    }
}