using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificAttack : MonoBehaviour
{
    Animator EnemyAttackAnimator;

    public GameObject Player;

    bool InCoR = false;

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
        yield return new WaitForSeconds(5);
        Attack();
        InCoR = false;
    }

    public void Attack()
    {
        EnemyAttackAnimator.Play("Standing Melee Attack Downward");
    }
}
