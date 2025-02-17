using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
    protected override void Awake()
    {
        dontDestroyOnload = true;
        base.Awake();
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
}
