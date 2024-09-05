using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace HamsterCombat
{
    public class TargetView : MonoBehaviour
    {
        [SerializeField] private Image _target;

        public event Action OnTargetClick;

        private Sequence _scaleSequence;
        private Sequence _rotateSequence;
        
        private void Awake()
        {
            var button = _target.gameObject.GetOrAddComponent<Button>();
            button.onClick.AddListener(() => OnTargetClick?.Invoke());
            button.onClick.AddListener(() =>
            {
                DOTween.Kill(_scaleSequence);
                
                _scaleSequence = DOTween.Sequence(_target.transform).
                    Append(_target.transform.DOScale(_target.transform.localScale * 1.05f, 0.1f)).
                    SetEase(Ease.Flash);
                
                if (_rotateSequence is { active: true }) return;
                
                _rotateSequence = DOTween.Sequence(_target.transform).
                    Append(_target.transform.DORotate(new Vector3(0, 0, 5), 0.3f)).
                    Append(_target.transform.DORotate(new Vector3(0, 0, -5), 0.3f)).
                    Append(_target.transform.DORotate(new Vector3(0, 0, 0), 0.3f)).
                    SetEase(Ease.Flash).OnComplete(()=>
                    {
                        if (_target.transform.localScale != Vector3.one)
                        {
                            _rotateSequence.Restart();
                        }
                    });
            });
        }

        private void Update()
        {
            if (transform.localScale.x > 1f)
            {
                if (transform.localScale.x > 1.5f)
                {
                    transform.localScale = Vector3.one * 1.5f;
                }
                
                transform.localScale -= Vector3.one * 0.1f;

                if (transform.localScale.x < 1f)
                {
                    transform.localScale = Vector3.one;
                }
            }
        }

        public void ChangeSprite(Sprite sprite)
        {
            _target.sprite = sprite;
        }
    }
}