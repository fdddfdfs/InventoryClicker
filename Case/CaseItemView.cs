using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class CaseItemView : MonoBehaviour
    {
        [SerializeField] private Image _item;
        [SerializeField] private Image _rarity;

        public void ChangeItem(Sprite item, Color itemRarityColor)
        {
            _item.sprite = item;
            _rarity.color = itemRarityColor;
        }
    }
}