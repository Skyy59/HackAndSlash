using UnityEngine;

public class Breakeable_Wall : MonoBehaviour, IDamageable
{
    [Header("Wall Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Effects")]
    [SerializeField] private GameObject destructParticle;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, string key)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            BreakWall();
        }
    }

    private void BreakWall()
    {
        if(destructParticle != null)
        {
            //TODO: DestructionWall particle
            //Instantiate(destructParticle, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
