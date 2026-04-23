using System;
using UnityEngine;

public class Weapon_Switch : MonoBehaviour
{
    [SerializeField] private Player_Controller player;
    [SerializeField] private Transform target;

    [SerializeField] private Transform leftHandIK;
    [SerializeField] private Transform rightHandIK;

    [SerializeField] private Weapon[] allWeapons;
    [SerializeField] private Weapon currentWeapon;

    [Space(5)]

    [SerializeField] private Weapon_Melee _meleeWeapon;
    [SerializeField] private Weapon_Ranged _rangedWeapon;

    private int _weaponIndex = 0;

    private void OnEnable() 
    {
        player.OnRequestAttack += SetAttack;
        player.OnRequestScroll += ChangeWeapon;

        Weapon.OnAttackAnimation += player.PlayAnimation;

        _meleeWeapon.OnHitRefill += AddRangeMeter;
        _rangedWeapon.OnHitRefill += AddMeleeMeter;

        Weapon.ReturnDirection = player.ReturnDirection;
    }

    private void OnDisable() 
    {
        player.OnRequestAttack -= SetAttack;
        player.OnRequestScroll -= ChangeWeapon;

        Weapon.OnAttackAnimation -= player.PlayAnimation;

        _meleeWeapon.OnHitRefill -= AddRangeMeter;
        _rangedWeapon.OnHitRefill -= AddMeleeMeter;
    }

    private void Start() 
    {
        for (int _i = 0; _i < allWeapons.Length; _i++)
        {
            allWeapons[_i].Switch(false);
        }
        currentWeapon = allWeapons[_weaponIndex];
        SetWeapon();    
    }

    private void Update() 
    {
        LookAtCrosshair();  
        transform.localScale = new Vector2(transform.localScale.x, target.position.x > player.transform.position.x ? 1f : -1); 
    }

    public void ChangeWeapon(float _dir)
    {
        if (_dir > 0) _weaponIndex = _weaponIndex + 1 > allWeapons.Length - 1 ? 0 : _weaponIndex + 1;
        else if (_dir < 0) _weaponIndex = _weaponIndex - 1 < 0 ? allWeapons.Length - 1 : _weaponIndex - 1;

        SetWeapon();
    }

    public void SetWeapon()
    {
        currentWeapon.Switch(false);
        currentWeapon = allWeapons[_weaponIndex];
        currentWeapon.Switch(true);
        currentWeapon.SetHandles(leftHandIK, rightHandIK);
    }

    private void LookAtCrosshair()
    {
        float _angleRadians = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x);
        float _angleDegrees = (180f / Mathf.PI) * _angleRadians;
        transform.rotation = Quaternion.Euler(0, 0, _angleDegrees);
    }

    public void SetAttack(bool _state)
    {
        currentWeapon.requestAttack = _state;
    }


    public void AddMeleeMeter(float _amount) => _meleeWeapon.AddToMeter(_amount);

    public void AddRangeMeter(float _amount) => _rangedWeapon.AddToMeter(_amount);
}
