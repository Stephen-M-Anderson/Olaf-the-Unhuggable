using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    float xMovement;
    NavMeshAgent agent;

    public float speed;
    public float verticalSpeed;

    public Vector3 currentVelocity; //delete later

    Animator charAnimator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        charAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        xMovement = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(xMovement, 0, 0);
        Vector3 direction = transform.position + movement;
        agent.destination = direction;

        //speed = agent.speed;
        //currentVelocity = agent.velocity;
        speed = agent.velocity.x;
        Debug.Log(currentVelocity);
        charAnimator.SetFloat("Speed", Mathf.Abs(speed));

        
    }
}
