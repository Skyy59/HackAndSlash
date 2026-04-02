using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_Controller : MonoBehaviour
{
    [SerializeField] private string gameScene = "Game";
    [SerializeField] private Button continueButton;
    [SerializeField] private UI_Section mainSection;

    private void Start() 
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;   
        // if (DataManager.instance) continueButton.gameObject.SetActive(DataManager.instance.FileExists());    
        mainSection.AdjustNavigation();
    }

    public void LoadGame()
    {
        // if (DataManager.instance) SceneManager.LoadScene(DataManager.instance.FileExists() ? DataManager.instance.GetSavedScene() : gameScene);
        SceneManager.LoadScene(gameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
