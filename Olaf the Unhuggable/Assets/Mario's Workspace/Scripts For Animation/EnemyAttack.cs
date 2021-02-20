using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    Animator enAnimation;
    Rigidbody rb;
    public GameObject Player;

    private int waitTimeMin = 1;
    private int waitTimeMax = 5;

    bool InCoR = false;
    public int RanNum;
    //public GameObject gameObject;
    //bool distanceChecker = false;
    //float time = 2.0f;

    void Start()
    {
        enAnimation = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //enAnimation.SetBool("Attack", true);
            //enAnimation.Play("Male Attack 1");
            //Attack();
            if (InCoR == false)
            {
                StartCoroutine(AttackPattern());
            }
            
            Debug.Log("Enter");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //enAnimation.SetBool("Attack", false);
            InCoR = false;
            StopCoroutine(AttackPattern());
            Debug.Log("Exit");
        }
    }

    IEnumerator AttackPattern()
    {
        InCoR = true;
        RanNum = Random.Range(waitTimeMin, waitTimeMax);
        Debug.Log(RanNum);
        yield return new WaitForSeconds(RanNum);
        Attack();
        Debug.Log("In Coroutine");
        InCoR = false;
    }

    public void Attack()
    {
        enAnimation.Play("Male Attack 1");
    }
}
