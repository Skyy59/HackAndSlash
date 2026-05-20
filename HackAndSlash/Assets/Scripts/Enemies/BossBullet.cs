using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Rigidbody2D rb2D;

    [Header("Stats")]
    [SerializeField] protected float bulletSpeed = 10f;
    [SerializeField] protected float bulletDamage = 10f;
    [SerializeField] protected float lifeTime = 4f;
    [SerializeField] protected LayerMask impactLayer;

    protected Vector2 moveDirection = Vector2.left;

    protected virtual void Awake()
    {
        if (rb2D == null) rb2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable()
    {
        Invoke(nameof(DestroyBullet), lifeTime);
    }

    protected virtual void FixedUpdate()
    {
        rb2D.linearVelocity = moveDirection.normalized * bulletSpeed;
    }

    public virtual void Launch(Vector2 direction, float speed, float damage, float duration, LayerMask layerMask)
    {
        moveDirection = direction.sqrMagnitude > 0.01f ? direction.normalized : Vector2.left;
        bulletSpeed = speed;
        bulletDamage = damage;
        lifeTime = duration;
        impactLayer = layerMask;

        CancelInvoke(nameof(DestroyBullet));
        Invoke(nameof(DestroyBullet), lifeTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!HitsImpactLayer(collision.gameObject.layer)) return;

        DamageTarget(collision);
        DestroyBullet();
    }

    protected bool HitsImpactLayer(int layer)
    {
        return (impactLayer & (1 << layer)) > 0;
    }

    protected void DamageTarget(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(bulletDamage, "Player");
        }
    }

    protected void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
