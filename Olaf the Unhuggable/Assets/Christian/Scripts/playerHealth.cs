/* Description: The purpose of this script is to manage the player's health. This includes taking damage and eventually when it
 * is added to the game will also include healing damage.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{

    [Header("Player Components")]

    private Rigidbody myRB; //The player's rigidbody
    private Animator myAnimator; //The animator that handles the animations for the player


    [Header("Bools")]

    public bool canTakeDamage = true; //If this bool is true then the player is capable of taking damage
    private bool canDie = false; //When this bool is set to true then the makeDead() function runs
    private bool isDead = false; //This bool is used to tell the animator to play the death animation
    public bool isDamaged = false; //This bool is true when the player takes damage
    private bool facingRight; //This bool is used to determine the direction the player is facing.
    bool invulnerabilityBool = false; //When this bool is true we want to run the "Invulnerability" function every frame


    [Header("Float Values")]

    public float fullHealth; //The amount of Health the player starts with & is currently set in the inspector
    private float currentHealth; //The amount of health the player currently has
    public float damageFreezeWaitTime = 1f; //The amount of time for the Coroutine that disables player input to flip its bool
    public float postDamageWaitTime = 2f; //The amount of time for the Coroutine that handles invulnerability to flip its bool
    public float deathWaitTime = 3f; //The amount of time for the Coroutine that tells the player when it can die to flip its bool


    [Header("UI Elements")]

    public Text healthText; //The text on the UI that tells the player how much health they have


    // Start is called before the first frame update
    void Start()
    {
        //GameObject bullet = GameObject.FindGameObjectWithTag("Bullet");
        //Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>(), true);

        currentHealth = fullHealth; //Setting the current health of the player full
        myRB = GetComponent<Rigidbody>(); //Calling the rigidbody of the player for us to use later
        myAnimator = GetComponent<Animator>(); //Calling the animator of the player for us to use later

        healthText.text = currentHealth.ToString(); //Send the current health to the UI element on the screen
    }

    // Update is called once per frame
    void Update()
    {
        //I don't know how to get the damaged animation to play since the damage function occurs over the course of one frame.
        //myAnimator.SetBool("damaged", isDamaged);

        facingRight = GetComponent<playerController>().facingRight; //Every frame we check if the character is facing right

        if (invulnerabilityBool == true)
        {
                Invulnerability(); //This function makes the player invulnerable while the "Damage" animation plays
        }

        //Once the coroutine ends it flips this bool and this condition is met
        if (canDie == true && isDead == true)
        {
            makeDead(); //Call the function that reloads the game upon death
        }
    }

    //This function is called when the player takes damage. It accepts a float value which is the amount of damage the player takes.
    //It's also made public so that scripts attached to things that can harm the player are able to access this function.
    public void addDamage(float damage)
    {
        //The canTakeDamage bool dictates whether or not the player is in post damage invulnerability. If they aren't, then we run
        //this next block of code.
        if (canTakeDamage == true)
        {
            canTakeDamage = false; //This enters the player into the post damage invulnerability state
            isDamaged = true; //Use this bool to tell the animator you are in a state of being damaged
            if (GetComponent<playerController>().ballManBool == true)
            {
                GetComponent<playerController>().BallModeInactive();
            }
            myAnimator.SetBool("damagedTrigger", isDamaged); //Actually set the bool damaged to influence the animator
            currentHealth -= damage; //subtract the damage value from the player's current health
            healthText.text = currentHealth.ToString(); //Send the current health to the UI element on the screen
            this.gameObject.GetComponent<GrappleScriptEvenNewer>().stopGrappleBool = true; //Getting hit should stop grappling

            StartCoroutine(DamageFreezeWait()); //Freeze player movement for a bit

            Knockback(); //This function knocks the player back after getting hit

            //Debug.Log("I took one hit of damage oh God oh fuck. Oh and by the way my health is currently at a respectable: " + currentHealth);
        }

        isDamaged = false; //Now that the function is over we end the damage state

        //This condition is called when the player's health goes to zero
        if (currentHealth <= 0)
        {
            healthText.text = "0"; //In case Health goes into the negative we want to just set the UI text to "0" 
                                  //instead of the actual value
            Death(); //Call the function that kills the player
        } else
        {
            StartCoroutine(PostDamageWait()); //Start the Coroutine for Post Damage Invulnerability
        }

    }

    //This function knocks the character back after taking damage
    void Knockback()
    {
        

        //If the player is facing right they get knocked back to the left
        if (facingRight)
        {
            myRB.velocity = Vector3.zero; //When damaged you lose all current velocity
            myRB.AddForce(new Vector3(-100, 0, 0), ForceMode.Impulse); //Apply a force to the left
        }
        //If the player is facing left they get knocked back to the right
        else
        {
            myRB.velocity = Vector3.zero; //When damaged you lose all current velocity
            myRB.AddForce(new Vector3(100, 0, 0), ForceMode.Impulse); //Apply a force to the right
        }
    }

    void Invulnerability()
    {

        //Debug.Log("Man is this Invulnerability bool flipping shit even working dawg?");

        Physics.IgnoreLayerCollision(14, 13, true); //The player layer and parryable layer will now ignore collision
        Physics.IgnoreLayerCollision(14, 15, true); //The player layer and non-parryable layer will now ignore collision

        //Blinking
        if (GetComponent<playerController>().ballManBool == true)
        {
            //ballman blinking
        } else
        {
            //non ballman blinking
        }

    }

    //This function handles player death
    void Death()
    {
        isDead = true; //This bool is used for the death animation
        myAnimator.SetBool("deadTrigger", isDead); //Set the bool in the animator to play the death animation
        myAnimator.SetFloat("speed", 0); //Set the speed variable in the animator to 0 to prevent the movement animation from playing
        //myRB.constraints = RigidbodyConstraints.FreezeAll; //Stop movement to allow the death animation to play out in place
        //GetComponent<playerController>().inputDisabled = true;
        StartCoroutine(DeathWait()); //Start this coroutine to allow time for the animation to play
    }

    //The function that reloads the game upon death
    public void makeDead()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //Reloads the current scene
    }

    //This Coroutine is what dictates when the player leaves the post damage invulnerability state
    IEnumerator PostDamageWait()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        invulnerabilityBool = true; //Set this bool to true to begin damage blinking

        //yield on a new YieldInstruction that waits however many seconds. 
        yield return new WaitForSeconds(postDamageWaitTime); //Seconds to wait before we flip the bool

        if (isDead == false) //If we're dead we don't exactly need to flip this bool now do we?
        {
            canTakeDamage = true;
        } 

        invulnerabilityBool = false; //flip the bool when our damage blinking should end

        if (isDead == false) //We don't want attacks crashing into our dead body so only flip this bool if we're alive
        {
            Physics.IgnoreLayerCollision(14, 13, false); //Allow collision between player and parryable stuff once more!
            Physics.IgnoreLayerCollision(14, 15, false); //Allow collision between player and non-parryable stuff once more!
        }

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    //This coroutine is what allows the death animation to play before the game resets.
    IEnumerator DeathWait()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits however many seconds. 
        yield return new WaitForSeconds(deathWaitTime);

        canDie = true;

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    IEnumerator DamageFreezeWait()
    {
        GetComponent<playerController>().inputDisabled = true; //Stop movement for a bit

        yield return new WaitForSeconds(damageFreezeWaitTime);

        if (isDead == false) //We don't want this bool to flip if the death animation has begun
        {
            GetComponent<playerController>().inputDisabled = false; //Start movement up again
        }
    }

    // OnCollisionEnter is called on the frame that the attached object's collider makes contact with another collider
    /*void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true);
        }
    }*/

}
