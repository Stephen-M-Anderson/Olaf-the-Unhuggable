using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    Animator enAnimation;
    Rigidbody rb;
    public GameObject Player;

    public int waitTimeMin = 0;
    public int waitTimeMax = 2;
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //enAnimation.SetBool("Attack", true);
            //enAnimation.Play("Male Attack 1");
            //Attack();
            StartCoroutine(AttackPattern());
            Debug.Log("Enter");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //enAnimation.SetBool("Attack", false);
            StopCoroutine(AttackPattern());
            Debug.Log("Exit");
        }
    }

    IEnumerator AttackPattern()
    {
        yield return new WaitForSeconds(Random.Range(waitTimeMin, waitTimeMax));
        Debug.Log("In Coroutine");
        Attack();
    }

    public void Attack()
    {
        enAnimation.Play("Male Attack 1");
    }
}
