/* Description: Script you would apply to an instakill surface like lava. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LavaDeath : MonoBehaviour
{
    [Header("Dealing Damage")]

    public float damage = 100; //The amount of damage the lava deals. I purposefully made it really high because it 
                              //should always be an instakill.
    GameObject thePlayer; //The player character's gameObject
    playerHealth thePlayerHealth; //The player's health script
    bool vulnerable; //A bool that determines of the player can be hurt or not.

    // Start is called before the first frame update
    void Start()
    {
        //recognizing the player game object to call its scripts' functions
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        //the player's health script, we want to reference this later for the lava to deal damage
        thePlayerHealth = thePlayer.GetComponent<playerHealth>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // OnCollisionEnter is called on the frame that the attached object's collider makes contact with another collider
    void OnCollisionEnter(Collision coll)
    {
        //If the collision that occurred is with a gameObject with the "Player" then this code is executed.
        if (coll.gameObject.tag == "Player")
        {
            //We call the function from the playerHealth script that damages the player and pass the amount of damage the lava does
            //into this function.
            thePlayerHealth.addDamage(damage);

            //Debug.Log("Lava kills you when you touch it ouch.");
        }
    }
}
