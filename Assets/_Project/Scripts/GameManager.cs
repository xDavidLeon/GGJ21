using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public List<GameObject> flock;
    public int flockSize = 20;
    public GameObject flockUnitPrefab;
    public Vector3 start_pos;

    public GameCanvas gameCanvas;
    public PaperMapEditor paperMapEditor;

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
    }

    protected override void OnSingletonStart()
    {
        base.OnSingletonStart();

        if (gameCanvas == null) gameCanvas = FindObjectOfType<GameCanvas>();
    }

    protected override void OnSingletonDestroy(bool isCurrentInstance)
    {
        base.OnSingletonDestroy(isCurrentInstance);
    }

    public void SpawnFlock(Vector3 position)
    {
        flock = new List<GameObject>();

        for (int i = 0; i < flockSize; i++)
        {
            var pos = position;
            pos.x += UnityEngine.Random.Range(-2.0f, 2.0f);
            pos.z += UnityEngine.Random.Range(-2.0f, 2.0f);
            Vector3 rot = new Vector3(0, UnityEngine.Random.Range(-360.0f, 360.0f), 0);
            flock.Add(Instantiate(flockUnitPrefab, pos, Quaternion.Euler(rot)));
        }
    }

    public void SetFlockWaypoints(List<Vector3> waypoints)
    {
        foreach (var flockUnit in flock)
        {
            flockUnit.GetComponent<NPCCharacter>()?.SetWaypoints(waypoints);
        }
    }

    public void OpenMap()
    {
        if (paperMapEditor == null) return;
        paperMapEditor.OpenMap();
        gameCanvas.uiGameplay.SetActive(false);
        gameCanvas.uiMap.SetActive(true);
    }

    public void CloseMap()
    {
        if (paperMapEditor == null) return;
        paperMapEditor.CloseMap();
        gameCanvas.uiGameplay.SetActive(true);
        gameCanvas.uiMap.SetActive(false);
    }

    public void Win()
    {
    }

    public void Lose()
    {
    }
}