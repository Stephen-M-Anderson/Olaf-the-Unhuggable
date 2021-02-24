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

    public bool canTakeDamage; //If this bool is true then the player is capable of taking damage
    private bool canDie; //When this bool is set to true then the makeDead() function runs
    private bool isDead; //This bool is used to tell the animator to play the death animation
    private bool isDamaged; //This bool is true when the player takes damage
    private bool facingRight; //This bool is used to determine the direction the player is facing.


    [Header("Float Values")]

    public float fullHealth; //The amount of Health the player starts with & is currently set in the inspector
    private float currentHealth; //The amount of health the player currently has

    [Header("UI Elements")]

    public Text healthText; //The text on the UI that tells the player how much health they have


    // Start is called before the first frame update
    void Start()
    {
        //Setting all of our bools the way we want them at the first frame of the game
        canTakeDamage = true; //The player starts in a state capable of being hit.
        canDie = false; //The player has full health at Start() so they aren't in the dying state
        isDead = false; //There wouldn't be a game if the player was dead dude...
        isDamaged = false; //We only want this bool active if the player is taking damage

        currentHealth = fullHealth; //Setting the current health of the player full
        myRB = GetComponent<Rigidbody>(); //Calling the rigidbody of the player for us to use later
        myAnimator = GetComponent<Animator>(); //Calling the animator of the player for us to use later
    }

    // Update is called once per frame
    void Update()
    {
        //I don't know how to get the damaged emotion to play since the damage function occurs over the course of one frame.
        //myAnimator.SetBool("damaged", isDamaged);

        facingRight = GetComponent<playerController>().facingRight; //Every frame we check if the character is facing right
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
            myAnimator.SetBool("damaged", isDamaged); //Actually set the bool damaged to influence the animator
            currentHealth -= damage; //subtract the damage value from the player's current health
            healthText.text = currentHealth.ToString(); //Send the current health to the UI element on the screen

            Knockback(); //This function knocks the player back after getting hit
            
            //Debug.Log("I took one hit of damage oh God oh fuck");
        }

        //This condition is called when the player's health goes to zero
        if (currentHealth <= 0)
        {
            healthText.text = "0"; //In case Health goes into the negative we want to just set the UI text to "0" 
                                  //instead of the actual value
            Death(); //Call the function that kills the player
        }

        isDamaged = false; //Now that the function is over we end the damage state
        myAnimator.SetBool("damaged", isDamaged); //Set the bool damaged to influence the animator
        StartCoroutine(PostDamageWait()); //Start the Coroutine for Post Damage Invulnerability
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

    //This function handles player death
    void Death()
    {
        isDead = true; //This bool is used for the death animation
        myAnimator.SetBool("dead", isDead); //Set the bool in the animator to play the death animation
        myAnimator.SetFloat("speed", 0); //Set the speed variable in the animator to 0 to prevent the movement animation from playing
        myRB.constraints = RigidbodyConstraints.FreezeAll; //Stop movement to allow the death animation to play out in place
        StartCoroutine(DeathWait()); //Start this coroutine to allow time for the animation to play

        //Once the coroutine ends it flips this bool and this condition is met
        if (canDie == true)
        {
            makeDead(); //Call the function that reloads the game upon death
        }
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

        //yield on a new YieldInstruction that waits however many seconds. 
        yield return new WaitForSeconds(2); //Seconds to wait before we flip the bool

        canTakeDamage = true;

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    //This coroutine is what allows the death animation to play before the game resets.
    IEnumerator DeathWait()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits however many seconds. 
        yield return new WaitForSeconds(3);

        canDie = true;

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
