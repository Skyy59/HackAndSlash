using UnityEngine;

public class HeavyEnemy : Enemy
{
    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float verticalSpeedClamp = 20f;
    private float _verticalVelocity;

    [Header("Detection")]
    [SerializeField] private Vector2 detectionCheckSize = Vector2.zero;
    [SerializeField] private Vector2 detectionCheckOffset = Vector2.zero;
    [SerializeField] private LayerMask playerLayer;

    [Header("Charge")]
    [SerializeField] private float chargeWindupDuration = 1.5f;
    [SerializeField] private float chargeSpeed = 12f;
    [SerializeField] private float chargeDistance = 8f;
    [SerializeField] private float chargeCooldown = 1.5f;
    [SerializeField] private float chargeDamage = 25f;

    [Header("GroundCheck")]
    [SerializeField] private Vector2 groundCheckSize = Vector2.zero;
    [SerializeField] private Vector2 groundCheckOffset = Vector2.zero;
    [SerializeField] private LayerMask groundLayer;

    [Header("SlopeCheck")]
    [SerializeField] private float maxSlopeAngle = 50f;
    [SerializeField] private float slopeCheckDistance = 0.6f;
    [SerializeField] private LayerMask slopeLayer;

    private float _chargeDirection = 1f;
    private float _nextChargeTime;
    private float _chargeWindupTimer;
    private Vector2 _chargeStartPosition;
    private bool _isPreparingCharge;
    private bool _isCharging;
    private bool _hasHitPlayerThisCharge;
    private bool _isGrounded;

    public bool IsCharging => _isCharging;

    protected override void Awake()
    {
        base.Awake();

        if (rb == null) rb = GetComponent<Rigidbody2D>();

        if (playerTr == null)
        {
            Player_Controller player = FindFirstObjectByType<Player_Controller>();
            if (player != null) playerTr = player.transform;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + detectionCheckOffset, attackRange);

        Gizmos.color = Color.magenta;
        Vector3 start = transform.position;
        Vector3 end = transform.position + (Vector3.down * slopeCheckDistance);
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, 0.05f);
    }

    protected override void Movement()
    {
        ApplyGravity();

        if (_isCharging)
        {
            ContinueCharge();
            return;
        }

        if (_isPreparingCharge)
        {
            PrepareCharge();
            return;
        }

        rb.linearVelocity = new Vector2(0f, _verticalVelocity);

        if (Time.time >= _nextChargeTime && PlayerDetected())
        {
            StartChargeWindup();
        }
    }

    private void ApplyGravity()
    {
        _isGrounded = Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckSize, 0f, groundLayer);

        if (_isGrounded)
        {
            if (_verticalVelocity < 0) _verticalVelocity = -0.1f;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
            _verticalVelocity = Mathf.Clamp(_verticalVelocity, -verticalSpeedClamp, 0);
        }
    }

    private bool PlayerDetected()
    {
        Vector2 checkPosition = (Vector2)transform.position + detectionCheckOffset;

        if (playerLayer.value != 0)
        {
            return Physics2D.OverlapCircle(checkPosition, attackRange, playerLayer);
        }

        if (playerTr == null) return false;

        return Vector2.Distance(checkPosition, playerTr.position) <= attackRange;
    }

    private void StartChargeWindup()
    {
        _isPreparingCharge = true;
        _chargeWindupTimer = 0f;
        _hasHitPlayerThisCharge = false;
        UpdateChargeDirection();
    }

    private void PrepareCharge()
    {
        rb.linearVelocity = new Vector2(0f, _verticalVelocity);
        UpdateChargeDirection();

        _chargeWindupTimer += Time.deltaTime;

        if (_chargeWindupTimer >= chargeWindupDuration)
        {
            StartCharge();
        }
    }

    private void UpdateChargeDirection()
    {
        if (playerTr != null)
        {
            _chargeDirection = playerTr.position.x > transform.position.x ? 1f : -1f;
        }

        FlipFacingDirection(_chargeDirection);
    }

    private void StartCharge()
    {
        _isPreparingCharge = false;
        _isCharging = true;
        _hasHitPlayerThisCharge = false;
        _chargeStartPosition = transform.position;
    }

    private void ContinueCharge()
    {
        Vector2 chargeVelocity = new Vector2(_chargeDirection * chargeSpeed, _verticalVelocity);

        if (_isGrounded && _verticalVelocity <= 0)
        {
            chargeVelocity = GetSlopeVelocity(chargeVelocity);
        }

        rb.linearVelocity = chargeVelocity;

        if (Vector2.Distance(_chargeStartPosition, transform.position) >= chargeDistance)
        {
            StopCharge();
        }
    }

    private void StopCharge()
    {
        _isPreparingCharge = false;
        _isCharging = false;
        _nextChargeTime = Time.time + chargeCooldown;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private Vector2 GetSlopeVelocity(Vector2 horizontalVelocity)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDistance, slopeLayer);

        if (hit.collider != null)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle > maxSlopeAngle) return new Vector2(0, _verticalVelocity);

            if (angle > 0.01f)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isCharging) return;

        HeavyBreakableWall breakableWall = collision.collider.GetComponentInParent<HeavyBreakableWall>();
        if (breakableWall != null)
        {
            Vector2 impactPoint = collision.contactCount > 0 ? collision.GetContact(0).point : collision.transform.position;
            breakableWall.Break(this, impactPoint);
            return;
        }

        if (IsPlayerCollision(collision.collider) && collision.collider.TryGetComponent(out IDamageable damageable))
        {
            HitDamageable(damageable);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isCharging) return;

        HeavyBreakableWall breakableWall = collision.GetComponentInParent<HeavyBreakableWall>();
        if (breakableWall != null)
        {
            breakableWall.Break(this, transform.position);
            return;
        }

        if (IsPlayerCollision(collision) && collision.TryGetComponent(out IDamageable damageable))
        {
            HitDamageable(damageable);
        }
    }

    private bool IsPlayerCollision(Collider2D collision)
    {
        if (playerLayer.value != 0)
        {
            return (playerLayer & (1 << collision.gameObject.layer)) > 0;
        }

        return collision.GetComponentInParent<Player_Health>() != null ||
            collision.GetComponentInParent<Player_Controller>() != null;
    }

    private void HitDamageable(IDamageable damageable)
    {
        if (_hasHitPlayerThisCharge) return;

        damageable.TakeDamage(chargeDamage, "Player");
        _hasHitPlayerThisCharge = true;
    }

    private void Reset()
    {
        maxHealth = 250f;
        attackRange = 12f;
        moveSpeed = 2f;
        chargeWindupDuration = 1.5f;
        chargeSpeed = 12f;
        chargeDistance = 8f;
        chargeCooldown = 1.5f;
        chargeDamage = 25f;
    }
}
