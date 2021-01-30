using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    public GameObject target;

    private void Awake()
    {
        Instantiate(target, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}