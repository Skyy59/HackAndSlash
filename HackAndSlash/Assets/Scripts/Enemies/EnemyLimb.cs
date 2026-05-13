using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyLimb : MonoBehaviour, IDamageable
{
   [Header("References")]
   [SerializeField] private Enemy mainEnemy;
   [SerializeField] private Transform limbEnemy;

    [Header("Damage")]
   [SerializeField] private float damageMultiplier = 1f;
   [SerializeField] private bool canBeSevered = true;
   [SerializeField] private float limbHealth = 50f;

   [Header("DeadPhysics")]
   [SerializeField] private Rigidbody2D limbRb;
   [SerializeField] private bool mainBodyLimb;

   [SerializeField] private string key;

   private bool _isSevered = false;
   public static Action<string[]> OnDamage;

   
   
   
   private void Awake() 
   {
       if(mainBodyLimb)
        {
            limbRb.bodyType = RigidbodyType2D.Kinematic;
            limbRb.gravityScale = 0;
            
        }
        else
        {
            limbRb.bodyType = RigidbodyType2D.Dynamic;
            limbRb.gravityScale = 1;

            RandomForce();
        }  
   }

   public void TakeDamage(float damage, string _key)
    {
        Debug.Log($"Limb golpeado. Daño recibido: {damage}. Vida restante: {limbHealth}");

        List<string> keys = new List<string>();

        keys.Add(_key);
        keys.Add(key);


        if (_isSevered) return;
        
        //if(mainEnemy != null && !canBeSevered)
        {
            //mainEnemy.TakeDamage(damage * damageMultiplier, "");

            
        }

        if (mainEnemy != null)
        {
            OnDamage?.Invoke(keys.ToArray());

            limbHealth -= damage;
            mainEnemy.TakeDamage(damage * damageMultiplier, "");


            if(canBeSevered && limbHealth <= Mathf.Epsilon)
            {
                Sever();
            }
        }
    }

    private void Sever()
    {
        if(_isSevered) return;
        _isSevered = true;

        Detach();
    }

    public void OnEnemyDeath()
    {
        if(_isSevered) return;

        limbRb.bodyType = RigidbodyType2D.Dynamic;
        limbRb.gravityScale = 1;

    }

    private void Detach()
    {
        if(limbRb != null)
        {
            transform.localScale = Vector3.zero;

            Instantiate(limbEnemy, transform.position, transform.rotation);

        }
    }

    private void RandomForce()
    {
        Vector2 force  = new Vector2(UnityEngine.Random.Range(-1, 1f), 1f).normalized;
        limbRb.AddForce(force * 5f, ForceMode2D.Impulse);
    }

 
}
