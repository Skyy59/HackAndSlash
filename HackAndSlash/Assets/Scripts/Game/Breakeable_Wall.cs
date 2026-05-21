using System.Collections;
using UnityEngine;

public class Breakeable_Wall : MonoBehaviour, IDamageable
{
    [Header("Wall Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Effects")]
    [SerializeField] private GameObject destructParticle;
    [SerializeField] private Material breakEffectMaterial;
    [SerializeField] private float breakEffectDuration = 0.35f;
    [SerializeField] private string breakAmountProperty = "_BreakAmount";

    private SpriteRenderer _spriteRenderer;
    private Collider2D _wallCollider;
    private bool _isBroken;

    private void Awake()
    {
        currentHealth = maxHealth;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _wallCollider = GetComponent<Collider2D>();
    }

    public void TakeDamage(float damage, string key)
    {
        if (_isBroken) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            BreakWall(transform.position);
        }
    }

    public void BreakFromHeavyCharge(HeavyEnemy heavyEnemy, Vector2 impactPoint)
    {
        if (_isBroken || heavyEnemy == null || !heavyEnemy.IsCharging) return;

        BreakWall(impactPoint);
    }

    private void BreakWall(Vector2 impactPoint)
    {
        if (_isBroken) return;

        _isBroken = true;

        if (destructParticle != null)
        {
            //TODO: DestructionWall particle
            //Instantiate(destructParticle, impactPoint, Quaternion.identity);
        }

        if (_wallCollider != null)
        {
            _wallCollider.enabled = false;
        }

        if (_spriteRenderer != null && breakEffectMaterial != null)
        {
            StartCoroutine(BreakEffectSequence());
            return;
        }

        Destroy(gameObject);
    }

    private IEnumerator BreakEffectSequence()
    {
        Material runtimeMaterial = new Material(breakEffectMaterial);
        _spriteRenderer.material = runtimeMaterial;

        float elapsedTime = 0f;
        runtimeMaterial.SetFloat(breakAmountProperty, 0f);

        while (elapsedTime < breakEffectDuration)
        {
            elapsedTime += Time.deltaTime;
            float breakAmount = Mathf.Clamp01(elapsedTime / breakEffectDuration);
            runtimeMaterial.SetFloat(breakAmountProperty, breakAmount);
            yield return null;
        }

        Destroy(runtimeMaterial);
        Destroy(gameObject);
    }
}
