using UnityEngine;
using System;
using System.Collections.Generic;

public class Player_Health : MonoBehaviour, IDamageable
{

    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float regenHealth = 5f;

    public static Action<string[]> OnDamage;
    


    private void OnEnable() 
    {
        Style_Controller.OnStyleThresholds += Heal;
    }

    private void OnDisable() 
    {
        Style_Controller.OnStyleThresholds -= Heal;
    }
    
    private void Awake() 
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, string key)
    {
        List<string> keys = new List<string>();

        keys.Add(key);

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnDamage?.Invoke(keys.ToArray());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //death
    }

    public void Heal(int thresholdCount)
    {

        
        float healAmount = 1f * thresholdCount;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        

    }




}
