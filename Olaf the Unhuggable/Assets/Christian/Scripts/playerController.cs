using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    //movement variables
    public float runSpeed;
    public float swingSpeed;
    private Rigidbody myRB;
    private Animator myAnimator;

    public bool facingRight; //Used for changing the direction the character is facing
    
    //jumping variables
    bool isGrounded = false;
    Collider[] groundCollisions;
    float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float jumpHeight;

    public bool isGrapplingController = false;

    private bool isGrappling;
    public Vector3 ropeHook;
    public float swingForce = 20f;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        facingRight = true;
        
    }

    // Update is called once per frame
    void Update()
    {

        isGrappling = GetComponent<GrappleScriptEvenNewer>().isGrappling;

        /* JUMPING */
        /* Checking for the jump button was moved from FixedUpdate to Update because for some reason when it was in 
         * fixed update it made every few jumps WAAAAAAY fuckin higher than they were supposed to be. */
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        DebugLogs();
    }

    void FixedUpdate()
    {

        /* MOVEMENT */
        float move = Input.GetAxis("Horizontal");
        myAnimator.SetFloat("speed", Mathf.Abs(move));


        if (isGrappling == true)
        {

            myRB.velocity = new Vector3(move * swingSpeed, myRB.velocity.y, 0);



        } else
        {
            myRB.velocity = new Vector3(move * runSpeed, myRB.velocity.y, 0);
        }

        /* This was originally the previously a check for the commented out "flip()" function but I changed it
         * because the flip function stupidly tried to flip the scaling into the negative but that IMMEDIATELY
         * fucked with all collision and facing left suddenly also meant sinking under the floor.*/
        //if (move > 0 && !facingRight && !isGrapplingController)
        if (move > 0 && !facingRight)
        {
            transform.eulerAngles = new Vector3(0, 90, 0); // Facing Right
            facingRight = !facingRight;
        }
        //else if(move < 0 && facingRight && !isGrapplingController)
        else if (move < 0 && facingRight)
        {
            transform.eulerAngles = new Vector3(0, 270, 0); // Facing Left
            facingRight = !facingRight;
        }

        /* CHECKING IF THE PLAYER IS GROUNDED FOR LANDING */

        groundCollisions = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if(groundCollisions.Length > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        myAnimator.SetBool("grounded", isGrounded);


    }

    void Jump()
    {
        myAnimator.SetBool("grounded", isGrounded);
        myRB.AddForce(new Vector3(0, jumpHeight, 0));
        isGrounded = false;
    }

    void DebugLogs()
    {
        Debug.Log("isGrounded = " + isGrounded);
    }

    /*void Flip()
    {
        facingRight = !facingRight;
        Vector3 flipVector = transform.localScale;
        flipVector.z *= -1;
        transform.localScale = flipVector;
    }*/

    
}
