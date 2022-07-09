/* Description: This attack was made for testing purposes. This is meant to be the hitbox for the most basic version of a concept we call the
 * "Danger Zone Attack" (I say "we" when at the time of typing this I'm the only one who uses that name because I couldn't think of anything
 * else...). The idea behind this attack is that unless the enemy is stunned then zooming to them will result in them attacking you as soon as
 * you reach them. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLightAttack : MonoBehaviour
{
    [Header("Overlap Sphere Stuff")]
    public Collider[] AttackTargets; //An array of colliders to hold our player's information if they get hit by the attack
    public GameObject centerObject; //The center of the enemy to make sure the sphere hurtbox of the attack is centered
    public float attackSphereRadius; //The radius of the spherical hurtbox of this attack
    public LayerMask whatIsPlayer; //A layermask only for the player

    [Header("Damage Stuff")]
    public float damage = 1; //The amount of damage the bullet deals.
    GameObject thePlayer; //The player character's gameObject
    playerHealth thePlayerHealth; //The player's health script
    bool vulnerable; //A bool that determines of the player can be hurt or not.

    // Start is called before the first frame update
    void Start()
    {
        //recognizing the player game object to call its scripts' functions
        thePlayer = GameObject.FindGameObjectWithTag("Player");
        //the player's health script, we want to reference this later for the bullet to deal damage
        thePlayerHealth = thePlayer.GetComponent<playerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        //We use this bool to determine if the player can take damage. It updates in accordance 
        //with the bool in the player health script that does the same thing.
        vulnerable = thePlayer.GetComponent<playerHealth>().canTakeDamage;

        //This sets elements of our array as objects with the player tag that enter the overlap sphere
        AttackTargets = Physics.OverlapSphere(centerObject.transform.position, attackSphereRadius, whatIsPlayer);

        //If the player has entered the range of the attack and they're vulnerable then they will be hit
        if(AttackTargets.Length >= 1 && vulnerable)
        {
            DoDamage();
        }

    }

    void DoDamage() //This function calls the damage function of the playerHealth Script
    {
        thePlayerHealth.addDamage(damage);
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, attackSphereRadius);
    }

}
