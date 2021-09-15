/* Description: This script is meant to handle the behavior of bullets. So far it propels bullets horizontally until
 * it collides with something, where it then despawns.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Movement Variables")]

    private Rigidbody bulletRB; //The bullet's rigidbody
    public Vector3 bulletForce; //The direction and force applied to move the bullet
    public float bulletSpeed; //The speed at which the bullet moves, this influences the bulletForce Vector3.


    [Header("Dealing Damage")]

    public float damage = 1; //The amount of damage the bullet deals.
    GameObject thePlayer; //The player character's gameObject
    playerHealth thePlayerHealth; //The player's health script
    bool vulnerable; //A bool that determines of the player can be hurt or not.

    [Header("Hitbox")]
    public Collider[] BulletTargets; //An array of colliders to store information on anything the bullet hits
    public float bulletSphereRadius; //The radius of the spherical hurtbox of this attack
    public LayerMask whatIsPlayer; //A layermask only for the player

    GameObject bulletParent;
    public Vector3 backwardsDirection;
    public bool bulletParried = false;

    // Start is called before the first frame update
    void Start()
    {
        //The parent object of the bullet
        bulletParent = transform.parent.gameObject;

        //getting the rigidbody attached to the bullet prefab
        bulletRB = GetComponent<Rigidbody>();
        // A vector 3 representing the magnitude in the x direction the bullet will move
        bulletForce = new Vector3(bulletSpeed, 0, 0);

        //recognizing the player game object to call its scripts' functions
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        //the player's health script, we want to reference this later for the bullet to deal damage
        thePlayerHealth = thePlayer.GetComponent<playerHealth>(); 

    }

    // Update is called once per frame
    void Update()
    {
        //We make the bullet move by giving it the velocity of the bulletForce Vector3
        bulletRB.velocity = bulletForce;

        //We use this bool to determine if the player can take damage. It updates in accordance 
        //with the bool in the player health script that does the same thing.
        vulnerable = thePlayer.GetComponent<playerHealth>().canTakeDamage;

        if (bulletParried == true)
        {
            BulletParried();
            bulletParried = false;
        }

        //Debug.Log("bullet force is: " + bulletForce);

        //BulletHitbox();
    }
    void BulletHitbox()
    {
        //This sets elements of our array as objects with the player tag that enter the overlap sphere
        BulletTargets = Physics.OverlapSphere(this.transform.position, bulletSphereRadius);

        if (BulletTargets.Length == 1)
        {
            if (BulletTargets[0].gameObject.tag == "Player" && vulnerable)
            {
                //We call the function from the playerHealth script that damages the player and pass the amount of damage a bullet does
                //into this function.
                thePlayerHealth.addDamage(damage);

                //Debug.Log("Bullets don't hurt as bad as I thought they would");
            }

            Destroy(this.gameObject);
        }
    }

    void BulletParried()
    {
        //Make bullet no longer hurt Olaf but stun enemies
        //Not implemented Yet 9/12/2021

        //Change direction of bullet
        backwardsDirection = bulletParent.gameObject.GetComponent<UniversalEnemyBehavior>().gameObjectMe.transform.position - bulletRB.transform.position;
        bulletForce = backwardsDirection;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, bulletSphereRadius);
        //Gizmos.DrawSphere(sphereTwo.position, area);
    }

    // OnCollisionEnter is called on the frame that the attached object's collider makes contact with another collider
    void OnCollisionEnter(Collision collision)
    {
        //If the collision that occurred is with a gameObject with the "Player" tag AND the player is capable of taking
        //damage (not in the "post damage invulnerability" state) then this code is executed.
        if (collision.gameObject.tag == "Player" && vulnerable)
        {
            //We call the function from the playerHealth script that damages the player and pass the amount of damage a bullet does
            //into this function.
            thePlayerHealth.addDamage(damage);
            
            //Debug.Log("Bullets don't hurt as bad as I thought they would");
        }

        //the bullet destroys itself upon any collision.
        Destroy(this.gameObject);
    }

}
