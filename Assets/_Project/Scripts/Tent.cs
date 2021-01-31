using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class Tent : MonoBehaviour
{
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            if (flockUnits.Contains(other.gameObject) == false) flockUnits.Add(other.gameObject);
        }
    }
}