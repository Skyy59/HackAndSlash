using UnityEngine;

public class SeismicWave : MonoBehaviour
{
    private const int WavePoints = 9;

    private float _direction = 1f;
    private float _speed;
    private float _maxDistance;
    private float _damage;
    private float _confusionDuration;
    private Vector2 _size;
    private LayerMask _playerLayer;
    private LayerMask _groundLayer;
    private Vector2 _startPosition;
    private LineRenderer _lineRenderer;
    private bool _hasHitPlayer;

    public void Initialize(float direction, float speed, float maxDistance, float damage, float confusionDuration, Vector2 size, LayerMask playerLayer, LayerMask groundLayer, Color color)
    {
        _direction = direction >= 0f ? 1f : -1f;
        _speed = speed;
        _maxDistance = maxDistance;
        _damage = damage;
        _confusionDuration = confusionDuration;
        _size = size;
        _playerLayer = playerLayer;
        _groundLayer = groundLayer;
        _startPosition = transform.position;

        CreateCollision();
        CreateVisual(color);
        StickToGround();
    }

    private void Update()
    {
        transform.position += Vector3.right * (_direction * _speed * Time.deltaTime);
        StickToGround();
        AnimateVisual();

        if (Vector2.Distance(_startPosition, transform.position) >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void CreateCollision()
    {
        Rigidbody2D waveRb = gameObject.AddComponent<Rigidbody2D>();
        waveRb.bodyType = RigidbodyType2D.Kinematic;
        waveRb.gravityScale = 0f;
        waveRb.freezeRotation = true;

        BoxCollider2D waveCollider = gameObject.AddComponent<BoxCollider2D>();
        waveCollider.isTrigger = true;
        waveCollider.size = _size;
    }

    private void CreateVisual(Color color)
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.positionCount = WavePoints;
        _lineRenderer.startWidth = 0.12f;
        _lineRenderer.endWidth = 0.12f;
        _lineRenderer.sortingOrder = 20;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;

        AnimateVisual();
    }

    private void AnimateVisual()
    {
        if (_lineRenderer == null) return;

        float halfWidth = Mathf.Max(0.1f, _size.x * 0.5f);
        float waveHeight = Mathf.Max(0.1f, _size.y * 0.5f);
        float timeOffset = Time.time * 10f;

        for (int i = 0; i < WavePoints; i++)
        {
            float progress = i / (float)(WavePoints - 1);
            float x = Mathf.Lerp(-halfWidth, halfWidth, progress);
            float y = Mathf.Sin((progress * Mathf.PI * 4f) + timeOffset) * waveHeight;
            _lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    private void StickToGround()
    {
        if (_groundLayer.value == 0) return;

        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 1.5f;
        RaycastHit2D groundHit = Physics2D.Raycast(rayOrigin, Vector2.down, 4f, _groundLayer);
        if (groundHit.collider == null) return;

        transform.position = new Vector3(transform.position.x, groundHit.point.y + (_size.y * 0.5f), transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasHitPlayer || !IsPlayerCollision(collision)) return;

        IDamageable damageable = collision.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage, "Player");
        }

        Player_Controller playerController = collision.GetComponentInParent<Player_Controller>();
        if (playerController != null)
        {
            playerController.ApplyConfusion(_confusionDuration);
        }

        _hasHitPlayer = true;
        Destroy(gameObject);
    }

    private bool IsPlayerCollision(Collider2D collision)
    {
        if (_playerLayer.value != 0)
        {
            return (_playerLayer & (1 << collision.gameObject.layer)) > 0;
        }

        return collision.GetComponentInParent<Player_Health>() != null ||
            collision.GetComponentInParent<Player_Controller>() != null;
    }
}
