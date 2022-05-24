using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificAttack : MonoBehaviour
{
    Animator EnemyAttackAnimator;

    public GameObject Player;

    bool InCoR = false;

    public float waitfor;   //animation length gets passed into here
    public float attackDelay; //manually set depending on enemy

    public int combometer;  //combo max
    private int combocounter = 0;
    public int rage; //rage max
    private int ragecounter = 0;

    void Start()
    {
        EnemyAttackAnimator = GetComponent<Animator>();
        waitfor = 0;
        combocounter = 0;
        ragecounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void OnTriggerStay(Collider other)
    {
        //check if player entered collider
        if (other.gameObject.CompareTag("Player"))
        {
            //check if coroutine hasn't already started
            if(InCoR == false)
            {
                StartCoroutine(AttackCombo());
                Debug.Log("Start Coroutine");
            }
        }
        Debug.Log("Enter");
    }

    public void OnTriggerExit(Collider other)
    {
        //if player left then stop coroutine
        if (other.gameObject.CompareTag("Player"))
        {
            InCoR = false;
            StopCoroutine(AttackCombo());
        }
        Debug.Log("Exit");
    }
    public void HorizontalAttack()
    {
        EnemyAttackAnimator.SetTrigger("BasicTrigger"); //transition to Basic attack
        combocounter++;                                //build combo
        ragecounter++;                                 //build rage
        waitfor = EnemyAttackAnimator.GetCurrentAnimatorStateInfo(0).length; //wait for lenth of animation
        Debug.Log("Waitfor Horizontal: " + waitfor);
    }
    public void DownAttack()
    {
        EnemyAttackAnimator.SetTrigger("ComboTrigger"); //transition to Combo attack
        combocounter = 0;                               //reset combo
        ragecounter++;                                  //build rage
        waitfor = EnemyAttackAnimator.GetCurrentAnimatorStateInfo(0).length; //wait for length of animation
        Debug.Log("Waitfor Down Attack: " + waitfor);
    }

    public void SpinAttack()
    {
        EnemyAttackAnimator.SetTrigger("RageTrigger"); //transition to Rage attack
        ragecounter = 0;                               //reset rage
        combocounter++;                                //build combo
        waitfor = EnemyAttackAnimator.GetCurrentAnimatorStateInfo(0).length; //wait for length of animaton
        Debug.Log("Waitfor Spin Attack: " + waitfor);
    }

    IEnumerator AttackCombo()
    {
        InCoR = true;
        Debug.Log("In Coroutine");
        //Wait for animation length time + User set delay before checking to run new animation
        yield return new WaitForSeconds(waitfor + attackDelay);
        if (combocounter >= combometer)
        {
            DownAttack();
            Debug.Log("Down Attack");
        } 
        else if (ragecounter >= rage)
        {
            SpinAttack();
        } 
        else
        {
            HorizontalAttack();
            Debug.Log("Horizontal Attack");
        }
        
        InCoR = false;
        //These are just to view the meter building in the animator
        EnemyAttackAnimator.SetInteger("animatorRage", ragecounter);
        EnemyAttackAnimator.SetInteger("animatorCombo", combocounter);
    }
}
