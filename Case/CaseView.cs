using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class CaseView : MonoBehaviour
    {
        private const float RollTime = 0.1f;
        private const int RollsToStop = 20;
        
        [SerializeField] private List<CaseItemView> _items;
        [SerializeField] private AnimationCurve _stopCurve;

        private WeightRandom _itemRandom;
        private InventoryChestItemData _currentChest;
        private int _currentFirst = 0;
        
        private Dictionary<InventoryChestItemData, WeightRandom> _chestRandoms;
        private Dictionary<InventoryItemRarityType, Color> _rarityColors;
        
        private int _remainingRolls;
        private InventoryItemData _resultRoll;
        

        public event Action OnStopRolling;

        public void Init(Dictionary<InventoryItemRarityType, Color> rarityColors)
        {
            _items.Sort(
                ((image, image1) => Math.Sign(image.transform.position.x - image1.transform.position.x)));

            _chestRandoms = new Dictionary<InventoryChestItemData, WeightRandom>();
            _rarityColors = rarityColors;
        }

        public void StartAnimation(InventoryChestItemData openedCase)
        {
            _currentChest = openedCase;
            
            if (!_chestRandoms.ContainsKey(openedCase))
            {
                _chestRandoms[openedCase] = new WeightRandom(openedCase.PossibleOutcomes, true);
            }

            _itemRandom = _chestRandoms[openedCase];
            
            for (int i = 0; i < _items.Count; i++)
            {
                InventoryItemData item = _currentChest.PossibleOutcomes[_itemRandom.GetRandom()];
                _items[i].ChangeItem(item.Icon, _rarityColors[item.InventoryItemRarityType]);
            }

            _remainingRolls = -1;
            
            MoveCells();
        }

        public void StopAnimation(InventoryItemData result)
        {
            _remainingRolls = RollsToStop;
            _resultRoll = result;
        }

        public void StartAnimationWithPredefinedItems(List<InventoryItemData> items)
        {
            if (items.Count < _items.Count) return;
            
            for (int i = 1; i < _items.Count; i++)
            {
                _items[i].ChangeItem(
                    items[0].Icon,
                    _rarityColors[items[0].InventoryItemRarityType]);
                items.RemoveAt(0);
                Debug.Log(items.Count);
            }
            
            MoveCells(items);
        }

        private void MoveCells(List<InventoryItemData> remainingItems)
        {
            int lastIndex = Mod(_currentFirst - 1, _items.Count);
            Vector3 lastPosition = _items[lastIndex].transform.position;

            Sequence moving = DOTween.Sequence();

            float duration = RollTime * 2;
            
            for (int i = 0; i < _items.Count; i++)
            {
                if(i == _currentFirst) continue;
                
                moving.Join(_items[i].transform.DOMoveX(
                    _items[Mod(i - 1,_items.Count)].transform.position.x,
                    duration).SetEase(Ease.Flash));
            }
            
            _items[_currentFirst].transform.position = lastPosition;
            
            _items[_currentFirst].ChangeItem(
                remainingItems[0].Icon,
                _rarityColors[remainingItems[0].InventoryItemRarityType]);
            
            remainingItems.RemoveAt(0);
            
            _currentFirst = (_currentFirst + 1) % _items.Count;

            if (remainingItems.Count != 0)
            {
                moving.OnComplete(() => MoveCells(remainingItems));
            }
        }

        private void MoveCells()
        {
            if (_remainingRolls > 0) _remainingRolls--;
            
            int lastIndex = Mod(_currentFirst - 1, _items.Count);
            Vector3 lastPosition = _items[lastIndex].transform.position;

            Sequence moving = DOTween.Sequence();

            float duration = _remainingRolls == -1 ?
                RollTime : 
                RollTime * _stopCurve.Evaluate(1 - (float)_remainingRolls/RollsToStop);
            
            for (int i = 0; i < _items.Count; i++)
            {
                if(i == _currentFirst) continue;
                
                moving.Join(_items[i].transform.DOMoveX(
                    _items[Mod(i - 1,_items.Count)].transform.position.x,
                    duration).SetEase(Ease.Flash));
            }
            
            _items[_currentFirst].transform.position = lastPosition;

            if (_remainingRolls == Mathf.FloorToInt(_items.Count / 2f))
            {
                _items[_currentFirst].ChangeItem(_resultRoll.Icon, _rarityColors[_resultRoll.InventoryItemRarityType]);
            }
            else
            {
                InventoryItemData item = _currentChest.PossibleOutcomes[_itemRandom.GetRandom()];
                _items[_currentFirst].ChangeItem(item.Icon, _rarityColors[item.InventoryItemRarityType]);
            }
            
            _currentFirst = (_currentFirst + 1) % _items.Count;

            if (_remainingRolls != 0)
            {
                moving.OnComplete(MoveCells);
            }
            else
            {
                OnStopRolling?.Invoke();
            }
        }
        
        private static int Mod(int x, int m) {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}