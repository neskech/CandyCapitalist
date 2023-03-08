using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSpawner : MonoBehaviour
{
    [System.Serializable]
    private record SpawnInformation
    {
        public GameObject prefab;
        public Vector2Int tilePosition;

    }

    [SerializeField] List<SpawnInformation> _playerParty;
    [SerializeField] List<SpawnInformation> _enemyParty;
    public TurnManager _turnManager;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;

        foreach (var obj in _playerParty)
            _turnManager.SpawnPlayer(i++, obj.prefab, obj.tilePosition, Quaternion.identity);

        i = 0;
        foreach (var obj in _enemyParty)
            _turnManager.SpawnEnemy(i++, obj.prefab, obj.tilePosition, Quaternion.identity);
    }
}
