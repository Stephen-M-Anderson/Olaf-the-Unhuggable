using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody bulletRB;
    private Vector3 bulletForce;
    public float bulletSpeed;
    public float damage = 1;
    GameObject thePlayer;
    playerHealth thePlayerHealth;

    // Start is called before the first frame update
    void Start()
    {
        bulletRB = GetComponent<Rigidbody>();
        bulletForce = new Vector3(bulletSpeed, 0, 0);

        thePlayer = GameObject.FindGameObjectWithTag("Player");
        thePlayerHealth = thePlayer.GetComponent<playerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        bulletRB.velocity = bulletForce;
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            thePlayerHealth.addDamage(damage);
            Debug.Log("The Attack was Successful");
        }

        Destroy(this.gameObject);
    }
}
