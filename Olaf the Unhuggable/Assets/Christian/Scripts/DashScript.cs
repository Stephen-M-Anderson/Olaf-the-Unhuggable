/* This was my attempt to isolate dashing into its own script but I gave up on it and kept it all in the player controller.
 * As such I will now move it to the spaghetti code folder and not fix it up. BYEEEEEEEEEEEEEEEEEEEEEEEEEEEE
 * 
 * Update: Nvm gonna fuckin make this script work fuck iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiit.
 * 
 * Description: 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScript : MonoBehaviour
{

    private Rigidbody myRB;
    private Animator myAnimator;

    private bool facingRightDash;

    private bool isGrappling;

    //CHANGE THIS LATER TO NOT HAVE SO MANY DASHES
    float dashes = 200f;

    public Transform crosshair;

    private Vector3 aimDir;
    private int dashDir;
    public float dashSpeed;
    public GameObject dashTowards;
    private GameObject currDashTowards;
    private Vector3 dashTowardsVect;
    private Vector3 dashFromVect;
    private bool canDash;
    private bool dashBool;
    private bool buttonAxisDashBool;
    private bool movementBool;

    private Vector3 dashPoint;


    // Start is called before the first frame update
    void Start()
    {
        facingRightDash = GetComponent<playerController>().facingRight;

        myRB = GetComponent<Rigidbody>();
        canDash = true;

    }

    // Update is called once per frame
    void Update()
    {

        aimDir = GetComponent<GrappleScriptEvenNewer>().aimDirection;

        float moveXRaw = Input.GetAxisRaw("Horizontal");
        float moveYRaw = Input.GetAxisRaw("Vertical");

        isGrappling = GetComponent<GrappleScriptEvenNewer>().isGrappling;

        if (Input.GetButtonDown("Fire2") && dashes > 0 && canDash)
        {

            //canDash = false;

            /*if (isGrappling)
            {
                //GrappleDash();
            }*/

            if (!isGrappling)
            {
                //Dash();

                if (moveXRaw != 0 || moveYRaw != 0)
                {
                    //buttonAxisDashBool = true;
                    //ButtonAxisDash(moveXRaw, moveYRaw);
                }
                //Dash();
                canDash = false;
                dashBool = true;
                //TwoDirectionDash();
                //OtherTwoDirectionDash();
                //AnyDirectionDash();
                //ThreeDirectionDash();
                //EightDirectionDash();
            }

            dashes--;
            //canDash = true;

        }
    }

    void FixedUpdate()
    {
        float moveXRaw = Input.GetAxisRaw("Horizontal");
        float moveYRaw = Input.GetAxisRaw("Vertical");

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
    }

    void Dash()
    {
        Debug.Log("Oh God Oh Fuck I'm dashing");
        /* Description Start!
         * 
         * This is where the final dash mechanic we go with will be placed.
         */

        EightDirectionDash();

        dashBool = false;
    }

    void ButtonAxisDash(float x, float y)
    {
        //myRB.velocity = Vector3.zero;
        Vector3 dir = new Vector3(x, y, myRB.velocity.z);
        myRB.velocity += dir.normalized * dashSpeed;

        Debug.Log("buttonAxisDash is fucking happening boi");

        StartCoroutine(DashWait());

        buttonAxisDashBool = false;
    }

    void TwoDirectionDash()
    {
        Debug.Log("Oh God Oh Fuck I'm two directional dashing");

        /* Description Start!
         * 
         * Using the facingRight bool we can determine the direction to dash in. Then we give an impulse force in that direction.
         */

        if (facingRightDash == true)
        {
            myRB.AddForce(new Vector3(300, 0, 0), ForceMode.Impulse);
            //myRB.AddForce(new Vector3(10000, myRB.velocity.y, 0));
        }
        else
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
                dashDir = 3;
                break;

            case "Right":
                dashDir = 3;
                break;

            case "Down-Right":
                dashDir = 3;
                break;

            case "Down":
                dashDir = 5;
                break;

            case "Down-Left":
                dashDir = 7;
                break;

            case "Left":
                dashDir = 7;
                break;

            case "Up-Left":
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

                myRB.AddForce(new Vector3(1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(5, 0, 0));

                //myRB.velocity = new Vector3(1, 0, 0) * 5;

                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");
                //myRB.AddForce(new Vector3(-300, 0, 0), ForceMode.Impulse);

                myRB.AddForce(new Vector3(-1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(-500, 0, 0) * Time.deltaTime);


                break;

            case 9:
                Debug.Log("This case should never happen what the fuck?");
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

            case 3:
                Debug.Log("Oh God Oh Fuck I'm dashing Right!");
                //myRB.AddForce(new Vector3(300, 0, 0), ForceMode.Impulse);

                myRB.AddForce(new Vector3(1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(5, 0, 0));

                //myRB.velocity = new Vector3(1, 0, 0) * 5;


                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");
                //myRB.AddForce(new Vector3(0, -20, 0), ForceMode.Impulse);

                myRB.AddForce(new Vector3(0, -0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(0, -5, 0));


                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");
                //myRB.AddForce(new Vector3(-300, 0, 0), ForceMode.Impulse);

                myRB.AddForce(new Vector3(-1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(-500, 0, 0) * Time.deltaTime);


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


                //dashTowardsVect = myRB.position + new Vector3(0, 3, 0);
                //Instantiate(dashTowards, dashTowardsVect, myRB.rotation);
                //currDashTowards = GameObject.Find("DashTowards(Clone)");
                //Vector3 dashHere = currDashTowards.transform.position;
                //dashHere = dashTowardsVect - myRB.position;
                //myRB.velocity = dashHere * 5;

                //dashPoint = new Vector3(0, 0.1f, 0);
                //movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
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

                dashPoint = new Vector3(1, 0.1f, 0);
                //movementForce = dashPoint * dashSpeed;


                //myRB.velocity = dashPoint * 15;

                StartCoroutine(DashWait());
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
                //movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                break;

            case 4:
                Debug.Log("Oh God Oh Fuck I'm dashing Down-Right!");
                //myRB.AddForce(new Vector3(150, -20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(1, -0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(5, -5, 0));

                dashPoint = new Vector3(1, -0.1f, 0);
                //movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                break;

            case 5:
                Debug.Log("Oh God Oh Fuck I'm dashing Down!");
                //myRB.AddForce(new Vector3(0, -20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(0, -0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(0, -5, 0));

                dashPoint = new Vector3(0, -0.1f, 0);
                //movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                break;

            case 6:
                Debug.Log("Oh God Oh Fuck I'm dashing Down-Left!");
                //myRB.AddForce(new Vector3(-150, -20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(-1, -0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + new Vector3(-5, -5, 0));

                dashPoint = new Vector3(-1, -0.1f, 0);
                //movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                break;

            case 7:
                Debug.Log("Oh God Oh Fuck I'm dashing Left!");
                //myRB.AddForce(new Vector3(-300, 0, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(-1, 0, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(-500, 0, 0) * Time.deltaTime);

                dashPoint = new Vector3(-1, 0, 0);
                //movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                break;

            case 8:
                Debug.Log("Oh God Oh Fuck I'm dashing Up-Left!");
                //myRB.AddForce(new Vector3(-150, 20, 0), ForceMode.Impulse);

                //myRB.AddForce(new Vector3(-1, 0.1f, 0) * dashSpeed);

                //myRB.MovePosition(transform.position + myRB.velocity + new Vector3(-5, 5, 0) * Time.deltaTime);

                dashPoint = new Vector3(-1, 0.1f, 0);
                //movementForce = dashPoint * dashSpeed;

                StartCoroutine(DashWait());
                break;

            case 9:
                Debug.Log("This case should never happen what the fuck?");
                break;
        }

    }

    IEnumerator DashWait()
    {
        //StartCoroutine(GroundDash());

        //myRB.useGravity = false;
        //isDashing = true;
        yield return new WaitForSeconds(.5f);
        //myRB.useGravity = true;
        //isDashing = false;
        canDash = true;

        Debug.Log("DashWait Happened");
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        Debug.Log("GroundDash Happened");
        //if (isGrounded)
        //hasDashed = false;
    }
}
