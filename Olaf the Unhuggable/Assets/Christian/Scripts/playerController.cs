/* Description:  It controls the player. Done...
 * ...
 * ...
 * ...
 * ...
 * Okay fuck, uh this handles movement, jumping, dashing, any other basic shit the player needs to do. Ye.
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{

    [Header("Movement Variables")]
    public bool facingRight = true; //Used for changing the direction the character is facing
    public bool swingingRight = true;
    private bool movementBool = true; //If this bool is true then the regular movement function is called
    public bool inputDisabled = false; //If this bool is true then all player input has been disabled
    public float runSpeed; //This variable correlates to the movement speed of the player
    public float swingSpeed; //This variable correlates to the movement speed of the player while swinging on a rope
    private float swingMagnitude;
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
    public GrappleScriptEvenNewer grappleScript;
    private bool isGrappling = false; //If this bool is true then the player is currently grappling
    private bool grappleMove = false; //This bool determines whether the grapple movement function is being used instead 
                                      //of the regular movement function.

    [Header("Speedometer")]
    public Text speedText; //This is where the text of your current SPEEEEED is stored
    float speed; //This is the value of your current SPEED
    private bool checkSpeed = true; //If we checked for speed every frame the number would be fucking changing 60 times a
                                    //goddamn second. Because of this we flip this bool at a regular interval to tell
                                    //the program to update our speed value.

    [Header("Ball Man Variables")]
    public GameObject olafDummy; //The mesh for Olaf's regular body
    public GameObject ballDummy; //The mesh for Olaf's ball form
    CapsuleCollider olafBodyCollider; //The collider component used in Olaf's regular form
    SphereCollider olafBallCollider; //The collider component used in Olaf's ball mode
    public bool ballManBool = false;

    [Header("Dash Variables")]
    public float dashes = 2f; //How many dashes are you allowed to use in a row? This replenishes after touching ground
    private bool canDash = true; //If true, the player can dash
    private bool dashBool = false; //If true, the dash function is called
    public bool isDashing = false; //If this bool is true then the player is currently dashing
    public Transform crosshair; //The location of the crosshair (used in some dashes)
    private Vector3 aimDir; //A vector representing the direction the player is currently aiming their mouse
    private int dashDir; //This integer is used in the switch cases for dashing. Each number correlates to a direction.
    public float dashSpeed; //How fast you can dash
    private Vector3 dashPoint; //This variable holds the direction we want the player to dash to
    public float dashCooldown; //Amount of time in seconds between dashes
    public float verticalEquilizer; //This one is a bit complicated. For some weird reason the way we're implementing
                                    //movement, vertical and horizontal forces are not created equal. It takes WAY more
                                    //force to move something vertically than horizontally. I used this value as an offset
                                    //to try and balance those out a little better.
    private bool GrappleDashBool = false; //This is meant to be a special dash you can do while grappling... shits busted



    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>(); //setting myRB to reference the rigidbody component of our player
        myAnimator = GetComponent<Animator>(); //setting myAnimator to reference the animator component of our player
        olafBallCollider = GetComponent<SphereCollider>();
        olafBodyCollider = GetComponent<CapsuleCollider>();
        grappleScript = GetComponent<GrappleScriptEvenNewer>();
    }

    // Update is called once per frame
    void Update()
    {
        /* Collecting Inputs */
        //Collecting directional inputs:
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        if (inputDisabled == false)
        {
            myAnimator.SetFloat("speed", Mathf.Abs(moveX)); //The vertical input we collected determines the speed of the 
                                                            //walking/running animation
        }
        movementForce = new Vector3(moveX * runSpeed, myRB.velocity.y, 0); //Creating a force (spd + dir) for movement
        swingingForce = new Vector3(myRB.velocity.x + (moveX * swingSpeed), myRB.velocity.y, 0); //Creating a force (spd + dir) for swinging
        swingMagnitude = swingSpeed * moveX;
        if (grappleScript.originallySwingingRight == false)
        {
            swingMagnitude *= -1;
        }
        if (grappleScript.swapGrappleDirection)
        {
            swingMagnitude *= -1;
        }

        /* SPEEDO CHEKKU */
        if (checkSpeed)
        {
            SpeedCalc(); //This function calculates the player's current speed and runs when the checkSpeed bool flips
        }

        /* Referencing Other Scripts */

        isGrappling = grappleScript.isGrappling; //Reference the grappling script to see if we're
                                                                          //grapplin'
        aimDir = grappleScript.aimDirection; //Reference the direction the player is aiming.
                                                                      //This is used for any dashes that use the crosshair

        
        /* Determining What Movement Type to Use */

        //If we're grappling we use grapple movement, if not then we use regular movement. That's it dawg
        if (isGrappling == true && inputDisabled == false)
        {
            grappleMove = true;
            movementBool = false;
        }
        else if (inputDisabled == false)
        {
            movementBool = true;
        }
        else
        {
            movementBool = false;
            grappleMove = false;
        }

        /* JUMPING */
        /* Checking for the jump button was moved from FixedUpdate to Update because for some reason when it was in 
         * fixed update it made every few jumps WAAAAAAY fuckin higher than they were supposed to be. */


        if (isGrounded && Input.GetButtonDown("Jump") && inputDisabled == false)
        {
            jumpBool = true;
        }

        if (isGrounded == true)
        {
            dashes = 2f;
        }

        //Debug.Log("isGrounded = " + isGrounded);

        /* Dashing */

        if (Input.GetButtonDown("Fire2") && dashes > 0 && canDash && !(moveX == 0 && moveY == 0) && inputDisabled == false)
        {
            //Debug.Log("Dash Button Pressed!");

            if (!isGrappling) //if you aren't grappling do whatever normal dash you selected
            {
                canDash = false; //we flip this bool to prevent you from dashing again on the immediate next frame
                dashBool = true; //this calls the dash function on the next FixedUpdate

            }
            else
            { //if you are grappling then do the grapple dash (doesn't currently work sry)
                GrappleDashBool = true; //This tells us to use the grapple dash
            }

            dashes--; //You just dashed bro so I gotta take one of your dashes sorry bro
        }

        //DirectionCheck();

    }

    void FixedUpdate()
    {
        /* Okay so if this is confusing let me explain why we're using FixedUpdate here. Essentially for physics based
         * movement type games (oh shit like this one wow!) it's ideal to grab inputs in Update but do anything having
         * to do with physics in FixedUpdate. Because of this the player controller should (in terms of physics shit)
         * flip a bool on Update and have that bool determine if a function is called on FixedUpdate.*/

        /* Collecting Inputs */

        //We need that RAW data for Button Axis Dashing so we collect those inputs too. Non raw inputs smooth the values
        //but raw inputs are ALWAYS 1, 0, or -1 meaning they are PERFECT for determining the 8 cardinal directions.
        //We need this because we want a Button Axis Dash in a specific direction to be the same EVERY time.
        float moveXRaw = Input.GetAxisRaw("Horizontal");
        float moveYRaw = Input.GetAxisRaw("Vertical");

        /* Jumping */
        if (jumpBool == true)
        {
            Jump();
        }

        /* Dashing */
        if (dashBool == true && (moveXRaw != 0 || moveYRaw != 0))
        {
            ButtonAxisDash(moveXRaw, moveYRaw);
        }

        if (GrappleDashBool == true && moveXRaw != 0)
        {
            GrappleDash(moveXRaw);
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

            // BEWARE YE WHO TREAD HERE
            // LEST YE TRY YET ANOTHER WAY OF MAKING OLAF NOT SWING AROUND LIKE A F#&%ING MUPPET

            //myRB.velocity += swingingForce * Time.deltaTime; //Apply velocity to our rigidbody to move it
            //myRB.velocity = myRB.velocity + swingingForce * swingSpeed * Time.deltaTime;
            //myRB.AddForce(grappleScript.currentSwingForceVector, ForceMode.Acceleration);

            // ok so it works, we unbroke the animator and now the swinging is so nice. 
            // we calculate the swing magnitude by multiplying the player's xInput by their
            // swing magnitude. 
            myRB.AddForce(grappleScript.CalculateSwingVector() * swingMagnitude);
        }
        else if (movementBool == true)
        {
            myRB.velocity = movementForce; //Apply velocity to our rigidbody to move it
        }

        /* This was originally the previously a check for the commented out "flip()" function but I changed it
        * because the flip function stupidly tried to flip the scaling into the negative but that IMMEDIATELY
        * fucked with all collision and facing left suddenly also meant sinking under the floor.*/


        DirectionCheck();
    }

    void DirectionCheck()
    {
        /* This function makes the player's character model face the correct direction */

        if (movementForce.x > 0 && !facingRight && inputDisabled == false)
        {
            transform.eulerAngles = new Vector3(0, 90, 0); // Facing Right
            facingRight = !facingRight;
        }
        else if (movementForce.x < 0 && facingRight && inputDisabled == false)
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

            if (ballManBool == true)
            {
                BallModeInactive(); //When we are grounded we want to make sure our character model is in regular form
                GetComponent<GrappleScriptEvenNewer>().stopGrappleBool = true;
            }
        }
        else
        {
            isGrounded = false;
        }

        myAnimator.SetBool("grounded", isGrounded); //The animator determines whether or not the jumping animation plays
                                                    //based on if the character is grounded.

        //Debug.Log("Grounded is: " + isGrounded);
    }

    void Jump()
    {
        myAnimator.SetBool("grounded", isGrounded); //The animator determines whether or not the jumping animation plays
                                                    //based on if the character is grounded.
        myRB.AddForce(new Vector3(0, jumpHeight, 0)); //Add some fucking force to make the character jump
        isGrounded = false; //If we're in the air we ain't grounded
        jumpBool = false; //gotta flip that bool so that it doesn't call this function on the next FixedUpdate
        BallModeActive(); //Activate BALL MAN MODE
    }

    public void ParryJump()
    {
        myAnimator.SetBool("grounded", isGrounded); //The animator determines whether or not the jumping animation plays
                                                    //based on if the character is grounded.
        myRB.AddForce(new Vector3(0, jumpHeight, 0)); //Add some fucking force to make the character jump
        isGrounded = false; //If we're in the air we ain't grounded
        jumpBool = false; //gotta flip that bool so that it doesn't call this function on the next FixedUpdate
        BallModeActive(); //Activate BALL MAN MODE
    }

    void ButtonAxisDash(float x, float y)
    {
        /* Description Start!
         * Button Axis Dashing takes the last directional input you gave in any of the 8 cardinal directions and 
         * dashes in that direction. 
         */
        if (ballManBool == false)
        {
            BallModeActive(); //Activate BALL MAN MODE
        }

        isDashing = true; //You are currently dashing my friend
        dashBool = false; //gotta flip that bool so that it doesn't call this function on the next FixedUpdate

        Vector3 dir = new Vector3(x, y / verticalEquilizer, myRB.velocity.z); //The direction we are about to dash in

        movementForce += dir * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                          //outside of the movement function.

        //This block of commented out code is my attempt at dashing by just using the rigidbody but it's fucked for some reason. 
        //So instead we modify the movementForce variable that then determines velocity in the rididbody. It's roundabout but it works
        //I guess...
        //Vector3 dir2 = new Vector3(x, y, myRB.velocity.z);
        //myRB.velocity += dir2 * dashSpeed;

        StartCoroutine(DashWait()); //This coroutine is a cooldown between dashes

        //Debug.Log("buttonAxisDash is fucking happening boi");
    }

    IEnumerator DashWait() //The cooldown between dash uses
    {
        //myRB.useGravity = false; //disabling gravity during dash is something we're considering but I can't recall if it
        //actually made a difference.
        yield return new WaitForSeconds(dashCooldown);
        //myRB.useGravity = true;
        isDashing = false;
        canDash = true;

        //Debug.Log("DashWait Happened");
    }

    void GrappleDash(float x)
    {
        Debug.Log("Oh God Oh Fuck I'm dashing while also fucking grappling hooooooly fuck");

        //if (facingRight == true)
        //{
        //myRB.AddForce(new Vector3(100, myRB.velocity.y, 0), ForceMode.Impulse);
        //myRB.AddForce(new Vector3(10000, myRB.velocity.y, 0));
        //}
        //else
        //{
        //myRB.AddForce(new Vector3(-100, myRB.velocity.y, 0), ForceMode.Impulse);
        //myRB.AddForce(new Vector3(-10000, myRB.velocity.y, 0));
        //}

        //Vector3 dir = new Vector3(x, myRB.velocity.y, myRB.velocity.z);
        Vector3 dir = new Vector3(x, 0, myRB.velocity.z);
        movementForce += dir * dashSpeed;

        StartCoroutine(DashWait());
        GrappleDashBool = false;

        /* Description Start!
         * 
         * Using the facingRight bool we can determine the direction to dash in. Then we give an impulse force in that direction. Make
         * sure this only works while grappling. 
         */
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

    IEnumerator SpeedCalcWait()
    {
        yield return new WaitForSeconds(0.05f);
        checkSpeed = true;
    }

    public void BallModeActive() //This function turns the player into a ball
    {
        StartCoroutine(BallModeWait());
        //ballManBool = true; //Ball Made Mode is active so we must flip the almighty bool to reflect that

        //Setting the correct mesh renderer active
        olafDummy.SetActive(false);
        ballDummy.SetActive(true);

        //Setting the correct collider active
        olafBallCollider.enabled = true;
        olafBodyCollider.enabled = false;

        //Debug.Log("Ball mode has FUCKING ENGAGED MOTHERFUCKER");
    }

    IEnumerator BallModeWait() //This coroutine is used to give us some buffer time between the next ground check.
    {
        yield return new WaitForSeconds(0.05f);
        ballManBool = true; //Ball Made Mode is active so we must flip the almighty bool to reflect that
    }
    public void BallModeInactive() //This function turns the player into his regular game model
    {
        ballManBool = false; //Ball Made Mode is NOT active so we must flip the almighty bool to reflect that

        //Setting the correct mesh renderer active
        olafDummy.SetActive(true);
        ballDummy.SetActive(false);

        //Setting the correct collider active
        olafBallCollider.enabled = false;
        olafBodyCollider.enabled = true;

        //Debug.Log("Ball mode is no longer FUCKING ENGAGED MOTHERFUCKER");
    }

}
