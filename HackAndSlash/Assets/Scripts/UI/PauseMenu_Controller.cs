using UnityEngine;

public class PauseMenu_Controller : MonoBehaviour, IControllableUI
{
    [SerializeField] private PanelUI mainPanel;

    [SerializeField] private PanelUI pausePanel;
    [SerializeField] private PanelUI settingsPanel;
    [SerializeField] private PanelUI upgradesPanel;

    private void OnEnable() 
    {
        Player_Controller.OnRequestPause += OpenPauseMenu;
    }

    private void OnDisable() 
    {
        Player_Controller.OnRequestPause -= OpenPauseMenu;
    }

    public void OpenPauseMenu()
    {
        settingsPanel.DisableCanvas();
        upgradesPanel.DisableCanvas();
        mainPanel.EnableCanvas();
        pausePanel.EnableCanvas();
    }

    public void ClosePauseMenu()
    {
        mainPanel.DisableCanvas();
        settingsPanel.DisableCanvas();
        upgradesPanel.DisableCanvas();
        pausePanel.EnableCanvas();
        Game_Controller.instance.RequestPause();
    }

    public void BackToMenu()
    {
        mainPanel.DisableCanvas();
        settingsPanel.DisableCanvas();
        upgradesPanel.DisableCanvas();
        pausePanel.DisableCanvas();
        Game_Controller.instance.ExitGame();
    }

    public void OnUnPause()
    {
        ClosePauseMenu();
    }
}
