using UnityEngine;
using UnityEngine.SceneManagement;

namespace HamsterCombat
{
    public class Error : MenuWithView<ErrorView>
    {
        public Error(Transform parent, string menuViewResourceName) : base(parent, menuViewResourceName)
        {
            _view.OnConfirm += RestartScene;
        }

        public void ShowError()
        {
            ChangeMenuActive(true);
        }

        private void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}