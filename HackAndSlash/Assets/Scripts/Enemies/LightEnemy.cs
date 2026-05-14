using UnityEngine;

public class LightEnemy : Enemy
{

    [Header("GroundCheck")]
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private Vector2 groundCheckOffset;
    [SerializeField] private LayerMask groundLayer;

    [Header("WallCheck")]
    [SerializeField] private float attractionForce = 5f;
    [SerializeField] private float detectionRange = 1f;

    [Header("Settings")]

    [Header("Attack")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float _moveDirection;
    private bool _isGrounded;
    private bool _hasRotatedThisFrame;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(groundCheckOffset, groundCheckSize);

        Vector2 frontOffsetLocal = new Vector2((groundCheckSize.x / 2f + 0.1f) * _moveDirection, groundCheckOffset.y + 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(frontOffsetLocal, new Vector2(0.1f, 0.1f));

        Gizmos.matrix = Matrix4x4.identity;
    }

    protected override void Awake()
    {
        base.Awake();
        _moveDirection = Random.value > 0.5f ? 1f : -1f;
        if (rb != null) rb.gravityScale = 0;
    }

    protected override void Update()
    {
        if (isDead) return;
        CheckGround();
        Movement();
        EnemyAnimationParameters();


        _hasRotatedThisFrame = false;
    }

    private void CheckGround()
    {
        Vector2 rotatedOffset = transform.rotation * groundCheckOffset;
        Vector2 checkPosition = (Vector2)transform.position + rotatedOffset;
        _isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, transform.eulerAngles.z, groundLayer);
    }

    protected override void Movement()
    {

        Vector2 frontOffsetLocal = new Vector2((groundCheckSize.x / 2f + 0.1f) * _moveDirection, groundCheckOffset.y + 0.2f);
        Vector2 rotatedFrontOffset = transform.rotation * frontOffsetLocal;
        Vector2 frontPos = (Vector2)transform.position + rotatedFrontOffset;

        bool isWallInFront = Physics2D.OverlapBox(frontPos, new Vector2(0.1f, 0.1f), transform.eulerAngles.z, groundLayer);
        RaycastHit2D nearSurface = Physics2D.Raycast(transform.position, -transform.up, detectionRange, groundLayer);


        if (!_hasRotatedThisFrame)
        {
            if (isWallInFront)
            {
                transform.Rotate(0, 0, _moveDirection * 90f);
                rb.position += (Vector2)transform.up * 0.15f;
                _hasRotatedThisFrame = true;
            }
            else if (!_isGrounded && nearSurface.collider != null)
            {
                transform.Rotate(0, 0, _moveDirection * -90f);
                rb.position += (Vector2)(-transform.up) * 0.2f;
                _hasRotatedThisFrame = true;
            }
        }

        
        if (!_isGrounded && nearSurface.collider == null)
        {
            rb.linearVelocity = new Vector2(0, -attractionForce);
            transform.rotation = Quaternion.identity;
        }
        else
        {
            rb.linearVelocity = (transform.right * (_moveDirection * moveSpeed)) + (-transform.up * attractionForce);
        }

        FlipFacingDirection(_moveDirection);
    }

    private void FlipFacingDirection(float direction)
    {
        if (direction == 0) return;
        float side = direction > 0 ? 1f : -1f;
        transform.localScale = new Vector3(side * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }


    private void EnemyAnimationParameters()
    {
        enemyAnimator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
       

    }
}