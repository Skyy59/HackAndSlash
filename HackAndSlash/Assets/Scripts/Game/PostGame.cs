using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PostGame : MonoBehaviour
{
    [SerializeField] private PanelUI scorePanel;

    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI timeText;

    private int _totalKills;
    private float _totalTime;

    public Action OnPostGameEnd;

    private Coroutine _showCR;

    public void CallPostGame(int _kills, float _time)
    {
        _totalKills = _kills;
        _totalTime = _time;
        if (_showCR != null) StopCoroutine(_showCR);
        _showCR = StartCoroutine(ShowPostGame());
    }

    private IEnumerator ShowPostGame()
    {
        scorePanel.EnableCanvas();
        yield return new WaitForSeconds(1f);

        float _counter = 0;

        while (_counter <= 1f)
        {
            killsText.text = Mathf.Lerp(0, _totalKills, _counter).ToString();
            timeText.text = Mathf.Lerp(0, _totalTime, _counter).ToString("00:00");
            _counter += Time.deltaTime;
        }

        killsText.text = _totalKills.ToString();
        timeText.text = _totalTime.ToString();

        yield return new WaitForSeconds(5f);
        OnPostGameEnd?.Invoke();

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if(nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            
        }

        yield return null;
    }
}
