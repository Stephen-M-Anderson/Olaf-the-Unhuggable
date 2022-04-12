using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificAttack : MonoBehaviour
{
    Animator EnemyAttackAnimator;
    

    //public List<Transform> range;

    public GameObject Player;

    bool InCoR = false;

    public float waitfor;
    public float attackDelay;
    public int combometer;
    private int combocounter = 0;
    public int rage;
    private int ragecounter = 0;

    void Start()
    {
        EnemyAttackAnimator = GetComponent<Animator>();
        /*EnemyAttackAnimator.SetInteger("maxRage", rage);
        EnemyAttackAnimator.SetInteger("maxCombo", combometer);*/
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
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
        if (other.gameObject.CompareTag("Player"))
        {
            InCoR = false;
            StopCoroutine(AttackCombo());
        }
        Debug.Log("Exit");
    }

    IEnumerator AttackCombo()
    {
        InCoR = true;
        Debug.Log("In Coroutine");
        //
        yield return new WaitForSeconds(waitfor + attackDelay);
        if (combocounter >= combometer)
        {
            EnemyAttackAnimator.SetBool("comboFill", true);
            DownAttack();
            
            Debug.Log("Down Attack");
        } 
        else if (ragecounter >= rage)
        {
            EnemyAttackAnimator.SetBool("rageFill", true);
            SpinAttack();
            
        } 
        else
        {
            EnemyAttackAnimator.SetBool("BasicAttack", true);
            HorizontalAttack();
            Debug.Log("Horizontal Attack");
        }
        //HorizontalAttack();
        InCoR = false;
        EnemyAttackAnimator.SetInteger("animatorRage", ragecounter);
        EnemyAttackAnimator.SetInteger("animatorCombo", combocounter);
    }

    public void HorizontalAttack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack Horizontal");
        combocounter++;
        ragecounter++;

        waitfor = EnemyAttackAnimator.GetCurrentAnimatorStateInfo(0).length;
        EnemyAttackAnimator.SetBool("BasicAttack", false);
        Debug.Log("Waitfor: " + waitfor);
    }

    public void DownAttack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack Downward");
        combocounter = 0;
        ragecounter++;
        waitfor = EnemyAttackAnimator.GetCurrentAnimatorStateInfo(0).length;
        EnemyAttackAnimator.SetBool("comboFill", false);
        Debug.Log("Waitfor: " + waitfor);
    }

    public void SpinAttack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack 360 High");
        ragecounter = 0;
        combocounter++;
        waitfor = EnemyAttackAnimator.GetCurrentAnimatorStateInfo(0).length;
        EnemyAttackAnimator.SetBool("rageFill", false);
        Debug.Log("Waitfor: " + waitfor);
    }
}
