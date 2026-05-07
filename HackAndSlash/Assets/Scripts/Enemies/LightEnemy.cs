using UnityEngine;

public class LightEnemy : Enemy
{
    [Header("References")]

    [Header("Movement")]

    [Header("GroundCheck")]
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private Vector2 groundCheckOffset;
    [SerializeField] private LayerMask groundLayer;



    [Header("Attack")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    

    private float _moveDirection;
    private bool _isGrounded;

    protected override void Awake() 
    {
        base.Awake();


        _moveDirection = Random.value > 0.5f ? 1f : -1f; 

        if(rb != null) rb.gravityScale = 0;   
    }

    protected override void Start()
    {
        base.Start();
        FlipFacingDirection(_moveDirection);
    }

    protected override void Update()
    {
        if(isDead) return;
        Movement();
    }

    private void CheckGround()
    {
        Vector2 rotatedOffset = transform.rotation * groundCheckOffset;
        Vector2 checkPosition = (Vector2)transform.position * rotatedOffset;

        
    }

    protected override void Movement()
    {
        rb.linearVelocity = new Vector2(_moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    private void FlipFacingDirection(float direction)
    {
        if(direction == 0) return;

        float side = direction > 0 ? 1f : -1f;
        transform.localScale = new Vector3(side * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}