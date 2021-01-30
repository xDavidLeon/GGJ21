using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;

public class NPCCharacter : MonoBehaviour
{
    public float speed = 1.0f;

    public List<Vector3> waypoints;
    public List<int> visited;

    public int currentWaypoint = -1;

    private Mover _mover;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        waypoints = new List<Vector3>();
        visited = new List<int>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaypoint == -1) currentWaypoint = GetClosestWaypoint();

        if (currentWaypoint != -1)
        {
            var waypoint = waypoints[currentWaypoint];
            var direction = (waypoint - transform.position).normalized;
            _mover.SetVelocity(direction * speed);

            if (Vector3.Distance(transform.position, waypoints[currentWaypoint]) < 0.25f)
            {
                ReachedWaypoint(currentWaypoint);
            }
        }
    }

    public void SetWaypoints(List<Vector3> w)
    {
        waypoints.Clear();
        visited.Clear();
        waypoints = w;
        currentWaypoint = GetClosestWaypoint();
    }

    public int GetClosestWaypoint(float maxDistance = 5.0f, float minDistance = 0.25f)
    {
        if (waypoints == null || waypoints.Count == 0) return -1;

        int closest = -1;
        float closestDistance = float.MaxValue;
        foreach (var waypoint in waypoints)
        {
            var id = waypoints.IndexOf(waypoint);
            if (visited.Contains(id)) return -1;
            var d = Vector3.Distance(waypoint, transform.position);
            if (d > maxDistance) continue;
            if (d > closestDistance) continue;
            if (d < minDistance) continue;
            closestDistance = d;
            closest = waypoints.IndexOf(waypoint);
        }

        return closest;
    }

    public int GetPreviousWaypoint(float maxDistance = 5.0f, float minDistance = 0.25f)
    {
        if (waypoints == null || waypoints.Count == 0) return -1;
        if (currentWaypoint == -1) return GetClosestWaypoint(maxDistance);
        if (currentWaypoint - 1 < 0) return -1;
        if (visited.Contains(currentWaypoint - 1)) return -1;

        var d = Vector3.Distance(waypoints[currentWaypoint - 1], transform.position);
        if (d > maxDistance) return -1;
        if (d < minDistance) return -1;
        return currentWaypoint + 1;
    }

    public int GetNextWaypoint(float maxDistance = 5.0f, float minDistance = 0.25f)
    {
        if (waypoints == null || waypoints.Count == 0) return -1;
        if (currentWaypoint == -1) return GetClosestWaypoint(maxDistance);
        if (currentWaypoint + 1 >= waypoints.Count) return -1;
        if (visited.Contains(currentWaypoint + 1)) return -1;
        var d = Vector3.Distance(waypoints[currentWaypoint + 1], transform.position);
        if (d > maxDistance) return -1;
        if (d < minDistance) return -1;

        return currentWaypoint + 1;
    }

    public void ReachedWaypoint(int id)
    {
        visited.Add(id);
        var next = GetNextWaypoint();
        var previous = GetPreviousWaypoint();
        if (next >= 0) currentWaypoint = next;
        else if (previous >= 0) currentWaypoint = previous;
        else currentWaypoint = -1;
    }
}