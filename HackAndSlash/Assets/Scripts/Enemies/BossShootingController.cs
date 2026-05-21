using System.Collections;
using UnityEngine;

public class BossShootingController : MonoBehaviour
{
    private enum ShootPattern
    {
        Straight,
        Fan,
        Circle,
        Aimed,
        Burst
    }

    [Header("References")]
    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform[] shootPoints;
    [SerializeField] private GameObject normalBulletPrefab;
    [SerializeField] private GameObject specialBulletPrefab;

    [Header("Animation")]
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private string idleAnimation = "Idle";

    [Header("Detection")]
    [SerializeField] private Vector2 detectionAreaSize = new Vector2(18f, 12f);
    [SerializeField] private Vector2 detectionAreaOffset = new Vector2(-9f, 0.5f);
    [SerializeField] private LayerMask playerLayer;

    [Header("General Shooting")]
    [SerializeField] private bool randomPatterns = true;
    [SerializeField] private float patternDuration = 4f;
    [SerializeField] private float delayBetweenPatterns = 0.4f;
    [SerializeField] private float bulletSpeed = 7f;
    [SerializeField] private float bulletDamage = 10f;
    [SerializeField] private float bulletLifeTime = 5f;
    [SerializeField, Range(0f, 1f)] private float specialBulletChance = 0.08f;
    [SerializeField] private float specialBulletSpeed = 5f;
    [SerializeField] private float specialBulletDamage = 15f;
    [SerializeField] private LayerMask bulletImpactLayer;

    [Header("Straight Pattern")]
    [SerializeField] private Vector2 straightDirection = Vector2.left;
    [SerializeField] private float straightFireRate = 0.18f;

    [Header("Fan Pattern")]
    [SerializeField] private int fanBulletCount = 7;
    [SerializeField] private float fanAngle = 70f;
    [SerializeField] private float fanFireRate = 0.45f;

    [Header("Circle Pattern")]
    [SerializeField] private int circleBulletCount = 18;
    [SerializeField] private float circleFireRate = 0.7f;

    [Header("Aimed Pattern")]
    [SerializeField] private float aimedFireRate = 0.16f;
    [SerializeField] private float aimedSpreadAngle = 8f;

    [Header("Burst Pattern")]
    [SerializeField] private int burstBulletCount = 5;
    [SerializeField] private float burstShotDelay = 0.06f;
    [SerializeField] private float burstDelay = 0.55f;

    private int _patternIndex;
    private Coroutine _shootRoutine;
    private string _currentAnimation;

    private void Awake()
    {
        if (bossAnimator == null) bossAnimator = GetComponent<Animator>();
        if (bossAnimator == null) bossAnimator = GetComponentInChildren<Animator>();

        if (playerTr == null)
        {
            Player_Controller player = FindFirstObjectByType<Player_Controller>();
            if (player != null) playerTr = player.transform;
        }

        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            rb2D.bodyType = RigidbodyType2D.Kinematic;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
        }

