using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void OnGameOver()
    {
        _gameIsOver = true;
    }
}
