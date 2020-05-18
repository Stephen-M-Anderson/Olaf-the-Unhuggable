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
    //This game object was added because having a line renderer attached 
    //to the player was causing problems for the position of the model.
    public GameObject lineRendererObject; 
    private SpringJoint joint;
    private float maxDistance = 100f;
    private Vector3 mousePosition;
    private Rigidbody myRB;
    public float grappleSpeed;

    public Transform crosshair;
    public SpriteRenderer crosshairSprite;
    public Transform cameraTransform;


    // Start is called before the first frame update
    void Awake()
    {
        lr = lineRendererObject.GetComponent<LineRenderer>();
        myRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        // mousePosition = Input.mousePosition;
        //mousePosition.z = 0f;

        var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, player.transform.position.z - cameraTransform.position.z));

        var facingDirection = worldMousePosition - transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        // 4
        var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Left Click Down");
            StartGrapple();
            crosshairSprite.enabled = false;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
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
        RaycastHit hit;
        Ray tempRay;
        tempRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(grappleSpawn.position, mousePosition, out hit, maxDistance, whatIsGrappleable))

        if (Physics.Raycast(tempRay, out hit, maxDistance, whatIsGrappleable))
        {
            if (hit.point.z != player.transform.position.z - cameraTransform.position.z)
            {
                Vector3 tempVector = new Vector3(hit.point.x, hit.point.y, player.transform.position.z);
                hit.point = tempVector;
            }
            grapplePoint = hit.point;
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

            lr.positionCount = 2;
            Debug.Log("Ray Hit");

        }
        else
        {

            Debug.Log("Ray didn't hit");
        }

    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
        Debug.Log("Stop Grapple");
    }

    void DrawRope()
    {
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