        PlayIdleAnimation();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + detectionAreaOffset, detectionAreaSize);
    }

    private void Update()
    {
        PlayIdleAnimation();

        if (PlayerDetected())
        {
            StartShooting();
        }
        else
        {
            StopShooting();
        }
    }

    private void OnDisable()
    {
        StopShooting();
    }

    private bool PlayerDetected()
    {
        Vector2 checkPosition = (Vector2)transform.position + detectionAreaOffset;

        if (playerLayer.value != 0)
        {
            return Physics2D.OverlapBox(checkPosition, detectionAreaSize, 0f, playerLayer);
        }

        if (playerTr == null) return false;

        Vector2 delta = (Vector2)playerTr.position - checkPosition;
        return Mathf.Abs(delta.x) <= detectionAreaSize.x * 0.5f &&
            Mathf.Abs(delta.y) <= detectionAreaSize.y * 0.5f;
    }

    private void StartShooting()
    {
        if (_shootRoutine != null) return;

        _shootRoutine = StartCoroutine(ShootingLoop());
    }

    private void StopShooting()
    {
        if (_shootRoutine == null) return;

        StopCoroutine(_shootRoutine);
        _shootRoutine = null;
    }

    private IEnumerator ShootingLoop()
    {
        while (enabled)
        {
            ShootPattern pattern = GetNextPattern();
            yield return StartCoroutine(RunPattern(pattern));
            yield return new WaitForSeconds(delayBetweenPatterns);
        }
    }

    private ShootPattern GetNextPattern()
    {
        int patternCount = System.Enum.GetValues(typeof(ShootPattern)).Length;

        if (randomPatterns)
        {
            return (ShootPattern)Random.Range(0, patternCount);
        }

        ShootPattern pattern = (ShootPattern)_patternIndex;
        _patternIndex = (_patternIndex + 1) % patternCount;
        return pattern;
    }

    private IEnumerator RunPattern(ShootPattern pattern)
    {
        float endTime = Time.time + patternDuration;

        while (Time.time < endTime)
        {
            switch (pattern)
            {
                case ShootPattern.Straight:
                    FireStraight();
                    yield return new WaitForSeconds(straightFireRate);
                    break;
                case ShootPattern.Fan:
                    FireFan();
                    yield return new WaitForSeconds(fanFireRate);
                    break;
                case ShootPattern.Circle:
                    FireCircle();
                    yield return new WaitForSeconds(circleFireRate);
                    break;
                case ShootPattern.Aimed:
                    FireAimed();
                    yield return new WaitForSeconds(aimedFireRate);
                    break;
                case ShootPattern.Burst:
                    yield return StartCoroutine(FireBurst());
                    yield return new WaitForSeconds(burstDelay);
                    break;
            }
        }
    }

    private void FireStraight()
    {
        FireFromAllPoints(straightDirection);
    }

    private void FireFan()
    {
        Transform shootPoint = GetShootPoint(0);
        if (shootPoint == null) return;

        int count = Mathf.Max(1, fanBulletCount);
        float startAngle = -fanAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float progress = count == 1 ? 0.5f : i / (float)(count - 1);
            float angle = startAngle + fanAngle * progress;
            FireBullet(shootPoint.position, RotateDirection(straightDirection, angle));
        }
    }

    private void FireCircle()
    {
        Transform shootPoint = GetShootPoint(0);
        if (shootPoint == null) return;

        int count = Mathf.Max(1, circleBulletCount);
        for (int i = 0; i < count; i++)
        {
            float angle = 360f * (i / (float)count);
            FireBullet(shootPoint.position, RotateDirection(Vector2.right, angle));
        }
    }

    private void FireAimed()
    {
        Transform shootPoint = GetShootPoint(0);
        if (shootPoint == null || playerTr == null) return;

        Vector2 direction = (playerTr.position - shootPoint.position).normalized;
        direction = RotateDirection(direction, Random.Range(-aimedSpreadAngle, aimedSpreadAngle));
        FireBullet(shootPoint.position, direction);
    }

    private IEnumerator FireBurst()
    {
        Transform shootPoint = GetShootPoint(0);
        if (shootPoint == null) yield break;

        int count = Mathf.Max(1, burstBulletCount);
        for (int i = 0; i < count; i++)
        {
            Vector2 direction = playerTr != null ? (playerTr.position - shootPoint.position).normalized : straightDirection;
            FireBullet(shootPoint.position, direction);
            yield return new WaitForSeconds(burstShotDelay);
        }
    }

    private void FireFromAllPoints(Vector2 direction)
    {
        if (shootPoints == null || shootPoints.Length == 0)
        {
            FireBullet(transform.position, direction);
            return;
        }

        for (int i = 0; i < shootPoints.Length; i++)
        {
            if (shootPoints[i] == null) continue;
            FireBullet(shootPoints[i].position, direction);
        }
    }

    private Transform GetShootPoint(int index)
    {
        if (shootPoints == null || shootPoints.Length == 0) return transform;

        index = Mathf.Clamp(index, 0, shootPoints.Length - 1);
        return shootPoints[index] != null ? shootPoints[index] : transform;
    }

    private void FireBullet(Vector2 position, Vector2 direction)
    {
        bool useSpecialBullet = specialBulletPrefab != null && Random.value <= specialBulletChance;
        GameObject prefab = useSpecialBullet ? specialBulletPrefab : normalBulletPrefab;
        if (prefab == null) return;

        GameObject bulletObject = Instantiate(prefab, position, Quaternion.identity);
        BossBullet bullet = bulletObject.GetComponent<BossBullet>();
        if (bullet == null) return;

        float speed = useSpecialBullet ? specialBulletSpeed : bulletSpeed;
        float damage = useSpecialBullet ? specialBulletDamage : bulletDamage;
        bullet.Launch(direction, speed, damage, bulletLifeTime, bulletImpactLayer);
    }

    private Vector2 RotateDirection(Vector2 direction, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        return rotation * direction.normalized;
    }

    private void Reset()
    {
        patternDuration = 4f;
        delayBetweenPatterns = 0.4f;
        detectionAreaSize = new Vector2(18f, 12f);
        detectionAreaOffset = new Vector2(-9f, 0.5f);
        bulletSpeed = 7f;
        bulletDamage = 10f;
        bulletLifeTime = 5f;
        specialBulletChance = 0.08f;
        specialBulletSpeed = 5f;
        specialBulletDamage = 15f;
        straightDirection = Vector2.left;
        straightFireRate = 0.18f;
        fanBulletCount = 7;
        fanAngle = 70f;
        fanFireRate = 0.45f;
        circleBulletCount = 18;
        circleFireRate = 0.7f;
        aimedFireRate = 0.16f;
        aimedSpreadAngle = 8f;
        burstBulletCount = 5;
        burstShotDelay = 0.06f;
        burstDelay = 0.55f;
        idleAnimation = "Idle";
    }

    private void PlayIdleAnimation()
    {
        if (bossAnimator == null || string.IsNullOrEmpty(idleAnimation)) return;
        if (_currentAnimation == idleAnimation && bossAnimator.GetCurrentAnimatorStateInfo(0).IsName(idleAnimation)) return;

        bossAnimator.Play(idleAnimation, 0, 0f);
        _currentAnimation = idleAnimation;
    }
}
