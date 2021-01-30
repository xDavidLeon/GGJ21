using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;

public class NPCCharacter : MonoBehaviour
{
    private Mover _mover;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
