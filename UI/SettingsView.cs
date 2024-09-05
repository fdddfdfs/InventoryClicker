using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HamsterCombat
{
    public class SettingsView : MonoBehaviour
    {
        [SerializeField] private Toggle _fullScreen;
        [SerializeField] private TMP_Dropdown _resolution;
        [SerializeField] private Button _closeMenu;
        [SerializeField] private Button _exit;

        private readonly List<(int x,int y)> _resolutions = new() { (1920, 1080), (960, 540), (480, 270) };
        
        private void Awake()
        {
            _resolution.onValueChanged.AddListener(ChangeResolution);
            ChangeResolution(SettingsStorage.Resolution.Value);

            List<TMP_Dropdown.OptionData> options = new();
            
            for (int i = 0; i < _resolutions.Count; i++)
            {
                options.Add(new TMP_Dropdown.OptionData($"{_resolutions[i].x}x{_resolutions[i].y}"));
            }

            _resolution.options = options;

            _resolution.value = SettingsStorage.Resolution.Value;
            
            _fullScreen.onValueChanged.AddListener(ChangeFullScreen);
            
            ChangeFullScreen(SettingsStorage.IsFullScreen.Value != 0);
            _fullScreen.isOn = SettingsStorage.IsFullScreen.Value != 0;
            
            _closeMenu.onClick.AddListener(()=>gameObject.SetActive(false));
            _exit.onClick.AddListener(Application.Quit);
        }

        private void ChangeFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;

            if (isFullScreen)
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
                SettingsStorage.IsFullScreen.Value = 1;
            }
            else
            {
                Screen.SetResolution(_resolutions[_resolution.value].x, _resolutions[_resolution.value].y, FullScreenMode.Windowed);
                SettingsStorage.IsFullScreen.Value = 0;
            }
        }

        private void ChangeResolution(int newResolution)
        {
            Screen.SetResolution(_resolutions[newResolution].x,
                _resolutions[newResolution].y, 
                SettingsStorage.IsFullScreen.Value != 0 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
            SettingsStorage.Resolution.Value = newResolution;
        }
    }
}