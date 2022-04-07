using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificAttack : MonoBehaviour
{
    Animator EnemyAttackAnimator;

    public List<Transform> range;

    public GameObject Player;

    bool InCoR = false;

    public int waitfor;
    public int combometer;
    private int combocounter = 0;
    public int rage;
    private int ragecounter = 0;

    void Start()
    {
        EnemyAttackAnimator = GetComponent<Animator>();
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
        yield return new WaitForSeconds(waitfor);
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
        //HorizontalAttack();
        InCoR = false;
    }

    public void HorizontalAttack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack Horizontal");
        combocounter++;
        ragecounter++;
    }

    public void DownAttack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack Downward");
        combocounter = 0;
        ragecounter++;
    }

    public void SpinAttack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack 360 High");
        ragecounter = 0;
        Debug.Log("Rage: " + rage);
    }
}
