using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleDamage : MonoBehaviour
{
    public float damage = 1;
    public float damageRate;
    public float pushBackForce;

    private Rigidbody playerRB;

    float nextDamage;

    GameObject thePlayer;
    playerHealth thePlayerHealth;

    // Start is called before the first frame update
    void Start()
    {
        //nextDamage = Time.time;
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        thePlayerHealth = thePlayer.GetComponent<playerHealth>();
        playerRB = thePlayer.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Attack();
            //Debug.Log("The Collision was Successful");
        }
    }*/

    void Attack()
    {
        //if(nextDamage <= Time.time)
        //{
            thePlayerHealth.addDamage(damage);
            //nextDamage = Time.time + damageRate;
           Debug.Log("The Attack was Successful");

            //pushBack(thePlayer.transform);
        //}
    }

    //Why the fuck is the pushback in this script and not playerHealth? Get this the fuck outta here!
    /*void pushBack(Transform pushedObject) 
    {
        // This was the old push back function, lets try to make a better one this time
        //Vector3 pushDirection = new Vector3(0, (pushedObject.position.y - transform.position.y), 0).normalized;
        //pushDirection *= pushBackForce;

        //Rigidbody pushedRB = pushedObject.GetComponent<Rigidbody>();
        //pushedRB.velocity = Vector3.zero;
        //pushedRB.AddForce(pushDirection, ForceMode.Impulse);

        Vector3 pushDirection = new Vector3((pushedObject.position.x - transform.position.x), 0, 0).normalized;
        pushDirection *= pushBackForce;

        playerRB.velocity = new Vector3(0, 0, 0);
        playerRB.AddForce(pushDirection, ForceMode.Impulse);

        Debug.Log("The pushback was successful");

    }*/

    /*void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Grappleable")
        {
            
        }
    }*/
}
