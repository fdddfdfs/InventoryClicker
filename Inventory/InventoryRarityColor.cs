using System;
using UnityEngine;

namespace HamsterCombat
{
    [Serializable]
    public class InventoryRarityColor
    {
        [SerializeField] private InventoryItemRarityType _rarity;
        [SerializeField] private Color _color;

        public InventoryItemRarityType Rarity => _rarity;

        public Color Color => _color;
    }
}