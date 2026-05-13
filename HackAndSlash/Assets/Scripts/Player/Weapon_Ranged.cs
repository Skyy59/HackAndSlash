using System;
using UnityEngine;

public class Weapon_Ranged : Weapon
{
    [SerializeField] private Transform[] barrel;
    [SerializeField] private Projectile projectile;
    [SerializeField] private int bulletsPerTrigger;

    [SerializeField] private float rateOfFire;
    [SerializeField] private bool triggerCanBeHeld;
    private bool _triggerAlreadyPressed;
    private float _rpm;
    private float _nextFireTimer;
    private bool _isFiring;

    [SerializeField] private int totalAmmo;
    [SerializeField] private int currentAmmo;

    [SerializeField] private float damage;

    [Header("Upgrades")]
    [SerializeField] private bool upgrade1;
    [SerializeField] private bool upgrade2;
    [SerializeField] private bool upgrade3;

    private bool _canFire = false;
    private int _currentLevel;

    public static Action<float, float> OnRequestHUD;
    public Action<float> OnHitRefill; 

    private void Start() 
    {
        OnRequestHUD?.Invoke(currentAmmo, totalAmmo);
        _rpm = 60f / rateOfFire;
    }

    private void Update() 
    {
        Attack(requestAttack);    
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
        if (currentAmmo <= 0) return;

        if (!triggerCanBeHeld && _triggerAlreadyPressed) return;

        for (int _i = 0; _i < bulletsPerTrigger; _i++)
        {
            Fire(_i);
        }

        currentAmmo--;

        currentAmmo = Mathf.Clamp(currentAmmo, 0, totalAmmo);

        OnRequestHUD?.Invoke(currentAmmo, totalAmmo);

        _nextFireTimer = Time.time + _rpm;

        _triggerAlreadyPressed = true;

        OnHitRefill?.Invoke(0.1f);
        OnAttackAnimation?.Invoke("Fire");


        void Fire(int _bullet)
        {
            Projectile _projectile = Instantiate(projectile, barrel[_bullet].position, Quaternion.LookRotation(Vector3.forward, barrel[_bullet].right));
            _projectile.LaunchProjectile(damage);
        }
    }

    public override void Switch(bool _state)
    {
        _canFire = _state;
        if (_state) Upgrades();
        base.Switch(_state);
    }

    public override void AddToMeter(float _amount)
    {
        float _actualAmount = totalAmmo * _amount;

        currentAmmo += Mathf.FloorToInt(_actualAmount);

        currentAmmo = Mathf.Clamp(currentAmmo, 0 , totalAmmo);

        OnRequestHUD?.Invoke(currentAmmo, totalAmmo);
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
            totalAmmo += 5;
            damage += 5;
        }

        if (upgrade2 && !_previousState2)
        {
            triggerCanBeHeld = true;
            totalAmmo += 5;
            damage += 5;
        }

        if (upgrade3 && !_previousState3)
        {
            damage += 10;
            bulletsPerTrigger = 3;
        }
    }
}
