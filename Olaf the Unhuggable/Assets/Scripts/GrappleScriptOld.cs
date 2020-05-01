using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScriptOld : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform grappleSpawn;
    public GameObject player;
    private SpringJoint joint;
    private float maxDistance = 10f;


    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Left Click Down");
            StartGrapple();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Left Click Up");
            StopGrapple();
        }

    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        Debug.Log("Start Grapple");
        RaycastHit hit;
        if (Physics.Raycast(grappleSpawn.position, transform.forward, out hit, maxDistance, whatIsGrappleable))
        {
            Debug.DrawRay(grappleSpawn.position, transform.forward * hit.distance, Color.yellow);
            grapplePoint = hit.point;
            joint = player.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            Debug.Log("Ray Hit");

        } else
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.yellow);
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
}
