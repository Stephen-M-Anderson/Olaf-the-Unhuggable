/* This was my attempt to isolate dashing into its own script but I gave up on it and kept it all in the player controller.
 * As such I will now move it to the spaghetti code folder and not fix it up. BYEEEEEEEEEEEEEEEEEEEEEEEEEEEE
 * 
 * Update: Nvm gonna fuckin make this script work fuck iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiit.
 * 
 * Description: This script handles all dashing mechanics. For some weird reason when moving all this code over from the 
 * playerController script every dash EXCEPT for the Button Axis Dash broke... But we were going with the button Axis Dash
 * anyway soo... whatever fuck it.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashScript : MonoBehaviour
{

    

    [Header("Outside Components")]
    private Rigidbody myRB; //A reference to the player's rigidbody component
    private Animator myAnimator; //A reference to the player's animator component
    bool isGrounded = false; //The bool used for ground detection. If this is true then the character is on solid ground.
    //public Vector3 movementForce; //Speed of movement plus a direction given by user input (dpad)
    public bool facingRight = true; //Used for changing the direction the character is facing


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
    public float verticalEquilizer; //This one is a bit complicated. For some weird reason the way we're implementing
                                    //movement, vertical and horizontal forces are not created equal. It takes WAY more
                                    //force to move something vertically than horizontally. I used this value as an offset
                                    //to try and balance those out a little better.

    [Header("Dash Changing")]
    private float whichDash; //This number correlates to a specific dash we want to change to for debug purposes.

    //These bools dictate which type of dash you want to use:
    private bool buttonAxisDashBool = false; //Dashing in the direction of the last directional inputs you moved towards
    private bool TwoDirectionDashBool = false; //Dashing Left or Right determined by last direction faced
    private bool OtherTwoDirectionDashBool = false; //Dashing Left or Right determined by crosshair direction
    private bool AnyDirectionDashBool = false; //Dash in ANY fucking direction (shit don't work rip)
    private bool ThreeDirectionDashBool = false; //Dash Left, Right, or Down depending on crosshair direction
    private bool EightDirectionDashBool = false; //Dash in the Eight Cardinal Directions depending on crosshair direction
    private bool FourDirectionDashBool = false; //Dash Up, Down, Left, or Right depending on Crosshair direction
    private bool GrappleDashBool = false; //This is meant to be a special dash you can do while grappling... shits busted

    public Text dashText; //This is where we store the text of which dash you're currently using

    [Header("Grappling Variables")]
    private bool isGrappling = false; //If this bool is true then the player is currently grappling

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>(); //setting myRB to reference the rigidbody component of our player
        myAnimator = GetComponent<Animator>(); //setting myAnimator to reference the animator component of our player

        whichDash = 1; //By default this is set to '1' which means Button Axis Dashing
        dashText.text = "Button Axis Dash";
    }

    // Update is called once per frame
    void Update()
    {
        aimDir = GetComponent<GrappleScriptEvenNewer>().aimDirection; //Reference the direction the player is aiming.
                                                                      //This is used for any dashes that use the crosshair
        isGrounded = GetComponent<playerController>().isGrounded;
        facingRight = GetComponent<playerController>().facingRight;

        /* Dashing */

        PickADash(); //This function takes the whichDash int and assigns you a dash based on that


        if (Input.GetButtonDown("Fire2") && dashes > 0 && canDash)
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

        if (isGrounded == true)
        {
            dashes = 2f; //Touching ground resets your dashes
        }
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

        /* Dashing */

        if (dashBool == true)
        {
            Dash();
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

    }

    void Dash()
    {
        //Debug.Log("Oh God Oh Fuck I'm dashing");
        /* Description Start!
         * 
         * This is where the final dash mechanic we go with will be placed.
         */

        //This switch case determines which dash is used. 
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
        isDashing = true; //You are currently dashing my friend
        dashBool = false; //gotta flip that bool so that it doesn't call this function on the next FixedUpdate
    }

    void ButtonAxisDash(float x, float y)
    {
        /* Description Start!
         * Button Axis Dashing takes the last directional input you gave in any of the 8 cardinal directions and 
         * dashes in that direction. 
         */

        Vector3 dir = new Vector3(x, y / verticalEquilizer, myRB.velocity.z); //The direction we are about to dash in
        GetComponent<playerController>().movementForce += dir * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                          //outside of the movement function.

        //Debug.Log("buttonAxisDash is fucking happening boi");

        StartCoroutine(DashWait()); //This coroutine is a cooldown between dashes

        buttonAxisDashBool = false;
    }

    IEnumerator DashWait() //The cooldown between dash uses
    {
        //myRB.useGravity = false; //disabling gravity during dash is something we're considering but I can't recall if it
        //actually made a difference.
        yield return new WaitForSeconds(.5f);
        //myRB.useGravity = true;
        isDashing = false;
        canDash = true;

        //Debug.Log("DashWait Happened");
    }

    void TwoDirectionDash()
    {
        //Debug.Log("Oh God Oh Fuck I'm two directional dashing");

        /* Description Start!
         * 
         * Using the facingRight bool we can determine the direction to dash in. Then we give an impulse force in that 
         * direction.
         */

        if (facingRight == true)
        {
            Vector3 dir = new Vector3(1, 0, 0); //The direction we are about to dash in
            GetComponent<playerController>().movementForce += dir * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                              //outside of the movement function.
            StartCoroutine(DashWait());

        }
        else
        {
            Vector3 dir = new Vector3(-1, 0, 0); //The direction we are about to dash in
            GetComponent<playerController>().movementForce += dir * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                              //outside of the movement function.
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

                dashPoint = new Vector3(1, 0, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                OtherTwoDirectionDashBool = false;
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");

                dashPoint = new Vector3(-1, 0, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

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
        Vector3 dir = new Vector3(1, 0, 0); //The direction we are about to dash in
        GetComponent<playerController>().movementForce += dashDir * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                              //outside of the movement function.

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

                dashPoint = new Vector3(1, 0, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                ThreeDirectionDashBool = false;
                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");

                dashPoint = new Vector3(0, -1f / verticalEquilizer, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                ThreeDirectionDashBool = false;
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");

                dashPoint = new Vector3(-1, 0, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

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

                dashPoint = new Vector3(0, 1f / verticalEquilizer, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce += dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to 
                                                        //our player outside of the movement function.

                StartCoroutine(DashWait());
                FourDirectionDashBool = false;
                break;

            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");

                dashPoint = new Vector3(1, 0, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                FourDirectionDashBool = false;
                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");

                dashPoint = new Vector3(0, -1f / verticalEquilizer, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                FourDirectionDashBool = false;
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");

                dashPoint = new Vector3(-1, 0, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

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

                dashPoint = new Vector3(0, 1f / verticalEquilizer, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce += dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                        //outside of the movement function.

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 2:
                Debug.Log("Oh God Oh Fuck I'm dashing UP-Right!");

                dashPoint = new Vector3(1, 1f / verticalEquilizer, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");

                dashPoint = new Vector3(1, 0, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 4:
                Debug.Log("Oh God Oh Fuck I'm dashing Down-Right!");

                dashPoint = new Vector3(1, -1f / verticalEquilizer, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");

                dashPoint = new Vector3(0, -1f / verticalEquilizer, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 6:
                Debug.Log("Oh God Oh Fuck I'm dashing Down-Left!");

                dashPoint = new Vector3(-1, -1f / verticalEquilizer, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");

                dashPoint = new Vector3(-1, 0, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

                StartCoroutine(DashWait());
                EightDirectionDashBool = false;
                break;

            case 8:
                Debug.Log("Oh God Oh Fuck I'm dashing Up-Left!");

                dashPoint = new Vector3(-1, 1f / verticalEquilizer, 0); //The direction we are about to dash in
                GetComponent<playerController>().movementForce = dashPoint * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                                       //outside of the movement function.

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
        /* Description Start!
         * 
         * Using the facingRight bool we can determine the direction to dash in. Then we give an impulse force in that 
         * direction. Make sure this only works while grappling.
         */

        Debug.Log("Oh God Oh Fuck I'm dashing while also fucking grappling hooooooly fuck");

        Vector3 dir = new Vector3(x, 0, myRB.velocity.z); //The direction we are about to dash in
        GetComponent<playerController>().movementForce += dir * dashSpeed; //Modifying the movementForce value is how we give velocity to our player
                                          //outside of the movement function.

        StartCoroutine(DashWait());
        GrappleDashBool = false;
    }

    void PickADash() //This function controls a switch case and a series of hideous if statements. These determine
                     //which dash to use for testing purposes upon user input (keys 1-7).
    {
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

}