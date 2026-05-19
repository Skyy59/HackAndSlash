using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_Controller : MonoBehaviour
{
    [SerializeField] private string gameScene = "District1";
    [SerializeField] private Button continueButton;
    [SerializeField] private UI_Section mainSection;

    private void Start() 
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;

        

        bool hasSaveData = PlayerPrefs.HasKey("ReachedLevelIndex");
        if(continueButton != null)
        {
            continueButton.gameObject.SetActive(hasSaveData);
        }   
        // if (DataManager.instance) continueButton.gameObject.SetActive(DataManager.instance.FileExists());    
        mainSection.AdjustNavigation();
    }

    public void NewGame()
    {
        Save_Manager.DeleteAllSaveData();

        SceneManager.LoadScene(gameScene);
    }

    public void ContinueGame()
    {
        int savedSceneIndex = Save_Manager.GetSavedLevel();

        SceneManager.LoadScene(savedSceneIndex);
    }

    public void LoadGame()
    {
        ContinueGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
