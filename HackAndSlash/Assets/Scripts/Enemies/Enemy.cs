using UnityEngine;
using UnityEngine.U2D.IK;
using System;
using System.Collections.Generic;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Transform playerTr;
    [SerializeField] protected Animator enemyAnimator;

    [Header("BodyParts")]
    [SerializeField] private EnemyLimb[] bodyParts;

    [Header("DisappearLimbs")]
    [SerializeField] private float timeBeforeFade = 3f;
    [SerializeField] private float fadeDuration = 1.5f;

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

    [Header("Effects")]
    [SerializeField] protected GameObject[] bloodPrefab;
    [SerializeField] protected Transform bloodSpawnPoint;

    private Arena _assignedArena;

    public static Action<string[]> OnDamage;
    public static Action<Enemy> OnEnemySpawned;


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

    public void AssignToArena(Arena arena)
    {
        _assignedArena = arena;
    }

    protected virtual void Die()
    {
        if(isDead) return;
        isDead = true;

        if (_assignedArena != null)
        {
            _assignedArena.KillEnemy();
        }

        if (bloodPrefab != null && bloodPrefab.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, bloodPrefab.Length);
            GameObject bloodSpawn = bloodPrefab[randomIndex];

            Vector3 spawnPos = bloodSpawnPoint != null ? bloodSpawnPoint.position : transform.position;

            if(bloodSpawn != null)
            {
                Instantiate(bloodSpawn, spawnPos, Quaternion.identity);
            }
        }
        

        if (bodyParts != null)
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

        if (enemyAnimator != null) enemyAnimator.enabled = false;

        SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();

        StartCoroutine(FadeDestroyBodyParts(allSprites));

  
    }

    private IEnumerator FadeDestroyBodyParts(SpriteRenderer[] sprites)
    {
        yield return new WaitForSeconds(timeBeforeFade);

        float elapsedTime = 0f;
        while(elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            for (int i = 0; i < sprites.Length; i++)
            {
                if(sprites[i] != null)
                {
                    Color c = sprites[i].color;
                    c.a = alpha;
                    sprites[i].color = c;
                }
            }
            yield return null;
        }

        Destroy(gameObject);    
    }

    public void SetPlayer(Transform player)
    {
        playerTr = player;
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
        OnEnemySpawned.Invoke(this);
    }

    protected virtual void Update()
    {
        if(isDead || playerTr == null) return;
        Movement();
        Attack();
    }

}
