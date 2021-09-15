/* Description: This is the most basic enemy behavior script I could make with the intention of testing parrying. This enemy can do what we 
 * call a "Danger Zone Attack". The idea behind this attack is that unless the enemy is stunned then zooming to them will result in them 
 * attacking you as soon as you reach them. It can't move or anything, just shoot like a regular turret and do that Danger Zone Attack.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalEnemyBehavior : MonoBehaviour
{
    public Collider[] DangerZoneTargets; //An array of colliders that holds the player if they enter the DANGER ZONE's overlap sphere
    public Collider[] NotSoDangerZoneTargets; //An array of colliders that holds the player if they enter the other zone's overlap sphere
    Vector3 spherePosition; //We want the center of our overlap spheres to be at this Vector3 position
    public float sphereRadius; //The value that we want to set our Danger Zone's radius to be
    public float doubleSphereRadius; //The value that we want to set our Not So Danger Zone's radius to be
    public LayerMask whatIsPlayer; //This layermask is specifically to tell if something is the player
    public GameObject player; //The player's game object
    public GameObject gameObjectMe; //This is the part of this game object that other components will reference for zooming

    public GameObject redLight; //This object holds the Danger Zone Attack
    public GameObject yellowLight; //This object let's the player know the Danger Zone Attack is near
    public GameObject greenLight; //This object let's the player know the Danger Zone Attack is not so near

    public bool enemyIsStunned = false; //This bool dictates whether or not this enemy is currently in the stunned state
    public float stunDuration; //The duration in seconds that the stun state lasts
    public float attackDuration; //The duration in seconds that the Danger Zone Attack itself lasts
    bool dangerZoneBool = false; //When this bool is active then the danger zone attack is underway
    public bool stunnedBool = false;
    public bool parryStun = false;

    // Start is called before the first frame update
    void Start()
    {
        //We set the center of our overlap sphere to this component because it happens to be at the center of the enemy
        spherePosition = gameObjectMe.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Initializing our overlap spheres
        DangerZoneTargets = Physics.OverlapSphere(spherePosition, sphereRadius, whatIsPlayer);
        NotSoDangerZoneTargets = Physics.OverlapSphere(spherePosition, doubleSphereRadius, whatIsPlayer);

        //if player parries inside of the overlap sphere the stunned function is called
        if(parryStun == true && enemyIsStunned == false) 
        {
            stunnedBool = true; //This will call the stun function in Update
            parryStun = false; //Setting this bool back to false so that it won't be infinitely fucking stunning the enemy
        }

        //These are the conditions by which a Danger Zone Attack is initiated
        if(player.GetComponent<GrappleScriptEvenNewer>().whatAmIZoomingTo == gameObjectMe && enemyIsStunned == false)
        {
            dangerZoneBool = true;
        } else
        {
            dangerZoneBool = false;
        }

        //We use bools to do the Danger Zone Attack because it can be canceled out of at any time
        if(dangerZoneBool == true)
        {
            DangerZoneAttack();
        } 
        else
        {
            RemoveLights();
        }

        if (stunnedBool == true && enemyIsStunned == false)
        {
            Stunned();
        }

    }

    void Stunned()
    {
        Debug.Log("Oh fuck " + this.name + " can't move because that bitch is STUNNED!");

        stunnedBool = false;

        //Disable Movement function goes here

        //Coroutine to flip the stunned bool
        StartCoroutine(StunWait());
    }

    void DangerZoneAttack() //The attack that occurs if a player is zooming to the enemy while not stunned
    {
        Debug.Log("Danger Zone Attack has commenced on enemy " + this.name + " motherfucker");

        if(DangerZoneTargets.Length == 0 && NotSoDangerZoneTargets.Length == 1)
        {
            //When the enemy is close to the danger zone then the yellow light shows

            greenLight.SetActive(false);
            yellowLight.SetActive(true);
            redLight.SetActive(false);
        }
        else if(DangerZoneTargets.Length == 1)
        {
            //When the enemy is IN the danger zone, then the red light shows

            greenLight.SetActive(false);
            yellowLight.SetActive(false);
            redLight.SetActive(true);

            //The red light stays until a coroutine is over
            StartCoroutine(RedLightWait());
        }
        else
        {
            //When the enemy is far then the green light shows

            greenLight.SetActive(true);
            yellowLight.SetActive(false);
            redLight.SetActive(false);
        }

    }

    void RemoveLights() //This function gets rid of the green and yellow lights for the Danger Zone Attack
    {
        greenLight.SetActive(false);
        yellowLight.SetActive(false);
        //redLight.SetActive(false);
    }

    IEnumerator StunWait() //The coroutine that dictates how long the Stun function lasts
    {
        enemyIsStunned = true;
        yield return new WaitForSeconds(stunDuration);
        enemyIsStunned = false;

        Debug.Log("Well shit my dude, " + this.name + " can move again! Fucker ain't stunned no more");

        //undo other stun stuff goes here
    }

    IEnumerator RedLightWait() //The coroutine that dictates how long the red light stays out
    {
        yield return new WaitForSeconds(attackDuration);
        redLight.SetActive(false);

        Debug.Log("Ding-dong, the red light is dead! Which old light? The red light!");
        //undo other stun stuff goes here
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(spherePosition, sphereRadius);
    }
}
