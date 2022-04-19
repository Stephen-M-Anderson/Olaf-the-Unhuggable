using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        
    }

    public void addDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            death();
        }
    }

    public void death()
    {
        Destroy(gameObject);
    }
}
