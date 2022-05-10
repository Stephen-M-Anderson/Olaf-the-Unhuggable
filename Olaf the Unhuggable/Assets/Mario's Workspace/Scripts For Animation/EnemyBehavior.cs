using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    NavMeshAgent agent;
    public List<Transform> range;
    //Rigidbody rigid;

    //Animator eAnimator;

    bool detected = false;
    //bool grounded;

    public Transform point1;
    public Transform point2;
    public Transform target;

    public int currentPoint = 0;
    public float jumpHeight;
    public float minJumpTime = 80;
    public float maxJumpTime = 100;
    public int XRight;
    public int XLeft;

    public GameObject player;

    public LayerMask groundLayer;
    public Transform groundCheck;
    //Collider[] groundCollisions;
    //float groundCheckRadius = 0.2f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target.position;
        player = GameObject.FindGameObjectWithTag("Player");
        //eAnimator = GetComponentInParent<Animator>();
        //eAnimator.SetBool("Start", true);
        //rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!detected)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                NextPoint();
            }
        }
        if (detected)
        {
            Chase();
        }

        /*if (grounded)
        {
            StartCoroutine(JumpLogic());
            Debug.Log("StartCorutine");
            Jump();
        }

        groundCollisions = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (groundCollisions.Length > 0)
        {
            grounded = true;
            Debug.Log("Grounded");
        }
        else
        {
            grounded = false;
            Debug.Log("Not Grounded");
        }
        grounded = true;
        eAnimator.SetBool("Grounded", grounded);*/
    }

    public void NextPoint()
    {
        //first you assign the range, +- 5.x from the enemy position
        point1.transform.position = new Vector3(agent.transform.position.x + XRight, 0, 0);
        point2.transform.position = new Vector3(agent.transform.position.x - XLeft, 0, 0);
        //then put it in the list, overwrites anything already in there, probably
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
            //eAnimator.SetBool("Detected", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detected = false;
            //eAnimator.SetBool("Detected", false);
        }
    }

    /*IEnumerator JumpLogic()
    {
        yield return new WaitForSeconds(Random.Range(minJumpTime, maxJumpTime));
        Debug.Log("In Coroutine");
        Jump();
    }

    public void Jump()
    {
        grounded = false;
        eAnimator.SetBool("Grounded", grounded);
        rigid.AddForce(new Vector3(0, jumpHeight, 0));
    }*/
}
