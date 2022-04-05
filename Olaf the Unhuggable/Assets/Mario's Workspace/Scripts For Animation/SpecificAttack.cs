using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificAttack : MonoBehaviour
{
    Animator EnemyAttackAnimator;

    public GameObject Player;

    bool InCoR = false;

    public int waitfor;
    public int combometer;
    public int rage;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EnemyAttackAnimator = GetComponent<Animator>();
        
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
        if (combometer >= 2)
        {
            DownAttack();
            Debug.Log("Down Attack");
        } 
        else if (rage >= 4)
        {
            SpinAttack();
        } 
        else
        {
            HorizontalAttack();
        }
        //HorizontalAttack();
        InCoR = false;
    }

    public void HorizontalAttack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack Horizontal");
        combometer++;
        rage++;
    }

    public void DownAttack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack Downward");
        combometer = 0;
        rage++;
    }

    public void SpinAttack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack 360 High");
        rage = 0;
        Debug.Log("Rage: " + rage);
    }
}
