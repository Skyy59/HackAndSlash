using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float projectileVelocity;
    [SerializeField] private float projectileDamage;
    [SerializeField] private LayerMask impactLayer;

    private void FixedUpdate() 
    {
        rb2D.linearVelocity = transform.up * projectileVelocity;   
    }

    public void LaunchProjectile(float _damage)
    {
        projectileDamage = _damage;
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        
        if((impactLayer & (1 << collision.gameObject.layer)) > 0)
        {
            Debug.Log("Impacto " + collision.name);
            if (collision.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TakeDamage(projectileDamage, "Range");

            }

            Destroy(gameObject);
        }
     

    }
}
