/* Description: The player controller controls the player (holy fuck no way!). This means that this script handles
 * movement, jumping, and whatever other shit should be included in a player controller.
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [Header("Movement Variables")]
    public bool facingRight = true; //Used for changing the direction the character is facing
    private bool movementBool = true; //If this bool is true then the regular movement function is called
    public float runSpeed; //This variable correlates to the movement speed of the player
    public float swingSpeed; //This variable correlates to the movement speed of the player while swinging on a rope
    public Vector3 movementForce; //Speed of movement plus a direction given by user input (dpad)
    private Vector3 swingingForce; //Speed of movement plus a direction given by user input (dpad) while swinging

    [Header("Outside Components")]
    private Rigidbody myRB; //A reference to the player's rigidbody component
    private Animator myAnimator; //A reference to the player's animator component

    [Header("Jumping Variables")]
    bool jumpBool = false; //Determines whether or not the player is jumping
    public bool isGrounded = false; //The bool used for ground detection. If this is true then the character is on solid ground.
    Collider[] groundCollisions; //The collider that determines whether or not the player is in contact with the ground.
    float groundCheckRadius = 0.2f; //The radius of the collider that determines if the player is in contact with ground.
    public LayerMask groundLayer; //The Layer that any object we call "ground" is set to
    public Transform groundCheck; //The transform component of the invisible object used for ground checking
    public float jumpHeight; //This number correlates to how high the player can jump

    [Header("Grappling Variables")]
    private bool isGrappling = false; //If this bool is true then the player is currently grappling
    private bool grappleMove = false; //This bool determines whether the grapple movement function is being used instead 
                                      //of the regular movement function.

    [Header("Speedometer")]
    public Text speedText; //This is where the text of your current SPEEEEED is stored
    float speed; //This is the value of your current SPEED
    private bool checkSpeed = true; //If we checked for speed every frame the number would be fucking changing 60 times a
                                    //goddamn second. Because of this we flip this bool at a regular interval to tell
                                    //the program to update our speed value.

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>(); //setting myRB to reference the rigidbody component of our player
        myAnimator = GetComponent<Animator>(); //setting myAnimator to reference the animator component of our player
    }

    // Update is called once per frame
    void Update()
    {
                                                /* Collecting Inputs */

        //Collecting directional inputs:
        float moveX = Input.GetAxis("Horizontal"); 
        float moveY = Input.GetAxis("Vertical");

        myAnimator.SetFloat("speed", Mathf.Abs(moveX)); //The vertical input we collected determines the speed of the 
                                                        //walking/running animation
        movementForce = new Vector3(moveX * runSpeed, myRB.velocity.y, 0); //Creating a force (spd + dir) for movement
        swingingForce = new Vector3(moveX * swingSpeed, myRB.velocity.y, 0); //Creating a force (spd + dir) for swinging

                                                /* Referencing Other Scripts */

        isGrappling = GetComponent<GrappleScriptEvenNewer>().isGrappling; //Reference the grappling script to see if we're
                                                                          //grapplin'

                                                /* SPEEDO CHEKKU */

        if (checkSpeed)
        {
            SpeedCalc(); //This function calculates the player's current speed and runs when the checkSpeed bool flips
        }

                                                /* Determining What Movement Type to Use */

        //If we're grappling we use grapple movement, if not then we use regular movement. That's it dawg
        if (isGrappling == true)
        {
            grappleMove = true;
            movementBool = false;
        } else
        {
            movementBool = true;
        }
                                                /* JUMPING */
        /* Checking for the jump button was moved from FixedUpdate to Update because for some reason when it was in 
         * fixed update it made every few jumps WAAAAAAY fuckin higher than they were supposed to be. */

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            jumpBool = true;
        }

        //Debug.Log("isGrounded = " + isGrounded);

    }

    void FixedUpdate()
    {
        /* Okay so if this is confusing let me explain why we're using FixedUpdate here. Essentially for physics based
         * movement type games (oh shit like this one wow!) it's ideal to grab inputs in Update but do anything having
         * to do with physics in FixedUpdate. Because of this the player controller should (in terms of physics shit)
         * flip a bool on Update and have that bool determine if a function is called on FixedUpdate.*/

                                                /* Jumping */
        if (jumpBool == true)
        {
            Jump(); 
        }

                                                /* Movement */

        Movement();

        groundedCheck();
                                                
    }

    void Movement()
    {
        if (isGrappling == true && grappleMove == true)
        {
            //If we wanted grapple movement to work differently from regular movement this is where we'd put that code.

            myRB.velocity = swingingForce; //Apply velocity to our rigidbody to move it
        }
        else if (movementBool == true)
        {
            myRB.velocity = movementForce; //Apply velocity to our rigidbody to move it
        }

        /* This was originally a check for a function that would flip the character's 3D model depending on facing
        * direction but we got rid of that because that function function stupidly tried to flip the scaling into the 
        * negative but that IMMEDIATELY fucked with all collision and facing left suddenly also meant sinking under 
        * the floor.*/

                                            /* Direction Check */
                /* This function makes the player's character model face the correct direction */

        if (movementForce.x > 0 && !facingRight)
        {
            transform.eulerAngles = new Vector3(0, 90, 0); // Facing Right
            facingRight = !facingRight;
        }
        else if (movementForce.x < 0 && facingRight)
        {
            transform.eulerAngles = new Vector3(0, 270, 0); // Facing Left
            facingRight = !facingRight;
        }
    }

    void groundedCheck()
    {
        /* This function creates a physics overlap sphere on the player and if that sphere collides with anything on the
         * "ground" layer then the player is grounded.*/

        groundCollisions = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (groundCollisions.Length > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        myAnimator.SetBool("grounded", isGrounded); //The animator determines whether or not the jumping animation plays
                                                    //based on if the character is grounded.
    }

    void Jump()
    {
        myAnimator.SetBool("grounded", isGrounded); //The animator determines whether or not the jumping animation plays
                                                    //based on if the character is grounded.
        myRB.AddForce(new Vector3(0, jumpHeight, 0)); //Add some fucking force to make the character jump
        isGrounded = false; //If we're in the air we ain't grounded
        jumpBool = false; //gotta flip that bool so that it doesn't call this function on the next FixedUpdate
    }

    

    void SpeedCalc()
    {
        checkSpeed = false;
        /* This one was how I calculated speed with arbitrary numbers: */
        //var vel = myRB.velocity;
        //speed = myRB.velocity.magnitude;
        //speed = (transform.position - lastPosition).magnitude * 100;
        //lastPosition = transform.position;

        /* This one was how I calculated speed in mph: */
        speed = myRB.velocity.magnitude * 2.237f;
        speedText.text = speed.ToString();
        StartCoroutine(SpeedCalcWait());
    }

    IEnumerator SpeedCalcWait() //This function determines how long between speed calculations
    {
        yield return new WaitForSeconds(0.05f);
        checkSpeed = true;
    }


}
