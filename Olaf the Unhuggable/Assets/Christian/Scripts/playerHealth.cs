using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerHealth : MonoBehaviour
{

    public float fullHealth = 6f;
    float currentHealth;

    private Rigidbody myRB;
    private Animator myAnimator;

    private bool isDead = false;
    private bool isDamaged = false;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = fullHealth;
        myRB = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        myAnimator.SetBool("damaged", isDamaged);
    }

    public void addDamage(float damage)
    {
        isDamaged = true;
        myAnimator.SetBool("damaged", isDamaged);
        currentHealth -= damage;
        isDamaged = false;
        Debug.Log("I took one hit of damage oh God oh fuck");

        //StartCoroutine(StillWaiting());

        if (currentHealth <= 0)
        {
            isDead = true;
            myAnimator.SetBool("dead", isDead);
            StartCoroutine(StillWaiting());
            makeDead();
        }
    }

    IEnumerator StillWaiting()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(10);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    public void makeDead()
    {
        SceneManager.LoadScene("SampleScene2");
    }
}
