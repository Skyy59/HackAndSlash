using UnityEngine;

public class LightEnemy : Enemy
{
    [Header("References")]

    [Header("Movement")]
    [SerializeField] private float rayDistance = 0.8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float walkDuration = 3f;
    [SerializeField] private float stopDuration = 1.5f;



    [Header("Attack")]
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private Transform firePoint;
    

    private Vector2 _moveDirection;
    private float _timer;
    private bool _isWalking = true;
    private float randomX;

    protected override void Start()
    {
        base.Start();

        randomX = Random.value > 0.5f ? 1f : -1f;
        _moveDirection  = new Vector2(randomX, 0).normalized;

        _timer = walkDuration;
    }

    protected override void Movement()
    {
        _timer -= Time.deltaTime;

        if (_timer <= Mathf.Epsilon)
        {
            _isWalking = !_isWalking;

            _timer = _isWalking ? walkDuration : stopDuration;

            if (!_isWalking) rb.linearVelocity = Vector2.zero;
        }

        if(_isWalking)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * _moveDirection.x, rayDistance, groundLayer);
            
            if(hit.collider == null)
            {
                hit = Physics2D.Raycast(transform.position, -transform.up, rayDistance, groundLayer);
            }

            if(hit.collider  != null)
            {
                Quaternion targetRotation = Quaternion.FromToRotation(Vector2.up, hit.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            rb.linearVelocity = transform.right * _moveDirection.x * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            LookAtPlayer();
        }
        
    }

    private void LookAtPlayer()
    {
        if(playerTr == null) return;

        Vector2 dir = (playerTr.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}
