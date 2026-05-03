using UnityEngine;

public class MediumEnemy : Enemy
{
    [Header("References")]



    [Header("AttackOverlap")]
    [SerializeField] private Transform rangeCheck;
    [SerializeField] private float rangeCheckRadius;
    [SerializeField] private LayerMask playerLayer;

    [Header("AttackPoint")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private float meleeDamage = 10f;

    [Header("Acceleration")]
    [SerializeField] private float acceleration = 5f;
    private float _currentHorizontalVelocity;

    private bool _isInRange = false;
    private float _targetVelocity;



    private void OnDrawGizmos() 
    {
        if(rangeCheck != null)
        {
           Gizmos.color = Color.yellow;
           Gizmos.DrawWireSphere(rangeCheck.position, rangeCheckRadius); 
        }

        if(attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, rangeCheckRadius);
        }
    }

    protected override void Movement()
    {
        _isInRange = Physics2D.OverlapCircle(rangeCheck.position, rangeCheckRadius, playerLayer);
        
        if (!_isInRange)
        {
            float direction = playerTr.position.x > transform.position.x ? 1f : -1f;
            _targetVelocity = direction * moveSpeed;

            FlipFacingDirection(rb.linearVelocity.x);
        }

        _currentHorizontalVelocity = Mathf.MoveTowards(rb.linearVelocity.x, _targetVelocity, acceleration * Time.deltaTime);
        rb.linearVelocity = new Vector2(_currentHorizontalVelocity, rb.linearVelocity.y);      
    }

    private void FlipFacingDirection(float horizontalVelocity)
    {
        if(Mathf.Abs(horizontalVelocity) < 0.1f) return;

        if(horizontalVelocity > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if(horizontalVelocity < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    protected override void Attack()
    {
        if(_isInRange && Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }


    private void PerformAttack()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (hitPlayer != null)
        {
            if(hitPlayer.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TakeDamage(meleeDamage);
            }
        }
    }

}
