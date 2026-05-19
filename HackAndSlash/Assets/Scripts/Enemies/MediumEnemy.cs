using UnityEngine;
using System.Collections;

public class MediumEnemy : Enemy
{

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float verticalSpeedClamp = 20f;
    private float _verticalVelocity;


    [Header("Chase RangeCheck")]
    [SerializeField] private Vector2 chaseCheckSize = Vector2.zero;
    [SerializeField] private Vector2 chaseCheckOffset = Vector2.zero;
    
    [Header("Attack RangeCheck")]
    [SerializeField] private Vector2 attackCheckSize = Vector2.zero;
    [SerializeField] private Vector2 attackCheckOffset = Vector2.zero;
    [SerializeField] private LayerMask playerLayer;


    [Header("AttackPoint")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 attackSize = Vector2.zero;
    [SerializeField] private float meleeDamage = 10f;
    [SerializeField] private float damageDelay = 1.5f;

    [Header("Acceleration")]
    [SerializeField] private float acceleration = 5f;
    private float _currentHorizontalVelocity;

    [Header("GroundCheck")]
    [SerializeField] private Vector2 groundCheckSize = Vector2.zero;
    [SerializeField] private Vector2 groundCheckOffset = Vector2.zero;
    [SerializeField] private LayerMask groundLayer;

    [Header("SlopeCheck")]
    [SerializeField] private float maxSlopeAngle = 50f;
    [SerializeField] private float slopeCheckDistance = 0.6f;
    [SerializeField] private LayerMask slopeLayer;

    
    private float _targetVelocity;
    private bool _isGrounded;
    private bool _playerInChaseRange = false;
    private bool _playerInAttackRange = false;
    private bool _isAttacking;



    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + chaseCheckOffset, chaseCheckSize);

        Gizmos.color = Color.violet;
        Gizmos.DrawWireCube((Vector2)transform.position + attackCheckOffset, attackCheckSize);

        Gizmos.color = Color.magenta; 
        Vector3 start = transform.position;
        Vector3 end = transform.position + (Vector3.down * slopeCheckDistance);

        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, 0.05f);

        if (attackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(attackPoint.position, attackSize);
        }

        
    }

    protected override void Update()
    {
        base.Update();

        EnemyAnimationParameters();
    }

    protected override void Movement()
    {
        _isGrounded = Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckSize, 0f, groundLayer); 

        _playerInChaseRange = Physics2D.OverlapBox((Vector2)transform.position + chaseCheckOffset, chaseCheckSize, 0f, playerLayer);

        _playerInAttackRange = Physics2D.OverlapBox((Vector2)transform.position + attackCheckOffset, attackCheckSize, 0f, playerLayer);


        if (_isGrounded)
        {
            if(_verticalVelocity < 0) _verticalVelocity = -0.1f;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
            _verticalVelocity = Mathf.Clamp(_verticalVelocity, -verticalSpeedClamp, 0);
        }

        if (_isAttacking)
        {
            _targetVelocity = 0;
        }
        else if (_playerInAttackRange)
        {
            _targetVelocity = 0;
        }
        else if (_playerInChaseRange)
        {
            float direction = playerTr.position.x > transform.position.x ? 1f : -1f;
            _targetVelocity = direction * moveSpeed;
            FlipFacingDirection(_targetVelocity);
        }
        else
        {
            _targetVelocity = 0;
        }

        _currentHorizontalVelocity = Mathf.MoveTowards(rb.linearVelocity.x, _targetVelocity, acceleration * Time.deltaTime);
        Vector2 finalVelocity = new Vector2(_currentHorizontalVelocity, _verticalVelocity);

        if(_isGrounded && _verticalVelocity <= 0)
        {
            finalVelocity = GetSlopeVelocity(finalVelocity);
        }

        rb.linearVelocity = finalVelocity; 

    }

    private Vector2 GetSlopeVelocity(Vector2 horizontalVelocity)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDistance, slopeLayer);

        if(hit.collider != null)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if(angle > maxSlopeAngle) return new Vector2(0, _verticalVelocity);

            if(angle > 0.01f)
            {
                Vector2 slopeDir = Vector2.Perpendicular(hit.normal);
                return slopeDir * -horizontalVelocity.x;
            }
        }

        return horizontalVelocity;
        
    }

    private void FlipFacingDirection(float targetDir)
    {
       if (targetDir == 0) return;
       float side = targetDir > 0 ? 1f : -1f;
       transform.localScale = new Vector3(side * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    protected override void Attack()
    {
        if (_playerInAttackRange && !_isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        _isAttacking = true;

        if(enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(damageDelay);

        if(isDead) yield break;

        PerformAttack();

        yield return new WaitForSeconds(0.2f);
        _isAttacking = false;
    }


    private void PerformAttack()
    {
       if(attackPoint == null) return;

       Collider2D hitPlayer = Physics2D.OverlapBox(attackPoint.position, attackCheckSize, 0f, playerLayer);

        if (hitPlayer != null)
        {
            if(hitPlayer.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TakeDamage(meleeDamage, "Player");
                Debug.Log("Attack");
            }
        }
    }


    private void EnemyAnimationParameters()
    {
        enemyAnimator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        enemyAnimator.SetBool("Grounded", _isGrounded);
        enemyAnimator.SetFloat("HP", currentHealth);

    }

}
