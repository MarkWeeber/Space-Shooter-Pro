using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShooterPro
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        private bool _gameIsOver;
        private bool _gamePaused;

        private void Start()
        {
            _gamePaused = true;
        }

        private void Update()
        {
            if (_gameIsOver)
            {
                if (Input.GetKeyDown(GlobalVariables.RESTART_KEYCODE))
                {
                    _gameIsOver = false;
                    RestartCurrentScene();
                }
            }
            if (_gamePaused)
            {
                if (Input.GetKeyDown(GlobalVariables.JUMP_KEYCODE) || Input.GetKeyDown(GlobalVariables.Q_KEYCODE))
                {
                    TogglePauseTheGame();
                }
            }
            else
            {
                if (Input.GetKeyDown(GlobalVariables.Q_KEYCODE))
                {
                    TogglePauseTheGame();
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

        public void TogglePauseTheGame()
        {
            if (_gamePaused)
            {
                Time.timeScale = 1f;
                UIManager.Instance.HideInfoPanel();
            }
            else
            {
                Time.timeScale = 0f;
                UIManager.Instance.ShowInfoPanel();
            }
            _gamePaused = !_gamePaused;
        }
    }
}