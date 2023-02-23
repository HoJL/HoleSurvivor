using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : Pooling
{
    public enum EnemyGrade
    {
        Normal,
        Rare,
        Unique,
        Legend

    }
    [SerializeField] float _spawnRadius = 3.0f;
    [SerializeField] GameObject _enemyPrefabs;
    [SerializeField] GameObject _rareEnemyPrefabs;
    [SerializeField] GameObject _uniqueEnemyPrefabs;
    [SerializeField] GameObject _legendEnemyPrefabs;
    [SerializeField] float _spawnTime = 1.0f;
    Transform _target;
    HoleManager _player;
    IObjectPool<PoolObject> _pool;
    IObjectPool<PoolObject> _rarePool;
    IObjectPool<PoolObject> _uniquePool;
    IObjectPool<PoolObject> _legendPool;
    float time = 0.0f;
    public float SpawnRadius {get => _spawnRadius; set => _spawnRadius = value;}
    public float SpawnTime {get => _spawnTime; set => _spawnTime = value;}
    
    // Start is called before the first frame update
    void Start()
    {
        _pool = new ObjectPool<PoolObject>(CreateNormal, OnGetObj, OnReleaseObj, OnDestroyObj, maxSize:20);
        _rarePool = new ObjectPool<PoolObject>(CreateRare, OnGetObj, OnReleaseObj, OnDestroyObj, maxSize:20);
        _uniquePool = new ObjectPool<PoolObject>(CreateUnique, OnGetObj, OnReleaseObj, OnDestroyObj, maxSize:20);
        _legendPool = new ObjectPool<PoolObject>(CreateLegend, OnGetObj, OnReleaseObj, OnDestroyObj, maxSize:20);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsGameEnd) return;
        if (GameManager.Instance.IsPause) return;
        if (!GameManager.Instance.IsGameStart) return;
        SpawnEnemy();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelfSpawn();
        }
    }

    public void Init(Transform player)
    {
        _target = player;
        _player = _target.GetComponent<HoleManager>();
    }

    Vector3 GetRandomPotion()
    {
        Vector3 playerPosition = _target.position;
        float x = playerPosition.x;
        float y = playerPosition.z;
        float randX = Random.Range(-_spawnRadius + x, _spawnRadius + x);
        float randY = Mathf.Sqrt(Mathf.Pow(_spawnRadius, 2) - Mathf.Pow(randX - x, 2));
        randY *= Random.Range(0, 2) == 0 ? -1 : 1;
        randY += y;
        Vector3 randomPosition = new Vector3(randX, 0, randY);

        return randomPosition;
    }

    PoolObject CreateNormal()
    {
        return CreateEnemy(_enemyPrefabs, _pool);
    }

    PoolObject CreateRare()
    {
        return CreateEnemy(_rareEnemyPrefabs, _rarePool);
    }

    PoolObject CreateUnique()
    {
        return CreateEnemy(_uniqueEnemyPrefabs, _uniquePool);
    }

    PoolObject CreateLegend()
    {
        return CreateEnemy(_legendEnemyPrefabs, _legendPool);
    }

    PoolObject CreateEnemy(GameObject obj, IObjectPool<PoolObject> pool)
    {
        PoolObject enemy = Instantiate(obj).GetComponent<PoolObject>();
        enemy.SetPool(pool);
        return enemy;
    }

    void DoAttack(float level)
    {
        var hm = _target.GetComponent<HoleManager>();
        hm.OnHit(level);
    }
    
    void Spawn(IObjectPool<PoolObject> pool, int level, EnemyGrade grade)
    {
        var enemy = pool.Get();
        enemy.transform.position = GetRandomPotion();
        enemy.transform.parent = transform;
        var dir = _target.position - enemy.transform.position;
        enemy.transform.rotation = Quaternion.LookRotation(dir);
        var rb = enemy.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        enemy.GetComponent<BoxCollider>().isTrigger = false;
        enemy.GetComponent<Enemy>().Init(_target,level, DoAttack, grade);
    }

    void SpawnEnemy()
    {
        if (Time.time - time < _spawnTime) return;
        SelfSpawn();
        time = Time.time;
    }

    void SelfSpawn()
    {
        SpawnNormal();
        if (_player.CurrentLevel >= 9)
        {
            SpawnLegend();
            SpawnUique();
            SpawnRare();
        }
        else if (_player.CurrentLevel >= 7)
        {
            SpawnUique();
            SpawnRare();
        }
        else if (_player.CurrentLevel >= 5)
        {
            if (GetIsPercent(20))
            {
                SpawnUique();
            }
            SpawnRare();
        }
        else if (_player.CurrentLevel >= 3)
        {
            if (GetIsPercent(20))
            {
                SpawnRare();
            }
        }
    }

    void SpawnKind(int len, IObjectPool<PoolObject> pool, int level, EnemyGrade grade)
    {
        for (int i = 0; i < len; i++)
            Spawn(pool, level, grade);
    }

    void SpawnNormal()
    {
        int normMax = 20;
        SpawnKind(normMax, _pool, 1, EnemyGrade.Normal);
    }

    void SpawnRare()
    {
        int rareMax = 3;
        SpawnKind(rareMax, _rarePool, 5, EnemyGrade.Rare);
    }

    void SpawnUique()
    {
        int uniqueMax = 3;
        SpawnKind(uniqueMax, _uniquePool, 9, EnemyGrade.Unique);
    }

    void SpawnLegend()
    {
        int legendMax = 3;
        SpawnKind(legendMax, _legendPool, 10, EnemyGrade.Legend);
    }


    bool GetIsPercent(float percent)
    {
        int max = 1000000;
        var hit = percent * 0.01f;
        hit *= max;
        var rand = Random.Range(1, max);
        if (rand <= hit) return true;
        return false;
    }
}
