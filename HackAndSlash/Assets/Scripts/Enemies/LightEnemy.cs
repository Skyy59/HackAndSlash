using UnityEngine;

public class LightEnemy : Enemy
{

    [Header("GroundCheck")]
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private Vector2 groundCheckOffset;
    [SerializeField] private LayerMask groundLayer;

    [Header("WallCheck")]
    [SerializeField] private float sensorDistance;

    [Header("Settings")]
    [SerializeField] private float attractionForce = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float _moveDirection;
    private bool _isGrounded;
    private Quaternion _targetRotation;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(groundCheckOffset, groundCheckSize);
        Gizmos.matrix = Matrix4x4.identity;

       Gizmos.color = Color.red;
       Gizmos.DrawRay(transform.position, transform.right * _moveDirection * sensorDistance);

        
    }

    protected override void Awake()
    {
        base.Awake();
        _moveDirection = Random.value > 0.5f ? 1f : -1f;
        _targetRotation = transform.rotation;
        if (rb != null) rb.gravityScale = 0;
    }

    protected override void Update()
    {
        if (isDead) return;
        CheckGround();
        Rotation();
        

    }

    private void FixedUpdate() 
    {
        if (isDead) return;
        Movement();
    }

    private void CheckGround()
    {
        Vector2 checkPosition = (Vector2)transform.position + (Vector2)(transform.rotation * groundCheckOffset);
        _isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, transform.eulerAngles.z, groundLayer);
    }

    private void Rotation()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, transform.right * _moveDirection, sensorDistance, groundLayer);

        if(wallHit.collider != null)
        {
            _targetRotation = Quaternion.LookRotation(Vector3.forward, wallHit.normal);
        }
        else if (!_isGrounded)
        {
            transform.Rotate(0,0, -_moveDirection * 90f * Time.deltaTime * (rotationSpeed / 2f));
            _targetRotation = transform.rotation;
            return;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * rotationSpeed);
    }

    protected override void Movement()
    {
        
        rb.linearVelocity = (transform.right * (_moveDirection * moveSpeed)) + (-transform.up * attractionForce);

        FlipFacingDirection(_moveDirection);
    }

    private void FlipFacingDirection(float direction)
    {
        if (direction == 0) return;
        float side = direction > 0 ? 1f : -1f;
        transform.localScale = new Vector3(side * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}