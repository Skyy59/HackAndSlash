using UnityEngine;

public class BossBouncingBullet : BossBullet
{
    private const float SpecialLifeTime = 10f;

    [Header("Special Visual")]
    [SerializeField] private Color specialColor = new Color(1f, 0.25f, 0.1f, 1f);

    protected override void Awake()
    {
        base.Awake();

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.color = specialColor;
    }

    protected override void OnEnable()
    {
        Invoke(nameof(DestroyBullet), SpecialLifeTime);
    }

    protected override void FixedUpdate()
    {
        if (rb2D.linearVelocity.sqrMagnitude < 0.01f)
        {
            rb2D.linearVelocity = moveDirection.normalized * bulletSpeed;
        }
    }

    public override void Launch(Vector2 direction, float speed, float damage, float duration, LayerMask layerMask)
    {
        base.Launch(direction, speed, damage, SpecialLifeTime, layerMask);
        rb2D.linearVelocity = moveDirection.normalized * bulletSpeed;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (HitsImpactLayer(collision.gameObject.layer))
        {
            DamageTarget(collision.collider);
        }

        if (collision.contactCount == 0) return;

        Vector2 normal = collision.GetContact(0).normal;
        Vector2 reflectedDirection = Vector2.Reflect(rb2D.linearVelocity.normalized, normal);
        moveDirection = reflectedDirection.sqrMagnitude > 0.01f ? reflectedDirection.normalized : -moveDirection;
        rb2D.linearVelocity = moveDirection * bulletSpeed;
    }
}
