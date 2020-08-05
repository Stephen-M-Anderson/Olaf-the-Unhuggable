using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{

    public float fullHealth;
    float currentHealth;

    private Rigidbody myRB;
    private Animator myAnimator;

    private bool canTakeDamage;
    private bool canDie;
    private bool isDead;
    private bool isDamaged;
    private bool currentlyDashing;

    private bool facingRight;

    public Text healthText;


    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        isDamaged = false;
        canTakeDamage = true;
        canDie = false;
        currentHealth = fullHealth;
        myRB = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        //myRB.constraints = RigidbodyConstraints.None;
    }

    // Update is called once per frame
    void Update()
    {
        currentlyDashing = GetComponent<playerController>().isDashing;

        //myAnimator.SetBool("damaged", isDamaged);

        healthText.text = currentHealth.ToString();

        facingRight = GetComponent<playerController>().facingRight;
    }

    public void addDamage(float damage)
    {

        //if (canTakeDamage == true && currentlyDashing == false)
        if (canTakeDamage == true)
        {
            canTakeDamage = false;
            isDamaged = true;
            myAnimator.SetBool("damaged", isDamaged);
            currentHealth -= damage;
            if (facingRight)
            {
                myRB.velocity = Vector3.zero;
                myRB.AddForce(new Vector3(-100, 0, 0), ForceMode.Impulse);
            } else
            {
                myRB.velocity = Vector3.zero;
                myRB.AddForce(new Vector3(100, 0, 0), ForceMode.Impulse);
            }
            StartCoroutine(StillWaiting());
            Debug.Log("I took one hit of damage oh God oh fuck");
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            myRB.constraints = RigidbodyConstraints.FreezeAll;
            myAnimator.SetBool("dead", isDead);
            StartCoroutine(StillWaitingDeath());

            if (canDie == true)
            {
                makeDead();
            }
        }

        isDamaged = false;
        myAnimator.SetBool("damaged", isDamaged);
    }

    IEnumerator StillWaiting()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits however many seconds. 
        yield return new WaitForSeconds(2);

        canTakeDamage = true;

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    IEnumerator StillWaitingDeath()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits however many seconds. 
        yield return new WaitForSeconds(3);

        canDie = true;

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    public void makeDead()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
