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
    private Vector3 bulletForce; //The direction and force applied to move the bullet
    public float bulletSpeed; //The speed at which the bullet moves, this influences the bulletForce Vector3.


    [Header("Dealing Damage")]

    public float damage = 1; //The amount of damage the bullet deals.
    GameObject thePlayer; //The player character's gameObject
    playerHealth thePlayerHealth; //The player's health script
    bool vulnerable; //A bool that determines of the player can be hurt or not.

    // Start is called before the first frame update
    void Start()
    {
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
