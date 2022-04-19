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
    private int AttackVMin = 1;
    private int AttackVMax = 3;

    bool InCoR = false;
    public int RanNum;
    private int AttackVersion;
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
            if (InCoR == false)
            {
                StartCoroutine(AttackPattern());
                //StartCoroutine(AttackPatternV2());
            }
            Debug.Log("Stay");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InCoR = false;
            StopCoroutine(AttackPattern());
            //StopCoroutine(AttackPatternV2());
            Debug.Log("Exit");
        }
    }

    IEnumerator AttackPattern()
    {
        Debug.Log("In Coroutine");
        InCoR = true;
        //RanNum = Random.Range(waitTimeMin, waitTimeMax);
        RanNum = 2;
        yield return new WaitForSeconds(RanNum);
        Attack();
        InCoR = false;
    }

    /*IEnumerator AttackPatternV2()
    {
        RanNum = 3;
        Debug.Log("In Corutine");
        for (int i = 0; i > 3; i++)
        {
            //Should attack three times total. 1->1->2->End or repeat based on player proximity
            Debug.Log("Entered For");
            if (!enAnimation.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                AttackVersion = i;
                Debug.Log("Looped");
                if (i == 3)
                {
                    AttackVersion = 2;

                }
                i++;
                AttackVersion *= AttackVersion;
                yield return new WaitForSeconds(RanNum);
                Attack();
            }
        }
        Debug.Log("End of Corutine");
    }*/

    public void Attack()
    {
        //AttackVersion = Random.Range(AttackVMin, AttackVMax);

        //DecideAttack();
        Debug.Log(AttackVersion);
        switch (AttackVersion)
        {
            case 1:
                enAnimation.Play("Male Attack 1");
                break;
            case 2:
                enAnimation.Play("Male Attack 2");
                break;
            case 3:
                enAnimation.Play("Male Attack 3");
                break;
        }
    }

    /*public void DecideAttack()
    {
        Debug.Log("Decide Attack");
        for (int i = 1; i < 4; i++)
        {
            //Should attack three times total. 1->1->2->End or repeat based on player proximity
            Debug.Log("Entered For");
            //this.enAnimation.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
            //enAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !enAnimation.IsInTransition(0)

            if (enAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !enAnimation.IsInTransition(0))
            {
                Debug.Log("Looped");
                AttackVersion = 1;
                if (i == 3)
                {
                    Debug.Log("Innermost loop");
                    AttackVersion =  2;
                    Debug.Log(AttackVersion + "Innermost loop");
                }
                Debug.Log(AttackVersion + "In Loop");
            }
        }
        Debug.Log(AttackVersion + "Outside Loop");
    }*/
}
