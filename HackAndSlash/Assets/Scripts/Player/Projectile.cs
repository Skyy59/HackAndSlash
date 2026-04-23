using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float projectileVelocity;
    [SerializeField] private float projectileDamage;

    private void FixedUpdate() 
    {
        rb2D.linearVelocity = transform.up * projectileVelocity;   
    }

    public void LaunchProjectile(float _damage)
    {
        projectileDamage = _damage;
    }
}
