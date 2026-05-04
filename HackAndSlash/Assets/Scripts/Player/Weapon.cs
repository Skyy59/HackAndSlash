using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform leftHandle;
    [SerializeField] private Transform rightHandle;

    [SerializeField] private SpriteRenderer weaponSprite;

    public bool requestAttack;

    public static Func<float> ReturnDirection;
    public static Action<string> OnAttackAnimation;

    public virtual void Attack(bool _state)
    {
        
    }

    public virtual void Switch(bool _state)
    {
        weaponSprite.color = _state ? Color.white : Color.clear;
    }

    public virtual void Upgrades()
    {
        
    }

    public virtual void AddToMeter(float _amount)
    {
        
    }

    public virtual void SetHandles(Transform _leftHand, Transform _rightHand)
    {
        _leftHand.parent = leftHandle;
        _leftHand.transform.localPosition = Vector2.zero;

        _rightHand.parent = rightHandle;
        _rightHand.transform.localPosition = Vector2.zero;
    }
}
