using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Positions where enemies will spawn")]
    [SerializeField] private Transform[] _spawnPoints;
    [Tooltip("Minimum number of active enemies")]
    [SerializeField] private int _minEnemiesCount;
    [Tooltip("Maximum number of active enemies")]
    [SerializeField] private int _maxEnemiesCount;
    [Tooltip("The delay between spawning enemies")]
    [SerializeField] private float _spawnDelay;
    [Tooltip("The enemy's prefab tag in ObjectPooler")]
    [SerializeField] private string[] _enemyTags;


    private int _enemiesCount; //current enemies count on level
    private float _spawnTimer; //timer to next spawn
    private Transform _player; //player instance

    private void Start()
    {
        _player = Player.Instance.transform;
    }

    private void Update()
    {
        //spawn minimal required enemies
        if(_enemiesCount < _minEnemiesCount)
        {
            SpawnEnemy();
            return;
        }

        //move spawner with player
        transform.position = _player.position;

        //spawn a new enemy after the timer expires
        if (_spawnTimer > 0)
            _spawnTimer -= Time.deltaTime;
        else
            SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        //return if we reached the limit of enemies
        if (_enemiesCount >= _maxEnemiesCount)
            return;

        //get random spawnpoint
        int randomPointID = Random.Range(0, _spawnPoints.Length);
        //pulling a new enemy out of the pool
        GameObject enemy = ObjectPooler.Instance.SpawnFromPool(_enemyTags[0], _spawnPoints[randomPointID].position, _spawnPoints[randomPointID].rotation);
        //subscribe to the event of the enemy's death
        enemy.GetComponent<Enemy>().EnemyDied += OnEnemyDied;
        //reset the spawn timer
        _spawnTimer = _spawnDelay;
        //increase the number of enemies
        _enemiesCount++;
    }

    private void OnEnemyDied(Enemy enemy)
    {
        //unsubscribe to the event of the enemy's death
        enemy.EnemyDied -= OnEnemyDied;
        //decrease the number of enemies
        _enemiesCount--;
    }

    private void OnDrawGizmos()
    {
        //draw the sphere on each spawn point
        Gizmos.color = Color.blue;
        foreach(Transform point in _spawnPoints)
        {
            Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
}
