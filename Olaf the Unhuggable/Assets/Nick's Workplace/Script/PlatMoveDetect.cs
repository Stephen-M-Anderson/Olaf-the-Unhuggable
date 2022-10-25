using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatMoveDetect : MonoBehaviour
{
    public bool platCheck;
    public Transform player;
    private RaycastHit platProbe;
    public GrappleScriptEvenNewer GrappleScript;
    private bool isGrappling;
    
    //This entire code could have been made easier if I just kept it simple stupid.
    // Start is called before the first frame update
    void Start()
    {
        platCheck = true;
    }

        // Update is called once per frame
        void Update()
    {

        PlayerPlatformParenter();

        PlayerParentYeeter();

        isGrappling = GrappleScript.isGrappling;   
    }

    private void OnTriggerEnter(Collider collision)     //Both OnTriggerEnter/exit and OnCollisionEnter/exit are linked to ProcessCollison
    {
        ProcessCollision(collision.gameObject);
        //Debug.Log("collision detected");
    }

    private void OnTriggerExit(Collider collision)
    {
        ProcessCollision(collision.gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        ProcessCollision(collision.gameObject);
        //Debug.Log("collision detected");
    }
    private void OnCollisionExit(Collision collision)
    {
        ProcessCollision(collision.gameObject);
    }

    //This function just detects whether a platform is collided with and then tells Platcheck to check if the player needs a parent.
    //I chose to only check on collision whether things are done, because the ontriggerexit stuff can get... finicky. 
    void ProcessCollision(GameObject collider)         //Process Collision takes the use cases where a player hits a platform and checks to see if they're standing on it. 
   
    {
        if (collider.CompareTag("Platform"))            //Only checks collider data from an object with the platform tag
        {
            Debug.Log("Platform Collision Detected.");
            platCheck = false;                          //Tells Platcheck to get off its ass and check if it should parent the platform detected.

        }
        else
        {
            player.SetParent(null);
        }
    }

    //Anytime we get a movement control that means that a player defnitely wants to dismount a platform, player should no longer be parented to a platform.
    //This should include but isn't limited to jumping, grappling, and any other drastic movement options.
    void PlayerParentYeeter()                   
    {
        if (Input.GetKeyDown("space"))  //NOTE TO SELF. ADD/Test USE CASE TO ONLY UNPARENT WHEN ACTUALLY GRAPPLING TARGET.
        {
            player.SetParent(null);
            Debug.Log("Yeeting parent because of controls");
        }
    }

    //This script checks if the player is on top of the platform and THEN parents it if so. If the platform is approached...
    //... from the side though, it should be ignored and then not parented.
    void PlayerPlatformParenter()
    {
        if (platCheck != true)                                          //Takes in switch input from collider and trigger enter/exit.
        {

            Ray platRay = new Ray(transform.position, Vector3.down);    //Raycast is generated to check platform. Raycast is cast from object position situated .130f down.
            if (Physics.Raycast(platRay, out platProbe, .130f))
            {
                if (platProbe.collider.CompareTag("Platform"))          //Only works if ray collides with a platform
                {
                    player.SetParent(platProbe.transform);              //Makes the character a child of the platform/collided target.
                    Debug.Log("Parent is Set");
                }
                else                                                    // If it doesn't detect a platform underfoot, then FUCK OFF.
                {
                    Debug.Log("You have no parents");
                    player.SetParent(null);                             //The code that does child yeeting.
                }
                platCheck = true;                                       //Tell the code that the platcheck went off and avoid 20000 parentings going off all the damn fucking time.
            }
        }
    }
}
