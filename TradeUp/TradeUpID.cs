using System;
using UnityEngine;

namespace HamsterCombat
{
    [Serializable]
    public class TradeUpID
    {
        [SerializeField] private InventoryItemRarityType _rarity;
        [SerializeField] private int _id;

        public InventoryItemRarityType Rarity => _rarity;

        public int ID => _id;
    }
}