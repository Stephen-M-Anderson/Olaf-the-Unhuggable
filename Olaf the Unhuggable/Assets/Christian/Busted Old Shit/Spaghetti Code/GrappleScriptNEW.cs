using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScriptNEW : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform grappleSpawn;
    public GameObject player;
    private Animator myAnimator;

    //This game object was added because having a line renderer attached 
    //to the player was causing problems for the position of the model.
    public GameObject lineRendererObject; 
    private SpringJoint joint;
    private float maxDistance = 100f;
    private Vector3 mousePosition;
    private Rigidbody myRB;
    public float grappleSpeed;

    //These are the variables for the crosshair shit that's new to this script
    public Transform crosshair;
    public SpriteRenderer crosshairSprite;
    public Transform cameraTransform;

    //These are variables for the Node method of grappling
    public GameObject lastNodePrefab;
    GameObject curLastNodePrefab;

    private bool isGrappling = false;

    // Start is called before the first frame update
    void Awake()
    {
        //lr = lineRendererObject.GetComponent<LineRenderer>();
        myRB = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        // mousePosition = Input.mousePosition;
        //mousePosition.z = 0f;

        // The following section is to handle the position of your mouse relative to the game world
        // as well as where to place the crosshair for the grapple. Atan is used here to make sure the
        // crosshair rotates in a circle around the player sort of relative to where you move the mouse.
        // Right now it isn't 1:1 with where the grapple shoots out but that might because the spawn of
        // the grapple is in the character's head, not at their center. 
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, player.transform.position.z - cameraTransform.position.z));
        var facingDirection = worldMousePosition - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }
        var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        if (Input.GetButtonDown("Fire1"))
        {
            // Once you press down the grapple button it begins the grapple function.
            // This also disables your crosshair.
            Debug.Log("Left Click Down");
            StartGrapple();
            crosshairSprite.enabled = false;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            // Letting go of the grapple button starts the function to end grappling.
            // This re-enables the crosshair.
            Debug.Log("Left Click Up");
            StopGrapple();
            crosshairSprite.enabled = true;
        }

        SetCrosshairPosition(aimAngle);

    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        Debug.Log("Start Grapple");
        isGrappling = true;
        myAnimator.SetBool("grappling", isGrappling);
        RaycastHit hit;
        Ray tempRay;
        tempRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(grappleSpawn.position, mousePosition, out hit, maxDistance, whatIsGrappleable))

        if (Physics.Raycast(tempRay, out hit, maxDistance, whatIsGrappleable)) 
        {
            if (hit.point.z != player.transform.position.z - cameraTransform.position.z)
            {
                // This modifies the z value of where our grapple is shot out to be consistent with
                // where our character is standing in 3D space.
                Vector3 tempVector = new Vector3(hit.point.x, hit.point.y, player.transform.position.z);
                hit.point = tempVector;
            }
            grapplePoint = hit.point;

            //float distanceFromPoint = Vector3.Distance(grappleSpawn.position, grapplePoint);

            // Okay so this big fucking chunk of code right here? This shit is the OLD SHIT and we want
            // the NEW SHIT. What this used to do was add a Spring Joint to our character, then connect
            // that joint from the player to the point that we have grappled onto. The purpose of this
            // NEW script is to change this to instead instantiate several small prefabs as parts of a 
            // rope and then have those all connected together with some kind of joint. These would all
            // connect starting from the grapple spawn and then going out until the grapple point.

            /*
            joint = player.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(grappleSpawn.position, grapplePoint);

            //joint.maxDistance = distanceFromPoint * 0.8f;
            //joint.minDistance = distanceFromPoint * 0.25f;

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 6f;
            joint.damper = 10f;
            joint.massScale = 2.5f;

            //I wanted to give the player a small burst of speed when they make a grapple but it really doesn't seem to be working out
            //myRB.velocity = new Vector3(grappleSpeed, myRB.velocity.y, 0);
            */

            //The new code that should do all that shit is gonna go here:

            /* PsuedoCode Start! */
            /* Instantiate the Last Node Prefab into the grapplePoint position;
             * 
             * Create a Script to attach to the Last Node Prefab that instantiates more nodes;
             * 
             * Have the Nodes instantiate UNTIL the distance between player and the Last Node Prefab 
             * is smaller than the arbitrary distance;
             * 
             * Once it works on the first try celebrate with some wings at Femboy Hooters;
             */

            //Instantiate(lastNodePrefab, grapplePoint, Quaternion.identity);

            curLastNodePrefab = (GameObject)Instantiate(lastNodePrefab, grappleSpawn.transform.position, Quaternion.identity);
            curLastNodePrefab.GetComponent<NodeConnectionScript>().grapplePoint = grapplePoint;

            // I'm gonna be real I kinda forgot what the fuck this does. I just remember without this 
            // the grapple stuff got fucked and then putting it here unfucked it.
            //lr.positionCount = 2;
            Debug.Log("Ray Hit");

        }
        else
        {
            Debug.Log("Ray didn't hit");
        }

    }

    void StopGrapple()
    {
        //lr.positionCount = 0;
        // Destroy(joint); // This MIGHT be defunct when the new grapple system is added? iunno
        isGrappling = false;
        myAnimator.SetBool("grappling", isGrappling);
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Nodes");
        GameObject[] gameObjects2 = GameObject.FindGameObjectsWithTag("LastNodes");
        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }

        for (var i = 0; i < gameObjects2.Length; i++)
        {
            Destroy(gameObjects2[i]);
        }

        Debug.Log("Stop Grapple");
    }

    void DrawRope()
    {
        // This whole function is designed to draw the line that goes from the player to the 
        // grapple point BUT that shit won't be needed with the new system so I might have to
        // comment this WHOLE bitch out baybeeeeeeeeeee.

        //This prevents the line of the grapple from being rendered when not grappling
        if (!joint) return;

        lr.SetPosition(0, grappleSpawn.position);
        lr.SetPosition(1, grapplePoint);

        Debug.Log("Rope Drawn");
    }

    private void SetCrosshairPosition(float aimAngle)
    {
        /*if (!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }*/

        var x = transform.position.x + 3f * Mathf.Cos(aimAngle);
        var y = transform.position.y + 3f * Mathf.Sin(aimAngle);

        //var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, player.transform.position.z - cameraTransform.position.z));

        //var crossHairPosition = worldMousePosition;

        var crossHairPosition = new Vector3(x, y, player.transform.position.z);
        crosshair.transform.position = crossHairPosition;
    }
}
