using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject flipEnemy;
    public float detectionTime;

    float startRun;
    bool firstDetection;

    public float runSpeed;
    public float walkSpeed;
    public bool facingRight = true;

    float moveSpeed;
    bool running;

    Rigidbody rigid;
    Animator eAnimator;
    Transform detectedPlayer;

    bool Detected;
    void Start()
    {
        rigid = GetComponentInParent<Rigidbody>();
        eAnimator = GetComponentInParent<Animator>();
        running = false;
        Detected = false;
        firstDetection = false;
        moveSpeed = walkSpeed;

        if(Random.Range(0, 10) > 5)
        {
            Flip();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Detected)
        {
            if(detectedPlayer.position.x < transform.position.x && facingRight)
            {
                Flip();
            } else if(detectedPlayer.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
            if (!firstDetection)
            {
                startRun = Time.time + detectionTime;
                firstDetection = true;
            }
        }
        if (Detected && !facingRight)
        {
            rigid.velocity = new Vector3((moveSpeed * -1), rigid.velocity.y, 0);
        } else if (Detected && !facingRight)
        {
            rigid.velocity = new Vector3(moveSpeed, rigid.velocity.y, 0);
        }
        if(!running && Detected)
        {
            if(startRun < Time.time)
            {
                moveSpeed = runSpeed;
                eAnimator.SetTrigger("Run");
                running = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !Detected)
        {
            Detected = true;
            detectedPlayer = other.transform;
            eAnimator.SetBool("Detected", Detected);
            
            if (detectedPlayer.position.x < transform.position.x && facingRight)
            {
                Flip();
            } else if(detectedPlayer.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            firstDetection = false;
            if (running)
            {
                eAnimator.SetTrigger("Run");
                moveSpeed = walkSpeed;
                running = false;
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = flipEnemy.transform.localScale;
        theScale.z *= -1;
        flipEnemy.transform.localScale = theScale;
    }
}
