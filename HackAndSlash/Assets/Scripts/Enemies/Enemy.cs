using UnityEngine;
using UnityEngine.U2D.IK;
using System;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Transform playerTr;
    [SerializeField] protected Animator enemyAnimator;

    [Header("BodyParts")]
    [SerializeField] private EnemyLimb[] bodyParts;

    [Header("Health")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth;
    protected bool isDead = false;

    [Header("Stats")]
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackCooldown = 1.5f;
    protected float lastAttackTime;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 3f;

    

    public static Action<string[]> OnDamage;


    protected virtual void Awake() 
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage, string key)
    {

        List<string> keys = new List<string>();

        keys.Add(key);
        

        if(isDead) return;
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            keys.Add("Dead");
            Die();

        } 

        OnDamage?.Invoke(keys.ToArray());
    }

    protected virtual void Die()
    {
        if(isDead) return;
        isDead = true;

        if(bodyParts != null)
        {
            for(int i = 0; i < bodyParts.Length; i++)
            {
                if(bodyParts[i] != null)
                {
                    bodyParts[i].OnEnemyDeath();
                } 
            }
        }

        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.None;
        }

        this.enabled = false;
        enemyAnimator.enabled = false;
    }

    protected virtual void Movement()
    {
        
    }

    protected virtual void Attack()
    {
        
    }

    protected virtual void Jump()
    {
        
    }
    
    protected virtual void Start() 
    {
        
    }

    protected virtual void Update()
    {
        if(isDead || playerTr == null) return;
        Movement();
        Attack();
    }

}
