using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeConnectionScript : MonoBehaviour
{

    public float distance = 2;
    public float speed = 1;

    bool done = false;

    public Vector3 grapplePoint;

    public GameObject player;
    public GameObject nodePrefab;
    public GameObject lastNode;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        lastNode = transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, grapplePoint, speed);

        if ((Vector3)transform.position != grapplePoint)
        {

            if (Vector3.Distance(player.transform.position, lastNode.transform.position) > distance)
            {


                CreateNode();

            }


        }
        else if (done == false)
        {

            done = true;


            lastNode.GetComponent<HingeJoint>().connectedBody = player.GetComponent<Rigidbody>();
        }


    }

    /* PsuedoCode Start! */
    /* 
     * 
     * Instantiate more nodes;
     * 
     * Have the Nodes instantiate UNTIL the distance between player and the Last Node Prefab 
     * is smaller than the arbitrary distance;
     * 
     * Once it works on the first try celebrate with some wings at Femboy Hooters;
     */

    void CreateNode()
    {

        Vector3 pos2Create = player.transform.position - lastNode.transform.position;
        pos2Create.Normalize();
        pos2Create *= distance;
        pos2Create += (Vector3)lastNode.transform.position;

        GameObject go = (GameObject)Instantiate(nodePrefab, pos2Create, Quaternion.identity);


        go.transform.SetParent(transform);

        lastNode.GetComponent<HingeJoint>().connectedBody = go.GetComponent<Rigidbody>();

        lastNode = go;


    }

}
