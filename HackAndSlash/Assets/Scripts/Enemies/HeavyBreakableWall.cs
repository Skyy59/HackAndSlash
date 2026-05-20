using UnityEngine;

public class HeavyBreakableWall : MonoBehaviour
{
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private bool destroyWholeObject = false;

    private bool _isBroken;

    public void Break(HeavyEnemy heavyEnemy)
    {
        Break(heavyEnemy, transform.position);
    }

    public void Break(HeavyEnemy heavyEnemy, Vector2 impactPoint)
    {
        if (_isBroken || heavyEnemy == null || !heavyEnemy.IsCharging) return;

        _isBroken = true;

        if (breakEffect != null)
        {
            Instantiate(breakEffect, impactPoint, transform.rotation);
        }

        if (destroyWholeObject)
        {
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HeavyEnemy heavyEnemy = collision.collider.GetComponentInParent<HeavyEnemy>();
        Vector2 impactPoint = collision.contactCount > 0 ? collision.GetContact(0).point : collision.transform.position;
        Break(heavyEnemy, impactPoint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HeavyEnemy heavyEnemy = collision.GetComponentInParent<HeavyEnemy>();
        Break(heavyEnemy, collision.transform.position);
    }
}
