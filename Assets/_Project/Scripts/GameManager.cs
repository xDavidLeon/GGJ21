using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public List<GameObject> flock;
    public int flockSize = 10;
    public GameObject flockUnitPrefab;

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
    }

    protected override void OnSingletonStart()
    {
        base.OnSingletonStart();
    }

    protected override void OnSingletonDestroy(bool isCurrentInstance)
    {
        base.OnSingletonDestroy(isCurrentInstance);
    }

    public void SpawnFlock(Vector3 position)
    {
        for (int i = 0; i < flockSize; i++)
        {
            var pos = position;
            pos += new Vector3(1, 0, 1) * UnityEngine.Random.Range(-2.0f, 2.0f);
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
}