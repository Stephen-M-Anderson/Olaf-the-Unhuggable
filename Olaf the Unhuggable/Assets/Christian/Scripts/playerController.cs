using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{

    //movement variables
    public float runSpeed;
    public float swingSpeed;
    private Rigidbody myRB;
    private Animator myAnimator;
    private Vector3 movementForce;
    private Vector3 swingingForce;

    //float moveX;
    //float moveY;
    //float moveXRaw;
    //float moveYRaw;

    private float tempXRaw;
    private float tempYRaw;

    //float test;
    //private float moveX;

    public bool facingRight; //Used for changing the direction the character is facing

    //jumping variables
    bool jumpBool;
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

    public float dashes = 2f;

    public Transform crosshair;

    private Vector3 aimDir;
    private int dashDir;
    public float dashSpeed;
    public GameObject dashTowards;
    private Vector3 dashTowardsVect;
    private Vector3 dashFromVect;
    private bool canDash;
    private bool dashBool;
    private bool buttonAxisDashBool;
    private bool movementBool;
    public bool isDashing;

    private Vector3 dashPoint;

    public Text speedText;
    float speed;

    //Vector3 lastPosition;

    private bool checkSpeed;

    private float whichDash;

    private bool TwoDirectionDashBool;
    private bool OtherTwoDirectionDashBool;
    private bool AnyDirectionDashBool;
    private bool ThreeDirectionDashBool;
    private bool EightDirectionDashBool;
    private bool FourDirectionDashBool;
    private bool GrappleDashBool;

    public Text dashText;

    public float verticalEquilizer;

    private Vector3 grappleVectController;
    private Vector3 invertedGrappleVectController;
    public float grappleSpeed;
    private bool grappleMove;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        //dashTowardsVect = new Vector3(0, 0, 0);
        canDash = true;
        facingRight = true;
        //movementBool = true;
        checkSpeed = true;
        movementBool = true;

        whichDash = 1;
        dashText.text = "Button Axis Dash";
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        float moveXRaw = Input.GetAxisRaw("Horizontal");
        float moveYRaw = Input.GetAxisRaw("Vertical");

        myAnimator.SetFloat("speed", Mathf.Abs(moveX));
        movementForce = new Vector3(moveX * runSpeed, myRB.velocity.y, 0);
        swingingForce = new Vector3(moveX * swingSpeed, myRB.velocity.y, 0);

        if (checkSpeed)
        {
            SpeedCalc();
        }

        //tempXRaw = moveXRaw;
        //tempYRaw = moveYRaw;

        isGrappling = GetComponent<GrappleScriptEvenNewer>().isGrappling;
        aimDir = GetComponent<GrappleScriptEvenNewer>().aimDirection;

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

        if (isGrounded == true)
        {
            dashes = 2f;
        }

        PickAGrapple();
        

        if (Input.GetButtonDown("Fire2") && dashes > 0 && canDash)
        {
            //Debug.Log("Dash Button Pressed!");
            //canDash = false;

            /*if (isGrappling)
            {
                //GrappleDash();
            }*/ 

            if (!isGrappling)
            {
                //Dash();

                //if (moveXRaw != 0 || moveYRaw != 0)
                //{
                    //buttonAxisDashBool = true;
                    //ButtonAxisDash(moveXRaw, moveYRaw);
                //}
                //Dash();
                canDash = false;

                dashBool = true;

                //TwoDirectionDash();
                //OtherTwoDirectionDash();
                //AnyDirectionDash();
                //ThreeDirectionDash();
                //EightDirectionDash();
            } else
            {
                GrappleDashBool = true;
            }

            dashes--;
            //canDash = true;

        }

        //DebugLogs();
    }

    void FixedUpdate()
    {
        float moveXRaw = Input.GetAxisRaw("Horizontal");
        float moveYRaw = Input.GetAxisRaw("Vertical");
        

        if (jumpBool == true)
        {
            Jump();
        }

        if (dashBool == true)
        {
            //movementBool = false;
            Dash();
            //EightDirectionDash();
        }

        if (buttonAxisDashBool == true && (moveXRaw != 0 || moveYRaw != 0))
        {
            ButtonAxisDash(moveXRaw, moveYRaw);
        }


        if (TwoDirectionDashBool == true)
        {
            TwoDirectionDash();
        }

        if (OtherTwoDirectionDashBool == true)
        {
            OtherTwoDirectionDash();
        }

        if (AnyDirectionDashBool == true)
        {
            AnyDirectionDash();
        }

        if (ThreeDirectionDashBool == true)
        {
            ThreeDirectionDash();
        }

        if (FourDirectionDashBool == true)
        {
            FourDirectionDash();
        }

        if (EightDirectionDashBool == true)
        {
            EightDirectionDash();
        }

        if (GrappleDashBool == true && moveXRaw != 0)
        {
            GrappleDash(moveXRaw);
        }
    

        Movement();

        groundedCheck();


    }

    void Movement()
    {
        if (isGrappling == true && grappleMove == true)
        {
            /*grappleMove = false;
            grappleVectController = GetComponent<GrappleScriptEvenNewer>().grappleVect;
            invertedGrappleVectController = new Vector3(-grappleVectController.x, -grappleVectController.y, grappleVectController.z);
            myRB.AddForce(invertedGrappleVectController * grappleSpeed);
            StartCoroutine(grappleMoveWait());*/

            myRB.velocity = swingingForce;
        }
        else if (movementBool == true)
        {
            myRB.velocity = movementForce;
        }

        /* This was originally the previously a check for the commented out "flip()" function but I changed it
        * because the flip function stupidly tried to flip the scaling into the negative but that IMMEDIATELY
        * fucked with all collision and facing left suddenly also meant sinking under the floor.*/
        //if (move > 0 && !facingRight && !isGrapplingController)
        //if (move > 0 && !facingRight && !isGrappling)
        if (movementForce.x > 0 && !facingRight)
        {
            transform.eulerAngles = new Vector3(0, 90, 0); // Facing Right
            facingRight = !facingRight;
        }
        //else if(move < 0 && facingRight && !isGrapplingController)
        //else if (move < 0 && facingRight && !isGrappling)
        else if (movementForce.x < 0 && facingRight)
        {
            transform.eulerAngles = new Vector3(0, 270, 0); // Facing Left
            facingRight = !facingRight;
        }
    }

    void groundedCheck()
    {
        /* CHECKING IF THE PLAYER IS GROUNDED FOR LANDING */

        groundCollisions = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (groundCollisions.Length > 0)
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
        jumpBool = false;
    }

    void DebugLogs()
    {
        Debug.Log("isGrounded = " + isGrounded);
    }

    void Dash()
    {
        //Debug.Log("Oh God Oh Fuck I'm dashing");
        /* Description Start!
         * 
         * This is where the final dash mechanic we go with will be placed.
         */

        switch (whichDash)
        {
            case 1:
                buttonAxisDashBool = true;
                break;

            case 2:
                TwoDirectionDashBool = true;
                break;

            case 3:
                OtherTwoDirectionDashBool = true;
                break;

            case 4:
                FourDirectionDashBool = true;
                break;

            case 5:
                ThreeDirectionDashBool = true;
                break;

            case 6:
                EightDirectionDashBool = true;
                break;

            case 7:
                AnyDirectionDashBool = true;
                break;

            case 8:
                GrappleDashBool = true;
                break;
           

        }


        //buttonAxisDashBool = true;
        isDashing = true;



        //EightDirectionDash();

        dashBool = false;
    }

    //void ButtonAxisDash()
    void ButtonAxisDash(float x, float y)
    {
        //myRB.velocity = Vector3.zero;
        Vector3 dir = new Vector3(x, y/verticalEquilizer, myRB.velocity.z);
        movementForce += dir * dashSpeed;
        //myRB.velocity += dir.normalized * 25;
        //myRB.velocity += dir.normalized * dashSpeed;

        //Debug.Log("buttonAxisDash is fucking happening boi");

        StartCoroutine(DashWait());

        buttonAxisDashBool = false;
    }

    IEnumerator DashWait()
    {
        //StartCoroutine(GroundDash());

        //myRB.useGravity = false;
        yield return new WaitForSeconds(.5f);
        //myRB.useGravity = true;
        isDashing = false;
        canDash = true;

        //Debug.Log("DashWait Happened");
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        //Debug.Log("GroundDash Happened");
        //if (isGrounded)
        //hasDashed = false;
    }

    void TwoDirectionDash()
    {
        //Debug.Log("Oh God Oh Fuck I'm two directional dashing");

        /* Description Start!
         * 
         * Using the facingRight bool we can determine the direction to dash in. Then we give an impulse force in that direction.
         */

        if (facingRight == true)
        {
            //myRB.AddForce(new Vector3(300, 0, 0), ForceMode.Impulse);
            //myRB.AddForce(new Vector3(10000, myRB.velocity.y, 0));
            Vector3 dir = new Vector3(1,0,0);
            movementForce += dir * dashSpeed;
            StartCoroutine(DashWait());

        } else
        {
            //myRB.AddForce(new Vector3(-300, 0, 0), ForceMode.Impulse);
            //myRB.AddForce(new Vector3(-10000, myRB.velocity.y, 0));
            Vector3 dir = new Vector3(-1, 0, 0);
            movementForce += dir * dashSpeed;
            StartCoroutine(DashWait());
        }

        TwoDirectionDashBool = false;
    }

    void OtherTwoDirectionDash()
    {
        //Debug.Log("Oh God Oh Fuck I'm two directional dashing using the crosshair to determine direction FUUUUUUUUUUUCk");

        /* Description Start!
        * 
        * Using the crosshair's direction we determine if the dash should be left or right. The circle the crosshair travels around
        * the player will be divided in two 180 degree chunks to determine left or right. 
        * 
        * * dashDir key:
        * 1 = Up
        * 2 = Up-Right
        * 3 = Right
        * 4 = Down-Right
        * 5 = Down
        * 6 = Down-Left
        * 7 = Left
        * 8 = Up-Left
        */

        switch (aimDir.x > 0 ? "Right" :
           aimDir.x < 0 ? "Left" : "Other")
        {
            case "Right":
                dashDir = 3;
                break;

            case "Left":
                dashDir = 7;
                break;

            case "Other":
                dashDir = 9;
                break;
        }

        switch (dashDir)
        {
            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");

                dashPoint = new Vector3(1, 0, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                OtherTwoDirectionDashBool = false;
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");

                dashPoint = new Vector3(-1, 0, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                OtherTwoDirectionDashBool = false;
                break;

            case 9:
                Debug.Log("This case should never happen what the fuck?");
                break;
        }
    }

    void AnyDirectionDash()
    {
        //Debug.Log("Oh God Oh Fuck I'm dashing in ANY DIRECTION");

        /* Description Start!
        * 
        * Using the crosshair's direction we determine where the dash should go. This will work exactly like the grapple being 
        * shot out.
        */

        Vector3 dashDir = crosshair.transform.position - transform.position;
        Vector3 dir = new Vector3(1, 0, 0);
        movementForce += dashDir * dashSpeed;

        StartCoroutine(DashWait());
        AnyDirectionDashBool = false;

        //dashDirection = new Vector3(crosshair.position.x, crosshair.position.y);

        // myRB.AddForce(dashDir * 10, ForceMode.Impulse);

        //crosshair.position

    }

    void ThreeDirectionDash()
    {
        //Debug.Log("Oh God Oh Fuck I'm three directional dashing");

        /* Description Start!
        * 
        * Using the crosshair's direction we determine if the dash should be left, right, or down. The bottom half of circle the 
        * crosshair travels around the player will be divided into three equal parts of 60 degrees.
        * 
        * * dashDir key:
        * 1 = Up
        * 2 = Up-Right
        * 3 = Right
        * 4 = Down-Right
        * 5 = Down
        * 6 = Down-Left
        * 7 = Left
        * 8 = Up-Left
        */

        switch (aimDir.x > 0 && aimDir.y < 1 && aimDir.y > -0.7 ? "Right" :
           aimDir.x >= -0.7 && aimDir.x <= 0.7 && aimDir.y < 0 ? "Down" :
           aimDir.x < 0 && aimDir.y < 1 && aimDir.y > -0.7 ? "Left" : "Other")
        {

            case "Right":
                dashDir = 3;
                break;

            case "Down":
                dashDir = 5;
                break;

            case "Left":
                dashDir = 7;
                break;

            case "Other":
                dashDir = 9;
                break;
        }

        switch (dashDir)
        {

            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");
                //myRB.AddForce(new Vector3(300, 0, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(5, 0, 0));

                //myRB.velocity = new Vector3(1 + myRB.velocity.x * 3f, myRB.velocity.y, 0);

                //dashPoint = new Vector3(1, 0, 0);
                //myRB.velocity = dashPoint * 15;

                dashPoint = new Vector3(1, 0, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                ThreeDirectionDashBool = false;
                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");
                //myRB.AddForce(new Vector3(0, -20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(0, -0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(0, -5, 0));

                dashPoint = new Vector3(0, -1f / verticalEquilizer, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                ThreeDirectionDashBool = false;
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");
                //myRB.AddForce(new Vector3(-300, 0, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(-1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(-500, 0, 0) * Time.deltaTime);

                dashPoint = new Vector3(-1, 0, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                ThreeDirectionDashBool = false;
                break;

            case 9:
                Debug.Log("This case should never happen what the fuck?");
                break;
        }

    }

    void FourDirectionDash()
    {
        Debug.Log("Oh God Oh Fuck I'm four directional dashing");

        /* Description Start!
        * 
        * Using the crosshair's direction we determine if the dash should be Up, Down, Left, or Right. The
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



        switch (aimDir.x >= -0.7 && aimDir.x <= 0.7 && aimDir.y >= 0.7 ? "UP" :
            aimDir.x >= -0.7 && aimDir.x <= 0.7 && aimDir.y <= -0.7 ? "Down" :
            aimDir.y >= -0.7 && aimDir.y <= 0.7 && aimDir.x <= -0.7 ? "Left" :
            aimDir.y >= -0.7 && aimDir.y <= 0.7 && aimDir.x >= 0.7 ? "Right" : "Other")
        {
            case "UP":
                dashDir = 1;
                break;

            case "Right":
                dashDir = 3;
                break;

            case "Down":
                dashDir = 5;
                break;

            case "Left":
                dashDir = 7;
                break;

            case "Other":
                dashDir = 9;
                break;
        }

        switch (dashDir)
        {
            case 1:
                Debug.Log("Oh God Oh Fuck I'm dashing UP!");

                //myRB.AddForce(new Vector3(0, 20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(0, 0.1f, 0) * dashSpeed);

                //dashTowardsVect = transform.position + new Vector3(0, 5, 0);
                //myRB.AddForce(dashTowardsVect * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(0, 5, 0));

                //dashTowardsVect = new Vector3(0, 5f, 0);
                //myRB.AddForce((dashTowardsVect).normalized * myRB.mass * dashSpeed);

                //myRB.velocity = new Vector3(0, 1, 0) * 5;
                //dashPoint = new Vector3(0, 1, 0);
                //myRB.velocity = dashPoint * 15;

                dashPoint = new Vector3(0, 1f / verticalEquilizer, 0);
                movementForce += dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                FourDirectionDashBool = false;
                break;

            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");
                //myRB.AddForce(new Vector3(300, 0, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(5, 0, 0));

                //myRB.velocity = new Vector3(1 + myRB.velocity.x * 3f, myRB.velocity.y, 0);

                //dashPoint = new Vector3(1, 0, 0);
                //myRB.velocity = dashPoint * 15;

                dashPoint = new Vector3(1, 0, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                FourDirectionDashBool = false;
                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");
                //myRB.AddForce(new Vector3(0, -20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(0, -0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(0, -5, 0));

                dashPoint = new Vector3(0, -1f / verticalEquilizer, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                FourDirectionDashBool = false;
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");
                //myRB.AddForce(new Vector3(-300, 0, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(-1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(-500, 0, 0) * Time.deltaTime);

                dashPoint = new Vector3(-1, 0, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                FourDirectionDashBool = false;
                break;

            case 9:
                Debug.Log("This case should never happen what the fuck?");
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

                //myRB.AddForce(new Vector3(0, 20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(0, 0.1f, 0) * dashSpeed);

                //dashTowardsVect = transform.position + new Vector3(0, 5, 0);
                //myRB.AddForce(dashTowardsVect * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(0, 5, 0));

                //dashTowardsVect = new Vector3(0, 5f, 0);
                //myRB.AddForce((dashTowardsVect).normalized * myRB.mass * dashSpeed);

                //myRB.velocity = new Vector3(0, 1, 0) * 5;
                //dashPoint = new Vector3(0, 1, 0);
                //myRB.velocity = dashPoint * 15;

                dashPoint = new Vector3(0, 1f / verticalEquilizer, 0);
                movementForce += dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 2:
                Debug.Log("Oh God Oh Fuck I'm dashing UP-Right!");
                //myRB.AddForce(new Vector3(150, 100, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3 (1, 0.1f, 0) * dashSpeed);

                //dashTowardsVect = transform.position + new Vector3(5, 5, 0);
                //myRB.AddForce(dashTowardsVect * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(5, 5, 0));

                //dashTowardsVect = new Vector3(5f, 5f, 0);
                //myRB.AddForce((dashTowardsVect).normalized * myRB.mass * dashSpeed);

                dashPoint = new Vector3(1, 1f / verticalEquilizer, 0);
                movementForce = dashPoint * dashSpeed;

                //myRB.velocity = dashPoint * 15;

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");
                //myRB.AddForce(new Vector3(300, 0, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(5, 0, 0));

                //myRB.velocity = new Vector3(1 + myRB.velocity.x * 3f, myRB.velocity.y, 0);

                //dashPoint = new Vector3(1, 0, 0);
                //myRB.velocity = dashPoint * 15;

                dashPoint = new Vector3(1, 0, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 4:
                Debug.Log("Oh God Oh Fuck I'm dashing Down-Right!");
                //myRB.AddForce(new Vector3(150, -20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(1, -0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(5, -5, 0));

                dashPoint = new Vector3(1, -1f / verticalEquilizer, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");
                //myRB.AddForce(new Vector3(0, -20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(0, -0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(0, -5, 0));

                dashPoint = new Vector3(0, -1f / verticalEquilizer, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 6:
                Debug.Log("Oh God Oh Fuck I'm dashing Down-Left!");
                //myRB.AddForce(new Vector3(-150, -20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(-1, -0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(-5, -5, 0));

                dashPoint = new Vector3(-1, -1f / verticalEquilizer, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");
                //myRB.AddForce(new Vector3(-300, 0, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(-1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(-500, 0, 0) * Time.deltaTime);

                dashPoint = new Vector3(-1, 0, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 8:
                Debug.Log("Oh God Oh Fuck I'm dashing Up-Left!");
                //myRB.AddForce(new Vector3(-150, 20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(-1, 0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(-5, 5, 0) * Time.deltaTime);

                dashPoint = new Vector3(-1, 1f / verticalEquilizer, 0);
                movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 9:
                Debug.Log("This case should never happen what the fuck?");
                break;
        }

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
        //var vel = myRB.velocity;
        //speed = myRB.velocity.magnitude;
        //speed = (transform.position - lastPosition).magnitude * 100;
        //lastPosition = transform.position;

        //this one puts it in mph:
        speed = myRB.velocity.magnitude * 2.237f;
        speedText.text = speed.ToString();
        StartCoroutine(SpeedCalcWait());
    }

    IEnumerator SpeedCalcWait()
    {
        yield return new WaitForSeconds(0.05f);
        checkSpeed = true;
    }

    IEnumerator grappleMoveWait()
    {
        yield return new WaitForSeconds(0.1f);
        grappleMove = true;
    }

    void PickAGrapple()
    {
        //if (!isGrappling)
        //{
            if (Input.GetKeyDown("1"))
            {
                whichDash = 1;
            }

            if (Input.GetKeyDown("2"))
            {
                whichDash = 2;
            }

            if (Input.GetKeyDown("3"))
            {
                whichDash = 3;
            }

            if (Input.GetKeyDown("4"))
            {
                whichDash = 4;
            }

            if (Input.GetKeyDown("5"))
            {
                whichDash = 5;
            }

            if (Input.GetKeyDown("6"))
            {
                whichDash = 6;
            }

            if (Input.GetKeyDown("7"))
            {
                whichDash = 7;
            }

        //}
       // else
       //{
           // whichDash = 8;
           // dashText.text = "Grapple Dash";
        //}

        switch (whichDash)
        {
            case 1:
                dashText.text = "Button Axis Dash";
                break;

            case 2:
                dashText.text = "Two Directional Dash";
                break;

            case 3:
                dashText.text = "Other Two Directional Dash";
                break;

            case 4:
                dashText.text = "Four Directional Dash";
                break;

            case 5:
                dashText.text = "Three Directional Dash";
                break;

            case 6:
                dashText.text = "Eight Directional Dash";
                break;

            case 7:
                dashText.text = "Any Direction Dash";
                break;

        }
    }

    /*void Flip()
    {
        facingRight = !facingRight;
        Vector3 flipVector = transform.localScale;
        flipVector.z *= -1;
        transform.localScale = flipVector;
    }*/



}
