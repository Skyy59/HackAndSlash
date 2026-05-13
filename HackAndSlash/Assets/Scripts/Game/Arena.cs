using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] private Door entryDoor;
    [SerializeField] private Door exitDoor;

    [SerializeField] private GameObject[] arenaEnemies;
    [SerializeField] private int enemiesPerWave;
    private int _enemiesAlive;

    [SerializeField] private Transform[] spawnPoints;

    private List<Transform> _usableSpawnPoints;
    private List<GameObject> _allEnemies;

    private bool _arenaCompleted;
    private bool _arenaStarted;
    

    private void Start() 
    {
        entryDoor.OpenDoor();
        exitDoor.OpenDoor();
    }

    private void OnTriggerEnter2D(Collider2D _other) 
    {
        if (_arenaCompleted || _arenaStarted) return;
        if (_other.CompareTag("Player"))
        {
            _arenaStarted = true;
            entryDoor.CloseDoor();
            exitDoor.CloseDoor();
            _usableSpawnPoints = spawnPoints.ToList();
            _allEnemies = arenaEnemies.ToList();
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        int _enemiesToSpawn = _allEnemies.Count - enemiesPerWave;
        if (_enemiesToSpawn <= 0) _enemiesToSpawn = _allEnemies.Count;
        else _enemiesToSpawn = enemiesPerWave;

        for (int _i = 0; _i < _enemiesToSpawn; _i++)
        {
            Instantiate(_allEnemies[0], _usableSpawnPoints[0].position, Quaternion.identity);
            _allEnemies.RemoveAt(0);
            if (_usableSpawnPoints.Count > 0) _usableSpawnPoints.RemoveAt(0);

            if (_usableSpawnPoints.Count <= 0) _usableSpawnPoints = spawnPoints.ToList();
        }
    }

    public void KillEnemy()
    {
        _enemiesAlive++;

        if (_enemiesAlive >= enemiesPerWave)
        {
            if (_allEnemies.Count > 0) SpawnEnemies();
            else OnArenaFinished();
        }
    }

    private void OnArenaFinished()
    {
        _arenaCompleted = true;
        _arenaStarted = false;
        exitDoor.OpenDoor();
    }
}
