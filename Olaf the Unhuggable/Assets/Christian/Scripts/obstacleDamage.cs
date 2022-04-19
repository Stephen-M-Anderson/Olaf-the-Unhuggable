/* Description: A script to attach to an obstacle that would do minor damage to the player.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleDamage : MonoBehaviour
{
    [Header("Dealing Damage")]

    public float damage = 1; //The amount of damage the obstacle deals.
    GameObject thePlayer; //The player character's gameObject
    playerHealth thePlayerHealth; //The player's health script
    bool vulnerable; //A bool that determines of the player can be hurt or not.

    // Start is called before the first frame update
    void Start()
    {
        //recognizing the player game object to call its scripts' functions
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        //the player's health script, we want to reference this later for the obstacle to deal damage
        thePlayerHealth = thePlayer.GetComponent<playerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
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

            //Debug.Log("I'll tell yah the REAL obstacle is my ex-wife huh? Thank you thank you I'll be here all week.");
        }
    }

}
