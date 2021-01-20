using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementThrowaway : MonoBehaviour
{
    public float speed = 2f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 moveVector = (transform.forward * y) + (transform.right * x);
        moveVector *= speed * Time.deltaTime;

        transform.localPosition += moveVector;
    }
}
