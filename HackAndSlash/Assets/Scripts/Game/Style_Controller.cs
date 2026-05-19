using System;
using System.Collections.Generic;
using UnityEngine;

public class Style_Controller : MonoBehaviour
{

    public enum StyleKey { Limb , Body , Head , Melee , Range , Dead, Player }

    [System.Serializable]
    public struct StyleParameter
    {
        public StyleKey styleKey;
        public int basePoints;
        public int maximumPermited;
        public float minimumMult;
        public bool hasLimit;
    }

    [System.Serializable]
    public struct StyleThresholds
    {
        public int amountDelta;
        public bool achieved;
    }

    [SerializeField] private StyleParameter[] styleParameters;
    [SerializeField] private StyleThresholds[] styleThresholds;

    [SerializeField] private int currentPoints;
    [SerializeField] private int totalPoints;

    [SerializeField] private float styleRemoveTimer;
    [SerializeField] private int styleRemoveAmount;
    private float _styleRemoveCounter;

    private List<StyleKey> _currentKeys;

    private List<StyleKey> _keysToCheck;

    public Action<int, int> OnStyleValue;
    public static Action<int> OnStyleThresholds;


    private void OnEnable() 
    {
        EnemyLimb.OnDamage += AddKeys;
        Enemy.OnDamage += AddKeys;
        Player_Health.OnDamage += AddKeys;
    }

    private void OnDisable() 
    {
        EnemyLimb.OnDamage -= AddKeys;
        Enemy.OnDamage -= AddKeys;
        Player_Health.OnDamage -= AddKeys;
    }

    private void Start() 
    {
        _currentKeys = new List<StyleKey>();
        _keysToCheck = new List<StyleKey>();

        OnStyleValue?.Invoke(currentPoints, totalPoints);
    }

    private void Update() 
    {
        PassiveStyleRemover();    
    }

    public void AddKeys(string[] _keysToAdd)
    {
        for (int _i = 0; _i < _keysToAdd.Length; _i++)
        {
            StyleKey _key = StyleKey.Limb;

            for (int _j = 0; _j < styleParameters.Length; _j++)
            {
                if (styleParameters[_j].styleKey.ToString() == _keysToAdd[_i]) _key = styleParameters[_j].styleKey;
                continue;
            }

            _keysToCheck.Add(_key);
        }

        CheckKeys();
        _styleRemoveCounter = 0;
    }

    private void PassiveStyleRemover()
    {
        if (currentPoints <= 0) return;

        _styleRemoveCounter += Time.deltaTime / styleRemoveTimer;

        if (_styleRemoveCounter >= 1)
        {
            _styleRemoveCounter = 0;
            currentPoints -= styleRemoveAmount;
            currentPoints = Mathf.Clamp(currentPoints, 0, totalPoints);
            CheckThresholds();
            OnStyleValue?.Invoke(currentPoints, totalPoints);
        }
    }

    private void CheckKeys()
    {
        for (int _i = 0; _i < _keysToCheck.Count; _i++)
        {
            currentPoints += ReturnKeyPoints(_keysToCheck[_i]);
            currentPoints = Mathf.Clamp(currentPoints, 0, totalPoints);
            CheckThresholds();
            _currentKeys.Add(_keysToCheck[_i]);
            _keysToCheck.RemoveAt(_i);
            OnStyleValue?.Invoke(currentPoints, totalPoints);
        }
    }

    private int ReturnKeyPoints(StyleKey _key)
    {
        int _counter = 0;
        for (int _i = 0; _i < _currentKeys.Count; _i++)
        {
            if (_currentKeys[_i] == _key) _counter++;
        }

        StyleParameter _parameter = ReturnStyleParameter(_key);

        if (_parameter.hasLimit)
        {
            int _keyAmount = _counter < _parameter.maximumPermited ? _counter : _parameter.maximumPermited;
            return Mathf.RoundToInt(Mathf.Lerp(_parameter.basePoints, _parameter.basePoints * _parameter.minimumMult, _keyAmount));
        }
        else
        {
            return _parameter.basePoints;
        }
    }

    private StyleParameter ReturnStyleParameter(StyleKey _key)
    {
        for (int _i = 0; _i < styleParameters.Length; _i++)
        {
            if (styleParameters[_i].styleKey == _key) return styleParameters[_i];
        }

        return new StyleParameter();
    }

    private void CheckThresholds()
    {
        for (int _i = 0; _i < styleThresholds.Length; _i++)
        {
            if (currentPoints >= styleThresholds[_i].amountDelta && !styleThresholds[_i].achieved)
            {
                styleThresholds[_i].achieved = true;
            }
            else if (currentPoints < styleThresholds[_i].amountDelta)
            {
                styleThresholds[_i].achieved = false;
            }
        }

        int _counter = 0;

        for (int _i = 0; _i < styleThresholds.Length; _i++)
        {
            if (styleThresholds[_i].achieved) _counter++;
        }

        OnStyleThresholds?.Invoke(_counter);
    }
}
