using System;
using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class TradeUpPickItemView : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemsParent;
        
        public RectTransform ItemsParent => _itemsParent;
        
        public GridLayoutGroup ItemsParentGridLayout { get; private set; }

        private void Awake()
        {
            ItemsParentGridLayout = _itemsParent.GetComponent<GridLayoutGroup>();

            if (!ItemsParentGridLayout)
            {
                throw new Exception("Items Parent must have grid layout group component on it");
            }
        }
    }
}