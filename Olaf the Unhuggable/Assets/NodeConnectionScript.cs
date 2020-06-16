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

    public List<GameObject> Nodes = new List<GameObject>();

    int vertexCount = 2;

    public LineRenderer lr;
    public GameObject grappleSpawn;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        grappleSpawn = GameObject.FindGameObjectWithTag("Grapple Spawn");

        lastNode = transform.gameObject;

        Nodes.Add(transform.gameObject);

        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, grapplePoint, speed);

        if ((Vector3)transform.position != grapplePoint)
        {

            if (Vector3.Distance(grappleSpawn.transform.position, lastNode.transform.position) > distance && done == false)
            {


                CreateNode();

                Debug.Log("Create Node");

            }


        }
        else if (done == false)
        {

            done = true;

            while (Vector3.Distance(grappleSpawn.transform.position, lastNode.transform.position) > distance)
            {
                CreateNode();
                Debug.Log("Too Fast Create Node");
            }

            lastNode.GetComponent<HingeJoint>().connectedBody = player.GetComponent<Rigidbody>();
        }

        //RenderLine();

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

        Vector3 pos2Create = grappleSpawn.transform.position - lastNode.transform.position;
        pos2Create.Normalize();
        pos2Create *= distance;
        pos2Create += (Vector3)lastNode.transform.position;

        GameObject go = (GameObject)Instantiate(nodePrefab, pos2Create, Quaternion.identity);


        go.transform.SetParent(transform);

        lastNode.GetComponent<HingeJoint>().connectedBody = go.GetComponent<Rigidbody>();

        //lastNode.GetComponent<SpringJoint>().connectedBody = go.GetComponent<Rigidbody>();
        // LOL I tried to fucking do this same code with spring joints to see if it would help
        // make it easier to move around since the original grapple script used a single spring
        // joint and it was fucking MADNESS. Nodes EVERYWHERE falling from the sky, cats and dogs
        // getting along, MASS HYSTERIA!

        lastNode = go;

        Nodes.Add(lastNode);

        vertexCount++;




    }

    void RenderLine()
    {

        lr.positionCount = vertexCount;

        int i;
        for (i = 0; i < Nodes.Count; i++)
        {
            lr.SetPosition(i, Nodes[i].transform.position);
        }

        lr.SetPosition(i, player.transform.position);
    }

}
