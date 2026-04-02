using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    public static Game_Controller instance;
    [Header("Game Controller")]
    [SerializeField] private string menuScene = "Main Menu";
    [SerializeField] private bool gameIsPaused = false;

    private void Awake() 
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }    
    }

    public void RequestPause()
    {
        gameIsPaused = !gameIsPaused;
        Time.timeScale = gameIsPaused ? 0f : 1f;
        Input_Manager.instance.SetPlayerInputs(!gameIsPaused);
        Input_Manager.instance.SetUIInputs(gameIsPaused);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Input_Manager.instance.RemoveAllInputs();
        SceneManager.LoadScene(menuScene);
    }
}
