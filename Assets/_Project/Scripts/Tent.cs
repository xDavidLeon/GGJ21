using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class Tent : MonoBehaviour
{
    public bool isPlayerInside = false;
    public List<GameObject> flockUnits;

    public int SheepReachedTent
    {
        get
        {
            return flockUnits.Count;
        }
    }
    
    private void Start()
    {
        flockUnits = new List<GameObject>();
    }

    void Update()
    {
        if (isPlayerInside)
        {
            // if (ReInput.players.GetPlayer(0).GetButtonDown("Map"))
            // {
            //     if (GameManager.Instance.paperMapEditor.watching_map) GameManager.Instance.CloseMap();
            //     else
            //         GameManager.Instance.OpenMap();
            // }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerInside) return;

        isPlayerInside = true;

        if (other.CompareTag("Player"))
        {
            GameManager.Instance.OpenMap();
        }
        else if (other.CompareTag("Sheep"))
        {
            if (flockUnits.Contains(other.gameObject) == false) flockUnits.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlayerInside) return;
        isPlayerInside = false;

        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CloseMap();
        }
    }
}