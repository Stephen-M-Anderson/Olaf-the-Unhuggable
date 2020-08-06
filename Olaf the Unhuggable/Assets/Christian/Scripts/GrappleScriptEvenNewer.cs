/* 
 
Known glitches found that need to be fixed in this script:
- If the player is too close to the grapple point they will clip into it.
 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrappleScriptEvenNewer : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public LayerMask whatIsZoomable;
    public LayerMask whatIsAble;
    public Transform grappleSpawn;
    public GameObject player;
    private Animator myAnimator;

    //This game object was added because having a line renderer attached 
    //to the player was causing problems for the position of the model.
    public GameObject lineRendererObject;
    private ConfigurableJoint joint;
    private float maxDistance = 15f;
    private Vector3 mousePosition;
    private Rigidbody myRB;
    public float grappleSpeed;

    //These are the variables for the crosshair shit that's new to this script
    public Transform crosshair;
    public SpriteRenderer crosshairSprite;
    public Transform cameraTransform;

    //These are variables for the Node method of grappling
    public GameObject hookPrefab;
    GameObject currHookPrefab;

    public bool isGrappling = false;
    public List<Vector3> ropePositions = new List<Vector3>();
    private bool distanceSet;
    public float maxRopeLength = 6f;
    public float currRopeLength;
    public float ropeMaxCastDistance = 9f;

    public Vector3 aimDirection;

    private bool grappleZoomBool;
    private bool grappleBool;
    private bool stopGrappleBool;

    public Vector3 grappleVect;

    // Start is called before the first frame update
    void Awake()
    {
        lr = lineRendererObject.GetComponent<LineRenderer>();
        myRB = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        // The following section is to handle the position of your mouse relative to the game world
        // as well as where to place the crosshair for the grapple. Atan is used here to make sure the
        // crosshair rotates in a circle around the player sort of relative to where you move the mouse.
        // Right now it isn't 1:1 with where the grapple shoots out but that might because the spawn of
        // the grapple is in the character's head, not at their center. 
        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, player.transform.position.z - cameraTransform.position.z));
        var facingDirection = worldMousePosition - grappleSpawn.transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }
        aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        //Debug.Log("aimAngle is: " + aimAngle);
       // Debug.Log("aimDirection is: " + aimDirection);

        grappleCheck();

        if (Input.GetButtonDown("Fire1"))
        {
            // Once you press down the grapple button it begins the grapple function.
            // This also disables your crosshair.
            //Debug.Log("Left Click Down");
            StartGrapple();
            //grappleBool = true;
            crosshairSprite.enabled = false;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            // Letting go of the grapple button starts the function to end grappling.
            // This re-enables the crosshair.
            //Debug.Log("Left Click Up");
            stopGrappleBool = true;
            //grappleBool = false;
            //StopGrapple();
            crosshairSprite.enabled = true;
        }

        if (Input.GetKeyDown("q"))
        {
            // Once you press down the grapple button it begins the grapple function.
            // This also disables your crosshair.
            //Debug.Log("Q Click Down");
            //StartGrappleZoom(aimDirection);
            grappleZoomBool = true;
            crosshairSprite.enabled = false;
        }
        else if (Input.GetKeyUp("q"))
        {
            // Letting go of the grapple button starts the function to end grappling.
            // This re-enables the crosshair.
           // Debug.Log("Q Click Up");
            grappleZoomBool = false;
            StopGrappleZoom();
            crosshairSprite.enabled = true;
        }

        if (isGrappling)
        {
            SwingCheck();
        }

        lr.positionCount = ropePositions.Count + 1;

        SetCrosshairPosition(aimAngle);

    }

    private void FixedUpdate()
    {
        if (grappleZoomBool == true)
        {
            StartGrappleZoom();
        }

        if (grappleBool == true)
        {
            StartGrapple();
        }

        if (stopGrappleBool == true)
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        //Debug.Log("Start Grapple");
        myAnimator.SetBool("grappling", isGrappling);
        RaycastHit hit;
        //Ray tempRay;
        //tempRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(grappleSpawn.position, mousePosition, out hit, maxDistance, whatIsGrappleable))
        //if (Physics.Raycast(tempRay, out hit, maxDistance, whatIsGrappleable))


        Vector3 grappleDir =  crosshair.transform.position - grappleSpawn.transform.position;

        if (Physics.Raycast(grappleSpawn.transform.position, grappleDir, out hit, maxDistance, whatIsGrappleable))
        {
            DoGrapple(hit);
        } 
        else if (Physics.Raycast(grappleSpawn.transform.position, grappleDir, out hit, maxDistance, whatIsZoomable))
        {
            grappleZoomBool = true;
            crosshairSprite.enabled = false;

            //Debug.Log("Ray didn't hit");
        }

    }

    void StopGrapple()
    {
        stopGrappleBool = false;
        lr.positionCount = 0;
        Destroy(joint); 
        isGrappling = false;
        myAnimator.SetBool("grappling", isGrappling);
        ropePositions.Clear();
        currRopeLength = maxRopeLength;

        //Debug.Log("Stop Grapple");
    }

    void DoGrapple(RaycastHit hit)
    {
        if (hit.point.z != player.transform.position.z - cameraTransform.position.z)
        {
            // This modifies the z value of where our grapple is shot out to be consistent with
            // where our character is standing in 3D space.
            Vector3 tempVector = new Vector3(hit.point.x, hit.point.y, player.transform.position.z);
            hit.point = tempVector;
        }
        grapplePoint = hit.point;
        grappleVect = grapplePoint - transform.position;

        ropePositions.Add(grapplePoint);

        //currHookPrefab = (GameObject)Instantiate(hookPrefab, grappleSpawn.transform.position, Quaternion.identity);
        //currHookPrefab.GetComponent<NodeConnectionScript>().grapplePoint = grapplePoint;


        joint = player.AddComponent<ConfigurableJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        //joint.connectedBody = grappleable surface;


        //
        joint.connectedAnchor = grapplePoint;
        GetComponent<playerController>().ropeHook = grapplePoint;
        //

        //joint.enableCollision = true;

        float distanceFromPoint = Vector3.Distance(grappleSpawn.position, grapplePoint);
        //float distanceFromPlayer = Vector3.Distance(transform.position, grapplePoint);
        //float halfDistanceFromPoint = distanceFromPoint / 2;


        //set the anchor of the joint
        //do we really need an anchor though? I'm not too sure. I feel like as long as we have the connected anchor
        //we should be fine... I guess we'll see if it feels too unnatural to go a little bit past the grappleable
        //surface while grappling. If this doesn't work we can also find other ways to restrict how far the player
        //can grapple.
        //joint.anchor = grapplePoint - grappleSpawn.position;


        // change that fuckin 9 later dawg its unsightly
        if (distanceFromPoint > maxRopeLength && distanceFromPoint < 15f)
        {
            isGrappling = true;

            //set the linear limit
            SoftJointLimit limit = joint.linearLimit; //First we get a copy of the limit we want to change
            limit.limit = maxRopeLength; //set the value that we want to change
            currRopeLength = maxRopeLength;
            joint.linearLimit = limit; //set the joint's limit to our edited version.

        }
        else if (distanceFromPoint > 15f)

        {
            StopGrapple();
        }
        else
        {
            isGrappling = true;

            //set the linear limit
            SoftJointLimit limit = joint.linearLimit; //First we get a copy of the limit we want to change
            limit.limit = distanceFromPoint; //set the value that we want to change
            joint.linearLimit = limit; //set the joint's limit to our edited version.
        }

        //I remember what this does now. This sets the number of points on the line renderer's line. I might
        //need to adjust this because the position count will increase upon collision.
        //lr.positionCount = 2;

        //Debug.Log("Ray Hit");
    }

    void StartGrappleZoom()
    {
        //Debug.Log("Start Grapple");
        myAnimator.SetBool("grappling", isGrappling);
        RaycastHit hit;
        //Ray tempRay;
        //tempRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(grappleSpawn.position, mousePosition, out hit, maxDistance, whatIsGrappleable))
        //if (Physics.Raycast(tempRay, out hit, maxDistance, whatIsGrappleable))


        Vector3 grappleDir = crosshair.transform.position - grappleSpawn.transform.position;

        if (Physics.Raycast(grappleSpawn.transform.position, grappleDir, out hit, maxDistance, whatIsZoomable))
        {
            if (hit.point.z != player.transform.position.z - cameraTransform.position.z)
            {
                //isGrappling = true;
                // This modifies the z value of where our grapple is shot out to be consistent with
                // where our character is standing in 3D space.
                Vector3 tempVector = new Vector3(hit.point.x, hit.point.y, player.transform.position.z);
                hit.point = tempVector;
            }
            grapplePoint = hit.point;

            Vector3 zoomDirection = grapplePoint - myRB.transform.position;

            myRB.velocity = zoomDirection * 5;

           // Debug.Log("Ray Hit");

        }
        else
        {
            //Debug.Log("Ray didn't hit");
        }
    }

    void StopGrappleZoom()
    {
        grappleZoomBool = false;
        //isGrappling = false;
    }

    void DrawRope()
    {

        //This prevents the line of the grapple from being rendered when not grappling
        if (!joint) return;

        //lr.SetPosition(0, grappleSpawn.position);
        //lr.SetPosition(1, grapplePoint);

        for(int i = 0; i < ropePositions.Count; i++)
        {
            lr.SetPosition(i, ropePositions[i]);
        }

        lr.SetPosition(ropePositions.Count, grappleSpawn.position);

        //Debug.Log("Rope Drawn");
    }

    private void SetCrosshairPosition(float aimAngle)
    {

        var x = grappleSpawn.transform.position.x + 2f * Mathf.Cos(aimAngle);
        var y = grappleSpawn.transform.position.y + 2f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, player.transform.position.z);
        crosshair.transform.position = crossHairPosition;
    }

    void SwingCheck()
    {
        RaycastHit hitMeBabyOneMoreTime;
        RaycastHit grappleUnwind;
        Vector3 dir = new Vector3(ropePositions.Last().x - grappleSpawn.position.x, ropePositions.Last().y - grappleSpawn.position.y);

        Physics.Raycast(grappleSpawn.position, dir, out hitMeBabyOneMoreTime, whatIsGrappleable);
       // Debug.DrawRay(grappleSpawn.position, dir, Color.red, 1);

        float anchorDifference = Vector3.Distance(hitMeBabyOneMoreTime.point, ropePositions.Last());

        //if (hitMeBabyOneMoreTime.point.x != ropePositions.Last().x || hitMeBabyOneMoreTime.point.y != ropePositions.Last().y)
        if (anchorDifference >= 0.01f)
        {
            ropePositions.Add(hitMeBabyOneMoreTime.point);

            float ropeSegLength = Vector3.Distance(hitMeBabyOneMoreTime.point, joint.connectedAnchor);

            float angleDiff = Vector3.Angle(joint.axis, hitMeBabyOneMoreTime.normal);

            joint.connectedAnchor = ropePositions.Last();

            joint.axis = hitMeBabyOneMoreTime.normal;

            currRopeLength -= ropeSegLength;

            //set the linear limit
            SoftJointLimit limit = joint.linearLimit; //First we get a copy of the limit we want to change
            limit.limit = currRopeLength; //set the value that we want to change
            joint.linearLimit = limit; //set the joint's limit to our edited version.


        }
        else if (ropePositions.Count > 1)
        {
            dir = new Vector3(ropePositions[ropePositions.Count - 2].x - grappleSpawn.position.x, ropePositions[ropePositions.Count - 2].y - grappleSpawn.position.y);

            Physics.Raycast(grappleSpawn.position, dir, out grappleUnwind, whatIsGrappleable);

            anchorDifference = Vector3.Distance(grappleUnwind.point, ropePositions[ropePositions.Count - 2]);

            //if (grappleUnwind.point.x == ropePositions[ropePositions.Count - 2].x && grappleUnwind.point.y == ropePositions[ropePositions.Count - 2].y)
            if (anchorDifference <= 0.001f)
            {
                float ropeSegLength = Vector3.Distance(grappleUnwind.point, joint.connectedAnchor);

                joint.connectedAnchor = ropePositions[ropePositions.Count - 2];

                Vector3 slerpVal = Vector3.Slerp(joint.axis, grappleUnwind.normal, 1f);

                joint.axis = grappleUnwind.normal;

                ropePositions.RemoveAt(ropePositions.Count - 1);

                currRopeLength += ropeSegLength;

                //set the linear limit
                SoftJointLimit limit = joint.linearLimit; //First we get a copy of the limit we want to change
                limit.limit = currRopeLength; //set the value that we want to change
                joint.linearLimit = limit; //set the joint's limit to our edited version.
            }


        }


    }

    void grappleCheck()
    {
        /* The purpose of this function is that if something is grappleable within your reach then the crosshair changes color */


        RaycastHit hit;
        Vector3 grappleDir = crosshair.transform.position - grappleSpawn.transform.position;

        if (Physics.Raycast(grappleSpawn.transform.position, grappleDir, out hit, maxDistance, whatIsAble))
        {
            crosshairSprite.color = Color.black;
        }
        else
        {
            crosshairSprite.color = Color.white;
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (grappleZoomBool == true)
        {
            StopGrappleZoom();
        }

    }
}
