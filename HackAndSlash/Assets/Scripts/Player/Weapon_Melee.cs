using System;
using UnityEngine;

public class Weapon_Melee : Weapon
{
    [SerializeField] private Vector2 meleeOffset;
    [SerializeField] private Vector2 meleeSize;
    [SerializeField] private LayerMask meleeLayer;

    [SerializeField] private int attackPerSwing;

    [SerializeField] private float rateOfFire;
    [SerializeField] private bool triggerCanBeHeld;
    private bool _triggerAlreadyPressed;
    private float _rpm;
    private float _nextFireTimer;
    private bool _isFiring;

    [SerializeField] private float totalBoost;
    [SerializeField] private float currentBoost;

    [SerializeField] private float damage;

    [Header("Upgrades")]
    [SerializeField] private bool upgrade1;
    [SerializeField] private bool upgrade2;
    [SerializeField] private bool upgrade3;

    private bool _canFire = false;

    private int _currentLevel;

    public static Action<float, float> OnRequestHUD;
    public Action<float> OnHitRefill;

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;

        if (ReturnDirection != null) Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(meleeOffset.x * ReturnDirection(), meleeOffset.y), meleeSize);    
    }

    private void Start() 
    {
        OnRequestHUD?.Invoke(currentBoost, totalBoost);
        _rpm = 60f / rateOfFire;
    }

    private void Update() 
    {
        Attack(requestAttack);
        ReduceMeter();
    }

    private void LateUpdate() 
    {
        _rpm = 60f / rateOfFire;
    }

    public override void Attack(bool _state)
    {
        if (!_canFire) return;

        _isFiring = _state;
        if (_isFiring)
        {
            if (Time.time >= _nextFireTimer) Fire();
        }
        else
        {
            _triggerAlreadyPressed = false;
        }
    }

    private void Fire()
    {
        if (!triggerCanBeHeld && _triggerAlreadyPressed) return;

        for (int _i = 0; _i < attackPerSwing; _i++)
        {
            Swing();
        }

        _nextFireTimer = Time.time + _rpm;

        _triggerAlreadyPressed = true;

        OnHitRefill?.Invoke(0.1f);
        OnAttackAnimation?.Invoke("Melee");

        void Swing()
        {
            Collider2D[] _hits = MeleeAttack();

            for (int _i = 0; _i < _hits.Length; _i++)
            {
                
                 if(_hits[_i].TryGetComponent(out IDamageable _damageable)) _damageable.TakeDamage(currentBoost > 0 ? damage * 1.5f : damage, "Melee");
            }
        }
    }

    private Collider2D[] MeleeAttack()
    {
        return Physics2D.OverlapBoxAll((Vector2)transform.position + new Vector2(meleeOffset.x * ReturnDirection(), meleeOffset.y), meleeSize, 0f, meleeLayer);
    }

    public override void Switch(bool _state)
    {
        _canFire = _state;
        if (_state) Upgrades();
        base.Switch(_state);
    }

    public override void AddToMeter(float _amount)
    {
        float _actualAmount = totalBoost * _amount;
        currentBoost += _actualAmount;

        currentBoost = Mathf.Clamp(currentBoost, 0, totalBoost);

        OnRequestHUD?.Invoke(currentBoost, totalBoost);
    }

    private void ReduceMeter()
    {
        if (currentBoost < 0 || !_canFire) return;

        currentBoost -= Time.deltaTime;

        currentBoost = Mathf.Clamp(currentBoost, 0, totalBoost);

        OnRequestHUD?.Invoke(currentBoost, totalBoost);
    }

    public void AddUpgrades()
    {
        _currentLevel++;
        Upgrades();
    }

    public override void Upgrades()
    {
        bool _previousState1 = upgrade1;
        bool _previousState2 = upgrade2;
        bool _previousState3 = upgrade3;

        if (!_previousState1) upgrade1 = _currentLevel > 0;
        if (!_previousState2) upgrade2 = _currentLevel > 1;
        if (!_previousState3) upgrade3 = _currentLevel > 2;

        if (upgrade1 && !_previousState1)
        {
            meleeSize.x += 0.5f;
            meleeOffset.x += 0.25f;
            damage += 5;
        }

        if (upgrade2 && !_previousState2)
        {
            meleeSize.x += 0.5f;
            meleeOffset.x += 0.25f;
            damage += 5;
        }

        if (upgrade3 && !_previousState3)
        {
            attackPerSwing = 2;
            meleeSize.x += 1f;
            meleeOffset.x += 0.5f;
            damage += 10;
        }
    }
}
