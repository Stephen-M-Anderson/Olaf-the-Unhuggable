using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryBulletScript : MonoBehaviour
{
    [Header("Movement Variables")]

    private Rigidbody bulletRB; //The bullet's rigidbody
    private Vector3 bulletForce; //The direction and force applied to move the bullet
    public float bulletSpeed; //The speed at which the bullet moves, this influences the bulletForce Vector3.

    // Start is called before the first frame update
    void Start()
    {
        //getting the rigidbody attached to the bullet prefab
        bulletRB = GetComponent<Rigidbody>();
        // A vector 3 representing the magnitude in the x direction the bullet will move
        bulletForce = new Vector3(bulletSpeed, 0, 0);

    }

    // Update is called once per frame
    void Update()
    {
        //We make the bullet move by giving it the velocity of the bulletForce Vector3
        bulletRB.velocity = bulletForce;
    }

    // OnCollisionEnter is called on the frame that the attached object's collider makes contact with another collider
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<UniversalEnemyBehavior>().stunnedBool = true;

            //Debug.Log("Oh fuck enemy " + collision.gameObject.name + " got hit by a fucking bullet my dude");
        }

        //the bullet destroys itself upon any collision.
        Destroy(this.gameObject);
    }
}
