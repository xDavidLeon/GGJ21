using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public GameObject uiMap;
    public GameObject uiGameplay;
    
    public void CloseMap()
    {
        GameManager.Instance.CloseMap();
    }
}
