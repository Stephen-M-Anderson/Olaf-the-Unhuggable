﻿/* Known glitches found that need to be fixed in this script:
 * - If the player is too close to the grapple point they will clip into it.
 * - Grapple Zooming lets you zoom to multiple points if its done fast. It SHOULD lock on to one point to zoom to
 * - grappleBool doesn't function the way we want it to. The goal is for a bool to be flipped on update (via button press) then the 
 * function to be called in FixedUpdate. This was causing problems, will look into this once the rest of the code is cleaned up.
 * - Right now the crosshair isn't 1:1 with where the grapple shoots out but that might because the spawn of the grapple is in the 
 * character's head, not at their center?
 * - Somehow in the process of adding comments I broke grapple zooming... Oops? I know how to fix it but I'll do it after I'm done 
 * commenting here. Fix is: call the movementForce variable from playerController and modify it the same way we did for dashing.
 * Description: This script controls the grapple mechanic in the game. Any surfaces labeled "grappleable" can be grappled to.
 * You can also zoom straight to anything labeled "zoomable".
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrappleScriptEvenNewer : MonoBehaviour
{
    [Header("Layers")]

    public LayerMask whatIsGrappleable; //This layer is for any surface that the player can grapple on.
    public LayerMask whatIsZoomable; //This layer is for any surface that the player can zoom to.
    public LayerMask whatIsAble; //This is a mix of both of the previously mentioned layers. 

    [Header("Line Renderer")]

    private LineRenderer lr; //This is a reference to the line renderer used to draw the rope into the game when grappling.
    public GameObject lineRendererObject; //This game object was added because having a line renderer
                                          //attached to the player was causing problems for the position of the model.

    [Header("Crosshair")]

    public Vector3 aimDirection; //A representation of where the player is aiming their crosshair using the mouse.
    public Transform crosshair; //The transform component of the crosshair image on the screen.
    public SpriteRenderer crosshairSprite; //The sprite used to represent the crosshair.
    public Transform cameraTransform; //The transform component of the game's camera.

    [Header("Bools")]

    public bool isGrappling; //This bool dictates whether or not the player is currently grappling.
    //private bool grappleBool; //This bool was an attempt at getting the grapple function to work the same way as the dash script:
                                //flipping bools on update then calling the function itself on fixed update. Something went wrong 
                                //with it so we may need to work on this.
    private bool grappleZoomBool; //This bool dictates whether or not the player is currently zoomin' https://youtu.be/dfrlUNgaFLQ 
    private bool stopGrappleBool; //This bool is flipped in order to start the function that ends grappling.


    [Header("Grapple Rope")]

    public List<Vector3> ropePositions = new List<Vector3>(); //A list of Vector3 positions that collectively make up the grapple rope
    public float maxRopeLength = 20f; //The longest that the grapple rope can be
    public float currRopeLength; //The current length of the grapple rope
    private float maxDistance = 20f; //The longest distance you can shoot the grapple rope out to

    [Header("Outside Components")]

    public GameObject player; //A reference to the player's GameObject
    private Animator myAnimator; //A reference to the player's animator component
    private Rigidbody myRB; //A reference to the player's rigidbody component

    [Header("Grappling")]

    private Vector3 grapplePoint; //The end point of the player's grapple rope
    public Transform grappleSpawn; //The transform component where the start point of the grapple rope spawns
    public Vector3 grappleVect; //This is a value for testing purposes. It gives a Vector3 of the length of the current grapple rope
    private ConfigurableJoint joint; //This is the joint that connects the player to the grapple rope. A configurable joint was decided
                                     //to be the best fit as every other joint we tried ended in a fucking mess of spaghetti code.
                                     //The joint is configured ( ͡° ͜ʖ ͡°) to help simulate how a character would fling around on a rope.
    private SoftJointLimit tempLimit;


    // Start is called before the first frame update
    void Awake()
    {
        //Setting all of our bools to their default positions
        isGrappling = false;
        grappleZoomBool = false;
        stopGrappleBool = false;

        lr = lineRendererObject.GetComponent<LineRenderer>(); //setting lr to become a reference to the line renderer component held in
                                                              //our lineRendererObject
        myRB = GetComponent<Rigidbody>(); //setting myRB to reference the rigidbody component of our player
        myAnimator = GetComponent<Animator>(); //setting myAnimator to reference the animator component of our player
    }

    // Update is called once per frame
    void Update()
    {

        // The following section is to handle the position of your mouse relative to the game world
        // as well as where to place the crosshair for the grapple. Atan is used here to make sure the
        // crosshair rotates in a circle around the player sort of relative to where you move the mouse.
        // Right now it isn't 1:1 with where the grapple shoots out but that might because the spawn of
        // the grapple is in the character's head, not at their center. 
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
            player.transform.position.z - cameraTransform.position.z)); //This variable turns the user's mouse position into
            //a position that fits the world space more appropriately. It also takes into account how we don't want shit moving on
            //the z axis.
        var facingDirection = worldMousePosition - grappleSpawn.transform.position; //facingDirection is the position from 
                                                                                    //worldMousePosition but contrasted with the 
                                                                                    //position that the grapple rope spawns at.
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x); 
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }
        aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right; //This wizardry that we call "trigonometry" 
            //in the above 6 lines somehow turns that facingDirection into a fucking circle around the player for the purpose of
            //aiming! Getting a circle from triangle math? How the fuck does that work? https://youtu.be/8GyVx28R9-s?t=112

        //Debug.Log("aimAngle is: " + aimAngle);
        //Debug.Log("aimDirection is: " + aimDirection);

        grappleCheck(); //This function is run every frame to determine if something you're aiming at can be grappled to.

        if (Input.GetButtonDown("Fire1"))
        {
            //Once you press down the grapple button it begins the grapple function.
            //This also disables your crosshair.

            //Debug.Log("Left Click Down");

            StartGrapple();
            //grappleBool = true;
            crosshairSprite.enabled = false;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            //Letting go of the grapple button starts the function to end grappling.
            //This re-enables the crosshair.

            //Debug.Log("Left Click Up");

            stopGrappleBool = true;
            //grappleBool = false;
            crosshairSprite.enabled = true;
        }

        if (isGrappling)
        {
            SwingCheck(); //If the player is currently grappling then we run this function that handles all of the rope swinging
                          //mechanics.
        }

        lr.positionCount = ropePositions.Count + 1; //This sets the amount of positions that connect the grapple rope to the length
                                                    //of the ropePositions list every frame.

        SetCrosshairPosition(aimAngle); //This starts the function that determines the location of the crosshair every frame.

    }

    private void FixedUpdate()
    {
        //Ideally when using physics based movement, you want all of your button presses to be registered during the regular
        //Update function, BUT you want all functions that involve physics to run in FixedUpdate. This is what we will be attempting
        //with any and all physics based functions. The way we achieve this is by flipping a bool on button press in update, then the 
        //bool dictates whether or not the function runs on FixedUpdate. This should minimize crazy fucked up bullshit happening with 
        //the physics. I hope...

        /*if (grappleZoomBool == true)
        {
            StartGrappleZoom(); //This function is for grapple zoomin!
        }*/

        //if (grappleBool == true)
        //{
           // StartGrapple();
        //}

        if (stopGrappleBool == true)
        {
            StopGrapple(); //If there was a war on grappling, this function would be the fuckin DEA.
        }
    }

    private void LateUpdate()
    {
        DrawRope(); //The grapple rope is drawn on LateUpdate because for some reason if it isn't it has a chance of coming out a 
                    //few frames late? I'm sorry I'm too much of a hack fraud to understand my own code but this was a fix suggested
                    //and in the words of our lord and savior, Todd Howard, "it just works".
    }

    void StartGrapple() //This function starts the grapple process, it also determines whether the player wants to grapple or zoom.
    {
        //Debug.Log("Start Grapple");

        myAnimator.SetBool("grappling", isGrappling); //This will be used to tell the animator to play a grappling animation...
                                                      //if we had one. cri.
        RaycastHit hit; //We are declaring a RayCastHit type variable here that we're just gonna call hit. Unity's TOTALLY DESCRIPTIVE
                        //AND NOT AWFUL documentation refers to a RayCastHit as a "Structure used to get information back from a 
                        //raycast".

        Vector3 grappleDir =  crosshair.transform.position - grappleSpawn.transform.position; //A Vector3 representing the direction
                                                                                              //the grapple will shoot out to

        if (Physics.Raycast(grappleSpawn.transform.position, grappleDir, out hit, maxDistance, whatIsGrappleable))
        {
            


            DoGrapple(hit); //If our raycast hits something grappleable then we store it as the value hit. Our raycast starts from the
                            //grappleSpawn location in the direction of grappleDir. The raycast then goes for the length of whatever 
                            //we set the value maxDistance to. This function we call handles actually making a grapple.

            //Debug.Log("Ray hit Grapple... motherfucker");
        }
        else if (Physics.Raycast(grappleSpawn.transform.position, grappleDir, out hit, maxDistance, whatIsZoomable))
        {
            //If our raycast instead hits something zoomable, then we call the StartGrappleZoom function

            StartGrappleZoom(hit);

            //Debug.Log("Ray hit Grapplezoom... bitch");
        }

    }

    void StopGrapple() //This function stops grappling in its tracks. All grapples get killed dead or your money back.
    {
        
        lr.positionCount = 0; //Setting the amount of positions on the line renderer to 0 essentially deletes any line it has rendered.
        Destroy(joint); //Destroy the joint that holds the player to the grappleable surface
        isGrappling = false; //This bool determines two things: 1.) Whether or not the grapple animation is playing and 2.) Whether or
                             //not the SwingCheck() function that handles all swinging mechanics is being called.
        myAnimator.SetBool("grappling", isGrappling); //We run this to make sure the animator knows the animation should end.
        ropePositions.Clear(); //We clear the list of rope positions we created so that it won't be full of stuff next time we need it.
        currRopeLength = maxRopeLength; //Reset the currRopeLength value to the maximum for the next time we need it.
        stopGrappleBool = false; //Flip this bool to show that the stopGrapple has uh... stopped.

        //Debug.Log("Stop Grapple");
    }

    void DoGrapple(RaycastHit hit) //The function that actually does all the grapple shit
    {
                                                /* Fixing out Z Values */

        if (hit.point.z != player.transform.position.z - cameraTransform.position.z)
        {
            //Our character never moves on the z axis but all of our calculations still factor in the z axis given that this is Unity 
            //3D. Because of this we use this function to make sure every single component of the player and the grappling hook stay
            //at the same z value.

            Vector3 tempVector = new Vector3(hit.point.x, hit.point.y, player.transform.position.z);
            hit.point = tempVector;
        }


                                                /* Setting Values for later Calculations */

        grapplePoint = hit.point; //We assign the point in 3D space of our raycast hit to be the point that our grapple rope ends.
        grappleVect = grapplePoint - transform.position; //We use this value for testing purposes. It tells us the location in 3D
                                                         //space between our grappling hook's end point and the player character.

        ropePositions.Add(grapplePoint); //We add the end of our grapple rope to a list called ropePositions. We will use this list to
                                         //determine the points along our rope for line renderer and wrapping/unwrapping mechanics.

        float distanceFromPoint = Vector3.Distance(grappleSpawn.position, grapplePoint); //The float value of distance between the
                                                                                         

                                                /* Configuring our Configurable joint */

        joint = player.AddComponent<ConfigurableJoint>(); //We add a configurable joint component to the player.
        joint.autoConfigureConnectedAnchor = false; //We want the autoConfigure value of them joint to be turned off. We want to do
                                                    //that part ourselves goddammit!
        joint.xMotion = ConfigurableJointMotion.Limited; //We want to be able to set limits on exactly how the joint can move
        joint.yMotion = ConfigurableJointMotion.Limited; 
        joint.zMotion = ConfigurableJointMotion.Limited; 
        joint.connectedAnchor = grapplePoint; //The anchor that connects the player and the joint is the end of our grapple rope.


                                                /* Calculations! */

        if (distanceFromPoint > maxRopeLength && distanceFromPoint < 15f) 
        {
            //We do this calculation if the player is grappling a distance longer than the max length the rope can stretch,
            //but still long enough for the game to register they are grappling.

            isGrappling = true; //let the world know... that we grapplin' tonight boiz

            //setting the linear limit:
            tempLimit = joint.linearLimit; //First we get a copy of the limit we want to change
            tempLimit.limit = maxRopeLength; //set the value that we want to change
            currRopeLength = maxRopeLength; //Set our current rope length to its maximum size
            joint.linearLimit = tempLimit; //set the joint's limit to our edited version.

        }
        else if (distanceFromPoint > 15f)
        {
            //We do this calculation if the player is grappling a distance too long for the game to register it has grappled.

            StopGrapple(); //This function ends world hunger... No actually it fucking stops the grapple genius.
        }
        else
        {
            isGrappling = true; //let the world know... that we grapplin' tonight boiz

            //setting the linear limit:
            tempLimit = joint.linearLimit; //First we get a copy of the limit we want to change
            tempLimit.limit = distanceFromPoint; //set the value that we want to change
            joint.linearLimit = tempLimit; //set the joint's limit to our edited version.
        }

        //Debug.Log("Ray Hit BAYBEEEEEEEEE");
    }

    void StartGrappleZoom(RaycastHit zoomHit) //This function grapple zooms! This zooms the player to the thing they hit with their grapple hook!
    {

        if (zoomHit.point.z != player.transform.position.z - cameraTransform.position.z)
        {
            //Our character never moves on the z axis but all of our calculations still factor in the z axis given that this is Unity 
            //3D. Because of this we use this function to make sure every single component of the player and the grappling hook stay
            //at the same z value.

            //isGrappling = true; //Do we want the grappling animation to play while zooming? Plus it'll run the swinging calculations
            Vector3 tempVector = new Vector3(zoomHit.point.x, zoomHit.point.y, player.transform.position.z);
            zoomHit.point = tempVector;
        }

        grapplePoint = zoomHit.point; //The point we want to zoom toward.
        Vector3 zoomDirection = grapplePoint - myRB.transform.position; //The direction we want to zoom
        myRB.velocity = zoomDirection * 5; //The zoomin itself

        //Debug.Log("Start Grapple Zoom is all according to Keikaku. TL note: Keikaku means plan");

    }

    void StopGrappleZoom() //The function what which stops the zoomin 
    {
        //Add code here to stop the player's momentum

        //isGrappling = false;
    }

    void DrawRope() //This function draws the grapple rope as a physical line.
    {

        if (!joint) return; //This prevents the line of the grapple from being rendered when not grappling

        for (int i = 0; i < ropePositions.Count; i++)
        {
            //Filling the positions of the line renderer with each element of our ropePositions list
            lr.SetPosition(i, ropePositions[i]);
        }

        lr.SetPosition(ropePositions.Count, grappleSpawn.position); //Filling the last entry of the line renderer's positions with the
                                                                    //spawn location of the grapple rope.

        //Debug.Log("Fuuuuug I'm shooting (drawing) Ropes dawg");
    }

    private void SetCrosshairPosition(float aimAngle) //This function determines the position of the crosshair on screen
    {
        //The two following lines create a distance from the grapple spawn along a circle that the crosshair travels:
        var x = grappleSpawn.transform.position.x + 2f * Mathf.Cos(aimAngle); 
        var y = grappleSpawn.transform.position.y + 2f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, player.transform.position.z); //The value we store the position of the crosshair
        crosshair.transform.position = crossHairPosition; //Actually setting the crosshair to the value we just determined above
    }


    /*-------------------------------------------------------------------------------------------------------------------------
     * SWING CHECK EXPLANATION
     *  An Incoherent Ramble, 
     *   by Stpehen Anderson
     * 
     * So I figured since this section is by far the most complicated bit for the grappling mechanics that I would write out
     * an in depth explanation for what it's doing.
     * 
     * PROBLEM: Using one of Unity's built in ropes for our grapple ends terribly.
     * 
     * OUR SOLUTION: An over-engineered "rope-under-tension" simulation using raycasts!
     * 
     * RESULT: It F*&%ING WORKED!
     * 
     * EXPLANATION: Olaf swings by placing down a "configurable joint," which basically acts like a hinge,
     *              wherever the player clicks their mouse to grapple. The key to getting the grapple to
     *              wrap around objects and terrain is changing where the joints "anchor point" is located.
     *              This anchor point determines what point Olaf's rope connects to, and is the point that
     *              Olaf rotates around as he swings. 
     *              
     *              Olaf continuously fires raycasts from himself to the anchor point to see if there are
     *              any obstructions that the rope should hit. When the raycast hits something between Olaf
     *              and the anchor point, it moves the anchor point to be the point that the raycast hit.
     *              All of the anchor points that are assigned this way are tracked in a stack so they can
     *              be removed in the proper order to unwrap.
     *               
     *              How do we tell when the rope should unwrap from an object? Well, we shoot an extra ray-
     *              cast at the second to last anchor point in the rope's anchor point stack. If both of 
     *              the raycasts hit their target successfully, we know that there is no longer an object
     *              or other kind of obstruction between Olaf and the previous anchor point.
     *              
     * Symbols:
     * O = Olaf, + = joint anchor, (+) = prior joint anchor, 
     * -> = movement, line = rope, ## = object with collision, 
     * R> = Raycast hit detected here
     *........................WRAPPING........................                     
     *:          STEP 1          :           STEP 2          :  Step 1 & 2: Olaf is swinging and his rope approaches an      
     *:           +              :            +              :              obstacle. The Raycast fires from Olaf to the                   
     *:          /               :            |              :              joint anchor every frame. When the raycast       
     *:         /  ##            :            | ##           :              hits something between Olaf and the anchor, 
     *:        /                 :            |              :              that signifies our rope "snagging" on an        
     *:       O->                :            O->            :              object or terrain.           
     *:                          :                           :                                           
     *:.......................................................                                           
     *:          STEP 3          :           STEP 4          :  Step 3:     The Raycast has hit an object between Olaf                                                
     *:           +              :           (+)             :              and the joint anchor so our rope should start                                
     *:           |              :            |              :              wrapping around the obstacle. 
     *:         R>|##            :            +##            :                                                   
     *:           |              :             \             :  Step 4:     We move the anchor point to where the Raycast                                             
     *:           |              :              \            :              hit was, allowing Olaf to swing around this                                      
     *:           O->            :               O->         :              new point.                                  
     *.......................................................: 
     *
     *.......................UNWRAPPING.......................                     
     *:          STEP 1          :           STEP 2          :  Step 1:     Olaf is swinging from a shorter rope segement           
     *:          (+)             :           (+)             :              as a result of wrapping. He fires a second
     *:           |              :            |              :              raycast towards (+), the second to last anchor 
     *:           +##            :            +##            :              point, but they are blocked. 
     *:            \             :            |              :                      
     *:             \            :            |              :  Step 2:     Olaf is now in direct line of sight of both
     *:            <-O           :          <-O              :              anchor points.
     *:.......................................................                                           
     *:          STEP 3          :           STEP 4          :  Step 3:     The raycasts now hit both the last and second                                               
     *:        R>(+)             :            +              :              to last anchor points, triggering an unwrap.
     *:           |              :           /               :               
     *:           +##            :          /  ##            :                                                   
     *:           |              :         /                 :  Step 4:     The last anchor point is deleted and replaced                                            
     *:           |              :        /                  :              by the second to last anchor point. 
     *:         <-O              :     <-O                   :                                               
     *.......................................................:
     */
    void SwingCheck() //This function handles the rope swinging mechanics for wrapping and unwrapping the grapple rope. 
    {
        RaycastHit hitMeBabyOneMoreTime; //We didn't want to just name this one 'hit' also. Stephen let me name it, his mistake.
                                         //Specifically, this Raycast is used to help us wrap our rope around 
        RaycastHit grappleUnwind; //Another RaycastHit we will use in the calculations for unwrapping the grapple rope
        Vector3 dir = new Vector3(ropePositions.Last().x - grappleSpawn.position.x, 
                                  ropePositions.Last().y - grappleSpawn.position.y); //Creates a direction to shoot out our wrapping 
                                                                                     //Raycast from the end of our rope to where the 
                                                                                     //rope spawns.

        Physics.Raycast(grappleSpawn.position, dir, out hitMeBabyOneMoreTime, maxRopeLength, whatIsGrappleable); //Shoot out our wrapping Raycast
        //Debug.DrawRay(grappleSpawn.position, dir, Color.red, 1);

        float anchorDifference = Vector3.Distance(hitMeBabyOneMoreTime.point, ropePositions.Last()); //The distance between the end of
                                                                                           //our rope and the point of our RaycastHit.

        if (anchorDifference >= 0.01f && hitMeBabyOneMoreTime.point != Vector3.zero) // this filters out extremely small movements in where the raycast is hitting
        {
            //This whole section is for wrapping the rope around something. The logic behind this is that we shoot a Raycast out from
            //the grappleSpawn object in the same direction the rope shoots out from. If the raycast hits an object (which happens
            //each time the player's rope wraps around an object) then it creates a new temporary end of the rope. We still keep the
            //rope rendered in full so this gives the illusion of a rope wrapping around an object. 

            ropePositions.Add(hitMeBabyOneMoreTime.point); //Add the point of our RaycastHit to the list of grapple rope positions.

            float ropeSegLength = Vector3.Distance(hitMeBabyOneMoreTime.point, joint.connectedAnchor); //The length of the current 
                                                                //segment of rope that will be removed in this portion of code.

            joint.connectedAnchor = ropePositions.Last(); //Set the connected anchor of the configurable joint to the last element in 
                                                          //the ropePositions list.

            joint.axis = hitMeBabyOneMoreTime.normal; //We set the axis of our configurable joint to the normal value of our wrapping 
                                                      //RaycastHit. Idk why we did this, Stephen tell me what this shit does
                                                      //Stephen answer: When we change where the joint is at, 

            currRopeLength -= ropeSegLength; //Set the length of our rope to the distance between the wrap around RaycastHit and the 
                                             //anchor of our configurable joint.

            //set the linear limit
            tempLimit = joint.linearLimit; //First we get a copy of the limit we want to change
            tempLimit.limit = currRopeLength; //set the value that we want to change
            joint.linearLimit = tempLimit; //set the joint's limit to our edited version.

        }
        else if (ropePositions.Count > 1) //Y tho Stephen, y tho?
        {
            //This section here is for unwrapping the grapple rope back around objects. Boy was this shit hard. We thought we could 
            //just do what we did for wrapping around but you know... backwards? Shit doesn't work that way, we had to do some
            //slerpin' ( ͡° ͜ʖ ͡°) to get it to work. 

            dir = new Vector3(ropePositions[ropePositions.Count - 2].x - grappleSpawn.position.x, 
                              ropePositions[ropePositions.Count - 2].y - grappleSpawn.position.y); //give a direction to shoot out our
                                                                                                   //Unwind RaycastHit

            Physics.Raycast(grappleSpawn.position, dir, out grappleUnwind, whatIsGrappleable); //Shoot out our Unwind RaycastHit

            anchorDifference = Vector3.Distance(grappleUnwind.point, ropePositions[ropePositions.Count - 2]); //The distance between 
                                                                //the point of contact in our unwind RaycastHit and the third to last 
                                                                //element in our ropePositions. Why that element? I forget, ask Stephen

            if (anchorDifference <= 0.001f) 
            {
                float ropeSegLength = Vector3.Distance(grappleUnwind.point, joint.connectedAnchor); //The length of the current 
                                                                    //segment of rope that will be added back in this portion of code.

                joint.connectedAnchor = ropePositions[ropePositions.Count - 2]; //Set the connected anchor back to what it was before 
                                                                                //the rope wrapped around something.

                Vector3 slerpVal = Vector3.Slerp(joint.axis, grappleUnwind.normal, 1f); //slerpin' ( ͡° ͜ʖ ͡°) is pretty complicated and 
                                                                             //I kind of forget what it does every time. But if this
                                                                             //makes sense to you I know it is spherically
                                                                             //interpolates between two vectors. 

                joint.axis = grappleUnwind.normal; //We set the axis of our configurable joint to the normal value of our unwrapping 
                                                   //RaycastHit

                ropePositions.RemoveAt(ropePositions.Count - 1); //Get rid of the wrap around RaycastHit point that was added to the 
                                                                 //ropePositions list.

                currRopeLength += ropeSegLength; //Add back the segment of rope length from unwrapping

                //set the linear limit
                tempLimit = joint.linearLimit; //First we get a copy of the limit we want to change
                tempLimit.limit = currRopeLength; //set the value that we want to change
                joint.linearLimit = tempLimit; //set the joint's limit to our edited version.
            }

        }

    }

    void grappleCheck() //This function changes the color of the crosshair when a grappleable object is within your reach.
    {
        RaycastHit grappleCheckhit; //A RaycastHit for determining whether or not a grappleable object is within reach.
        Vector3 grappleDir = crosshair.transform.position - grappleSpawn.transform.position; //The direction to shoot the raycast out

        if (Physics.Raycast(grappleSpawn.transform.position, grappleDir, out grappleCheckhit, maxDistance, whatIsAble))
        {
            crosshairSprite.color = Color.black; //if there is something grappleable within reach then it turns black
        }
        else
        {
            crosshairSprite.color = Color.white; //if there isn't something grappleable within reach then it stays white
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //This is to make sure that once the player collides with anything then it interrupts their grapple zoom. 

        if (grappleZoomBool == true)
        {
            StopGrappleZoom();
        }
    }
}