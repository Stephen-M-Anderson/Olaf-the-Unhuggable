using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    Rigidbody rigid;
    bool facingRight;
    Animator charAnimator;

    private bool grounded = false;
    Collider[] groundCollisions;
    float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float jumpHeight;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        charAnimator = GetComponent<Animator>();
        facingRight = true;
    }

    
    void FixedUpdate()
    {
        if(grounded && Input.GetAxis("Jump") > 0)
        {
            grounded = false;
            charAnimator.SetBool("Grounded", grounded);
            rigid.AddForce(new Vector3(0, jumpHeight, 0));
        }
        
        groundCollisions = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (groundCollisions.Length > 0)
        {
            grounded = true;
        } else
        {
            grounded = false;
        }

        charAnimator.SetBool("Grounded", grounded);

        float move = Input.GetAxis("Horizontal");
        charAnimator.SetFloat("Speed", Mathf.Abs(move));

        rigid.velocity = new Vector3(move * speed, rigid.velocity.y, 0);

        if (move > 0 && !facingRight)
        {
            Flip();
        } else if(move < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.z *= -1;
        transform.localScale = scale;
    }
}
