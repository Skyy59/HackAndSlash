using System;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade_Drone : MonoBehaviour
{

    public enum WeaponUpgrade { Melee, Range};

    [SerializeField] private Player_Controller player;
    [SerializeField] private WeaponUpgrade upgradeType;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer interactKey;

    public static Action<WeaponUpgrade> OnUpgrade;
    private bool _droneUsed;

    private void OnTriggerEnter2D(Collider2D _other) 
    {
        if (_droneUsed) return;
        if (_other.CompareTag("Player"))
        {
            interactKey.color = Color.white;
            player.OnInteractAction += Interact;
        }
    }

    private void OnTriggerExit2D(Collider2D _other) 
    {
        if (_other.CompareTag("Player"))
        {
            player.OnInteractAction -= Interact;
            interactKey.color = Color.clear;
        }
    }

    private void Interact()
    {
        if (_droneUsed) return;
        OnUpgrade?.Invoke(upgradeType);
        player.OnInteractAction -= Interact;
        _droneUsed = true;
        interactKey.color = Color.clear;
    }
}
