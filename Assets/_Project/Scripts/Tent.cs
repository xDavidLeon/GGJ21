using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tent : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCharacter>().paperMapEditor.OpenMap();
        }
    }
}
