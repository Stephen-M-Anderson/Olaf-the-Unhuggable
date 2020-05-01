using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleDamage : MonoBehaviour
{
    public float damage = 1;
    public float damageRate;
    public float pushBackForce;

    float nextDamage;

    GameObject thePlayer;
    playerHealth thePlayerHealth;

    // Start is called before the first frame update
    void Start()
    {
        nextDamage = Time.time;
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        thePlayerHealth = thePlayer.GetComponent<playerHealth>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Attack();
            Debug.Log("The Collision was Successful");
        }
    }

    void Attack()
    {
        if(nextDamage <= Time.time)
        {
            thePlayerHealth.addDamage(damage);
            nextDamage = Time.time + damageRate;
            Debug.Log("The Attack was Successful");

            pushBack(thePlayer.transform);
        }
    }

    void pushBack(Transform pushedObject) 
    {
        Vector3 pushDirection = new Vector3(0, (pushedObject.position.y - transform.position.y), 0).normalized;
        pushDirection *= pushBackForce;

        Rigidbody pushedRB = pushedObject.GetComponent<Rigidbody>();
        pushedRB.velocity = Vector3.zero;
        pushedRB.AddForce(pushDirection, ForceMode.Impulse);

        Debug.Log("The pushback was successful");

    }
}
