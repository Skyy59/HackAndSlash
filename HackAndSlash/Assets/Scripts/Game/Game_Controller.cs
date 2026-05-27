using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    public static Game_Controller instance;
    [Header("Game Controller")]
    [SerializeField] private PostGame postGame;
    [SerializeField] private string menuScene = "Main Menu";
    [SerializeField] private bool gameIsPaused = false;

    private int _kills;
    private float _time;
    private bool _levelEnded = false;

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

    private void Start() 
    {
        SetCursorState(false);
    }

    private void Update() 
    {
        if (!_levelEnded && !gameIsPaused)
        {
            _time += Time.deltaTime;
        }
          
    }

    public void AddKill() { _kills++; }

    public void EndReached()
    {
        if (_levelEnded) return;
        _levelEnded = true;
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        Save_Manager.CheckAndSaveRecord(currentSceneIndex, _time);

        Save_Manager.SaveProgress();

        if(postGame != null)
        {
            postGame.CallPostGame(_kills, _time);
        }
        
        
    }

    public void SetCursorState(bool _state)
    {
        Cursor.visible = _state;
        Cursor.lockState = _state ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void RequestPause()
    {
        gameIsPaused = !gameIsPaused;
        SetCursorState(gameIsPaused);
        Time.timeScale = gameIsPaused ? 0f : 1f;
        Input_Manager.instance.SetPlayerInputs(!gameIsPaused);
        Input_Manager.instance.SetUIInputs(gameIsPaused);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SetCursorState(true);
        Input_Manager.instance.RemoveAllInputs();
        SceneManager.LoadScene(menuScene);
    }
}
