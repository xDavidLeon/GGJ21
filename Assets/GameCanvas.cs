using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCanvas : MonoBehaviour
{
    public GameObject uiMap;
    public GameObject uiGameplay;
    public GameObject uiPlayAgain;
    public GameObject uiWin;
    public GameObject uiLose;
    public TextMeshProUGUI txtCurrentCampNumber;
    public TextMeshProUGUI txtSheepReachedCamp;

    public void CloseMap()
    {
        GameManager.Instance.CloseMap();
    }

    public void Replay()
    {
        SceneManager.LoadScene("Game");
    }
}
