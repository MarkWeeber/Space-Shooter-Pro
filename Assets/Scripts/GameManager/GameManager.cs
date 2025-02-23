using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShooterPro
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        private bool _gameIsOver;
        protected override void Awake()
        {
            dontDestroyOnload = true;
            base.Awake();
        }

        private void Update()
        {
            if (_gameIsOver)
            {
                if (Input.GetKeyDown(GlobalVariables.RESTART_KEYCODE))
                {
                    RestartCurrentScene();
                }
            }
            if (Input.GetKeyDown(GlobalVariables.ESCAPE_KEYCODE))
            {
                Application.Quit();
            }
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene(GlobalVariables.MAIN_MENU_SCENE);
        }

        public void OpenGameScene()
        {
            SceneManager.LoadScene(GlobalVariables.GAME_SCENE);
        }

        public void RestartCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void SetGameOver()
        {
            _gameIsOver = true;
        }
    }
}