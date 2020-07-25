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

    float dashes = 2f;

    public Transform crosshair;

    private Vector3 aimDir;
    private int dashDir;

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

        aimDir = GetComponent<GrappleScriptEvenNewer>().aimDirection;

        /* JUMPING */
        /* Checking for the jump button was moved from FixedUpdate to Update because for some reason when it was in 
         * fixed update it made every few jumps WAAAAAAY fuckin higher than they were supposed to be. */
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (isGrounded == true)
        {
            dashes = 2f;
        }

        if (Input.GetButtonDown("Fire2") && dashes > 0)
        {

            if (isGrappling)
            {
                //GrappleDash();
            } else
            {
                //Dash();

                //TwoDirectionDash();
                //OtherTwoDirectionDash();
                //AnyDirectionDash();
                //ThreeDirectionDash();
                EightDirectionDash();
            }

            dashes--;

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
        //if (move > 0 && !facingRight && !isGrappling)
        if (move > 0 && !facingRight)
        {
            transform.eulerAngles = new Vector3(0, 90, 0); // Facing Right
            facingRight = !facingRight;
        }
        //else if(move < 0 && facingRight && !isGrapplingController)
        //else if (move < 0 && facingRight && !isGrappling)
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

    void Dash()
    {
        Debug.Log("Oh God Oh Fuck I'm dashing");
        /* Description Start!
         * 
         * This is where the final dash mechanic we go with will be placed.
         */
    }

    void TwoDirectionDash()
    {
        Debug.Log("Oh God Oh Fuck I'm two directional dashing");

        /* Description Start!
         * 
         * Using the facingRight bool we can determine the direction to dash in. Then we give an impulse force in that direction.
         */

        if (facingRight == true)
        {
            myRB.AddForce(new Vector3(300, 0, 0), ForceMode.Impulse);
            //myRB.AddForce(new Vector3(10000, myRB.velocity.y, 0));
        } else
        {
            myRB.AddForce(new Vector3(-300, 0, 0), ForceMode.Impulse);
            //myRB.AddForce(new Vector3(-10000, myRB.velocity.y, 0));
        }
    }

    void OtherTwoDirectionDash()
    {
        Debug.Log("Oh God Oh Fuck I'm two directional dashing using the crosshair to determine direction FUUUUUUUUUUUCk");

        /* Description Start!
        * 
        * Using the crosshair's direction we determine if the dash should be left or right. The circle the crosshair travels around
        * the player will be divided in two 180 degree chunks to determine left or right. 
        */

        switch (dashDir)
        {
            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");
                break;
        }
    }

    void AnyDirectionDash()
    {
        Debug.Log("Oh God Oh Fuck I'm dashing in ANY DIRECTION");

        /* Description Start!
        * 
        * Using the crosshair's direction we determine where the dash should go. This will work exactly like the grapple being 
        * shot out.
        */

        Vector3 dashDir = crosshair.transform.position - transform.position;

        //dashDirection = new Vector3(crosshair.position.x, crosshair.position.y);

        myRB.AddForce(dashDir * 10, ForceMode.Impulse);
       
        //crosshair.position

    }

    void ThreeDirectionDash()
    {
        Debug.Log("Oh God Oh Fuck I'm three directional dashing");

        /* Description Start!
        * 
        * Using the crosshair's direction we determine if the dash should be left, right, or down. The bottom half of circle the 
        * crosshair travels around the player will be divided into three equal parts of 60 degrees.
        */

        switch (dashDir)
        {
            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");
                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");
                break;
        }

    }

    void EightDirectionDash()
    {
        Debug.Log("Oh God Oh Fuck I'm eight directional dashing");

        /* Description Start!
        * 
        * Using the crosshair's direction we determine if the dash should be in any of the 8 cardinal directions, like a compass. The
        * circle that the crosshair travels around the player will be divided into 8 equal parts of 45 degrees each. 
        * 
        * dashDir key:
        * 1 = Up
        * 2 = Up-Right
        * 3 = Right
        * 4 = Down-Right
        * 5 = Down
        * 6 = Down-Left
        * 7 = Left
        * 8 = Up-Left
        * 
        */

        switch (aimDir.x >= -0.38 && aimDir.x <= 0.38 && aimDir.y > 0 ? "UP" :
            aimDir.x >= -0.38 && aimDir.x <= 0.38 && aimDir.y < 0 ? "Down" :

            aimDir.y >= -0.38 && aimDir.y <= 0.38 && aimDir.x < 0 ? "Left" :
            aimDir.y >= -0.38 && aimDir.y <= 0.38 && aimDir.x > 0 ? "Right" :

            aimDir.x >= 0.38 && aimDir.x <= 0.92 && aimDir.y > 0 ? "Up-Right" :
            aimDir.x >= 0.38 && aimDir.x <= 0.92 && aimDir.y < 0 ? "Down-Right" :

            aimDir.x <= -0.38 && aimDir.x >= -0.92 && aimDir.y > 0 ? "Up-Left" :
            aimDir.x <= -0.38 && aimDir.x >= -0.92 && aimDir.y < 0 ? "Down-Left" : "Other")
        {
            case "UP":
                dashDir = 1;
                break;

            case "Up-Right":
                dashDir = 2;
                break;

            case "Right":
                dashDir = 3;
                break;

            case "Down-Right":
                dashDir = 4;
                break;

            case "Down":
                dashDir = 5;
                break;

            case "Down-Left":
                dashDir = 6;
                break;

            case "Left":
                dashDir = 7;
                break;

            case "Up-Left":
                dashDir = 8;
                break;

            case "Other":
                dashDir = 9;
                break;
        }

        switch (dashDir)
        {
            case 1:
                Debug.Log("Oh God Oh Fuck I'm dashing UP!");
                myRB.AddForce(new Vector3(0, 20, 0), ForceMode.Impulse);
                break;

            case 2:
                Debug.Log("Oh God Oh Fuck I'm dashing UP-Right!");
                myRB.AddForce(new Vector3(150, 20, 0), ForceMode.Impulse);
                break;

            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");
                myRB.AddForce(new Vector3(300, 0, 0), ForceMode.Impulse);
                break;

            case 4:
                Debug.Log("Oh God Oh Fuck I'm dashing Down-Right!");
                myRB.AddForce(new Vector3(150, -20, 0), ForceMode.Impulse);
                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");
                myRB.AddForce(new Vector3(0, -20, 0), ForceMode.Impulse);
                break;

            case 6:
                Debug.Log("Oh God Oh Fuck I'm dashing Down-Left!");
                myRB.AddForce(new Vector3(-150, -20, 0), ForceMode.Impulse);
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");
                myRB.AddForce(new Vector3(-300, 0, 0), ForceMode.Impulse);
                break;

            case 8:
                Debug.Log("Oh God Oh Fuck I'm dashing Up-Left!");
                myRB.AddForce(new Vector3(-150, 20, 0), ForceMode.Impulse);
                break;

            case 9:
                Debug.Log("This case should never happen what the fuck?");
                break;
        }

    }

    void GrappleDash()
    {
        Debug.Log("Oh God Oh Fuck I'm dashing while also fucking grappling hooooooly fuck");

        if (facingRight == true)
        {
            myRB.AddForce(new Vector3(100, myRB.velocity.y, 0), ForceMode.Impulse);
            //myRB.AddForce(new Vector3(10000, myRB.velocity.y, 0));
        }
        else
        {
            myRB.AddForce(new Vector3(-100, myRB.velocity.y, 0), ForceMode.Impulse);
            //myRB.AddForce(new Vector3(-10000, myRB.velocity.y, 0));
        }

        /* Description Start!
         * 
         * Using the facingRight bool we can determine the direction to dash in. Then we give an impulse force in that direction. Make
         * sure this only works while grappling. 
         */
    }

    /*void Flip()
    {
        facingRight = !facingRight;
        Vector3 flipVector = transform.localScale;
        flipVector.z *= -1;
        transform.localScale = flipVector;
    }*/



}
