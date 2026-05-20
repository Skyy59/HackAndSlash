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

    [Header("Seismic Stomp")]
    [SerializeField] private float stompWindupDuration = 1.5f;
    [SerializeField] private int stompCount = 3;
    [SerializeField] private float stompDelay = 1.5f;
    [SerializeField] private float stompWaveSpeed = 8f;
    [SerializeField] private float stompWaveDistance = 10f;
    [SerializeField] private float stompWaveDamage = 20f;
    [SerializeField] private float stompConfusionDuration = 2.5f;
    [SerializeField] private Vector2 stompOriginOffset = new Vector2(0f, -0.5f);
    [SerializeField] private Vector2 stompWaveSize = new Vector2(1f, 0.45f);
    [SerializeField] private Color stompWaveColor = new Color(1f, 0.65f, 0.15f, 0.85f);

    [Header("GroundCheck")]
    [SerializeField] private Vector2 groundCheckSize = Vector2.zero;
    [SerializeField] private Vector2 groundCheckOffset = Vector2.zero;
    [SerializeField] private LayerMask groundLayer;

    [Header("SlopeCheck")]
    [SerializeField] private float maxSlopeAngle = 50f;
    [SerializeField] private float slopeCheckDistance = 0.6f;
    [SerializeField] private LayerMask slopeLayer;

    [Header("Animation")]
    [SerializeField] private string idleAnimation = "Iddle";
    [SerializeField] private string jumpAnimation = "Jump";
    [SerializeField] private string chargeAnimation = "Sprint";
    [SerializeField] private string stompAnimation = "Pisoton";

    private float _chargeDirection = 1f;
    private float _nextAttackTime;
    private float _chargeWindupTimer;
    private Vector2 _chargeStartPosition;
    private float _stompDirection = 1f;
    private float _stompWindupTimer;
    private float _nextStompTime;
    private int _stompsRemaining;
    private bool _isPreparingCharge;
    private bool _isCharging;
    private bool _isPreparingStomp;
    private bool _isStomping;
    private bool _hasHitPlayerThisCharge;
    private bool _isGrounded;
    private string _currentAnimation;

    public bool IsCharging => _isCharging;

    protected override void Awake()
    {
        base.Awake();

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (enemyAnimator == null) enemyAnimator = GetComponent<Animator>();
        if (enemyAnimator == null) enemyAnimator = GetComponentInChildren<Animator>();

        if (playerTr == null)
        {
            FindPlayerReference();
        }
    }

    protected override void Update()
    {
        if (isDead) return;
        if (playerTr == null) FindPlayerReference();
        if (playerTr == null)
        {
            EnemyAnimationParameters();
            return;
        }

        Movement();
        Attack();
        EnemyAnimationParameters();
    }

    private void FindPlayerReference()
    {
        Player_Controller player = FindFirstObjectByType<Player_Controller>();
        if (player != null) playerTr = player.transform;
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

        if (_isPreparingStomp)
        {
            PrepareSeismicStomp();
            return;
        }

        if (_isStomping)
        {
            ContinueSeismicStomp();
            return;
        }

        rb.linearVelocity = new Vector2(0f, _verticalVelocity);

        if (Time.time >= _nextAttackTime && PlayerDetected())
        {
            StartRandomHeavyAttack();
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
            if (Physics2D.OverlapCircle(checkPosition, attackRange, playerLayer)) return true;
        }

        if (playerTr == null) return false;

        return Vector2.Distance(checkPosition, playerTr.position) <= attackRange;
    }

    private void StartRandomHeavyAttack()
    {
        if (Random.value < 0.5f)
        {
            StartChargeWindup();
        }
        else
        {
            StartSeismicStompWindup();
        }
    }

    private void StartChargeWindup()
    {
        _isPreparingCharge = true;
        _isPreparingStomp = false;
        _isStomping = false;
        _chargeWindupTimer = 0f;
        _hasHitPlayerThisCharge = false;
        UpdateChargeDirection();
        PlayHeavyAnimation(chargeAnimation, true);
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
        _isPreparingStomp = false;
        _isStomping = false;
        _hasHitPlayerThisCharge = false;
        _chargeStartPosition = transform.position;
        PlayHeavyAnimation(chargeAnimation, true);
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
        _nextAttackTime = Time.time + chargeCooldown;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void StartSeismicStompWindup()
    {
        _isPreparingStomp = true;
        _isPreparingCharge = false;
        _isCharging = false;
        _stompWindupTimer = 0f;
        UpdateStompDirection();
        PlayHeavyAnimation(stompAnimation, true);
    }

    private void PrepareSeismicStomp()
    {
        rb.linearVelocity = new Vector2(0f, _verticalVelocity);
        UpdateStompDirection();

        _stompWindupTimer += Time.deltaTime;

        if (_stompWindupTimer >= stompWindupDuration)
        {
            StartSeismicStomp();
        }
    }

    private void StartSeismicStomp()
    {
        _isPreparingStomp = false;
        _isStomping = true;
        _isPreparingCharge = false;
        _isCharging = false;
        _stompsRemaining = Mathf.Max(1, stompCount);
        _nextStompTime = Time.time;
        PlayHeavyAnimation(stompAnimation, true);
    }

    private void ContinueSeismicStomp()
    {
        rb.linearVelocity = new Vector2(0f, _verticalVelocity);

        if (_stompsRemaining <= 0)
        {
            StopSeismicStomp();
            return;
        }

        if (Time.time < _nextStompTime) return;

        DoSeismicStomp();
        _stompsRemaining--;
        _nextStompTime = Time.time + stompDelay;

        if (_stompsRemaining <= 0)
        {
            StopSeismicStomp();
        }
    }

    private void DoSeismicStomp()
    {
        UpdateStompDirection();
        PlayHeavyAnimation(stompAnimation, true);

        Vector2 spawnPosition = (Vector2)transform.position + stompOriginOffset;
        GameObject waveObject = new GameObject("Seismic Wave");
        waveObject.transform.position = spawnPosition;

        SeismicWave seismicWave = waveObject.AddComponent<SeismicWave>();
        seismicWave.Initialize(
            _stompDirection,
            stompWaveSpeed,
            stompWaveDistance,
            stompWaveDamage,
            stompConfusionDuration,
            stompWaveSize,
            playerLayer,
            groundLayer,
            stompWaveColor);
    }

    private void StopSeismicStomp()
    {
        _isPreparingStomp = false;
        _isStomping = false;
        _nextAttackTime = Time.time + chargeCooldown;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void UpdateStompDirection()
    {
        if (playerTr != null)
        {
            _stompDirection = playerTr.position.x > transform.position.x ? 1f : -1f;
        }

        FlipFacingDirection(_stompDirection);
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

        Vector2 impactPoint = collision.contactCount > 0 ? collision.GetContact(0).point : collision.transform.position;
        if (TryBreakWall(collision.collider, impactPoint)) return;

        if (IsPlayerCollision(collision.collider) && collision.collider.TryGetComponent(out IDamageable damageable))
        {
            HitDamageable(damageable);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isCharging) return;

        if (TryBreakWall(collision, transform.position)) return;

        if (IsPlayerCollision(collision) && collision.TryGetComponent(out IDamageable damageable))
        {
            HitDamageable(damageable);
        }
    }

    private bool TryBreakWall(Collider2D collision, Vector2 impactPoint)
    {
        HeavyBreakableWall heavyBreakableWall = collision.GetComponentInParent<HeavyBreakableWall>();
        if (heavyBreakableWall != null)
        {
            heavyBreakableWall.Break(this, impactPoint);
            return true;
        }

        Breakeable_Wall breakeableWall = collision.GetComponentInParent<Breakeable_Wall>();
        if (breakeableWall != null)
        {
            breakeableWall.BreakFromHeavyCharge(this, impactPoint);
            return true;
        }

        return false;
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

    private void EnemyAnimationParameters()
    {
        if (enemyAnimator == null) return;

        if (_isPreparingCharge || _isCharging)
        {
            PlayHeavyAnimation(chargeAnimation, false);
            return;
        }

        if (_isPreparingStomp || _isStomping)
        {
            PlayHeavyAnimation(stompAnimation, false);
            return;
        }

        if (!_isGrounded)
        {
            PlayHeavyAnimation(jumpAnimation, false);
            return;
        }

        PlayHeavyAnimation(idleAnimation, false);
    }

    private void PlayHeavyAnimation(string animationName, bool restart)
    {
        if (enemyAnimator == null || string.IsNullOrEmpty(animationName)) return;
        if (!restart && _currentAnimation == animationName) return;

        enemyAnimator.Play(animationName, 0, 0f);
        _currentAnimation = animationName;
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
        stompWindupDuration = 1.5f;
        stompCount = 3;
        stompDelay = 1.5f;
        stompWaveSpeed = 8f;
        stompWaveDistance = 10f;
        stompWaveDamage = 20f;
        stompConfusionDuration = 2.5f;
        stompWaveSize = new Vector2(1f, 0.45f);
        idleAnimation = "Iddle";
        jumpAnimation = "Jump";
        chargeAnimation = "Sprint";
        stompAnimation = "Pisoton";
    }
}
