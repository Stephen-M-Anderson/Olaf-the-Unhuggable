using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;
    public List<Transform> range;

    Animator eAnimator;

    bool detected = false;

    public Transform point1;
    public Transform point2;
    public int currentPoint = 0;

    public GameObject player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target.position;
        player = GameObject.FindGameObjectWithTag("Player");
        eAnimator = GetComponentInParent<Animator>();
        eAnimator.SetBool("Start", true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!detected)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                nextPoint();
            }
        }
        if (detected)
        {
            Chase();
        }
    }

    public void nextPoint()
    {
        //first you assign the range, +- 5.x from the enemy position
        point1.transform.position = new Vector3(agent.transform.position.x + 5, 0, 0);
        point2.transform.position = new Vector3(agent.transform.position.x - 5, 0, 0);
        //then put it in the list, overwrites anything already in there
        range.Insert(0, point1);
        range.Insert(1, point2);
        //go to the current point on that range and increment
        agent.destination = range[currentPoint].position;
        currentPoint += 1;
        //if you reach the end start over
        if (currentPoint > 1)
        {
            currentPoint = 0;
        }
    }

    public void Chase()
    {
        agent.destination = player.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !detected)
        {
            detected = true;
            eAnimator.SetBool("Detected", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detected = false;
            eAnimator.SetBool("Detected", false);
        }
    }
}
