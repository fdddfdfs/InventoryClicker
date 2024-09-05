using System;
using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class ErrorView : MonoBehaviour
    {
        [SerializeField] private Button _confirmation;

        public event Action OnConfirm;
        
        private void Awake()
        {
            _confirmation.onClick.AddListener(() => OnConfirm?.Invoke());
        }
    }
}