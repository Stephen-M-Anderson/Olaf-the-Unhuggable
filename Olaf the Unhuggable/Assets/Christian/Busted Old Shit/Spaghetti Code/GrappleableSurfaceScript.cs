﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleableSurfaceScript : MonoBehaviour
{

    public GameObject lastNodeObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "LastNodes")
        {
            col.transform.parent = transform;
        }
    }
}
