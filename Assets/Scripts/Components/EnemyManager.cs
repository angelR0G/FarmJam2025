using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Monsters")]
    public GameObject stalkerPrefab;
    public int stalkersCount = 10;
    public GameObject ambusherPrefab;
    public int ambushersCount = 10;
    public GameObject corrosivePrefab;
    public int corrosivesCount = 10;
    public GameObject nightmarePrefab;

    [Header("Spawnpoints")]
    public List<Vector3> stalkerSpawns = new List<Vector3>();
    public List<Vector3> ambusherSpawns = new List<Vector3>();
    public List<Vector3> corrosiveSpawns = new List<Vector3>();
    public Vector3 nightmareSpawn = Vector3.zero;

    [Header("Other")]
    public PlayerComponent playerReference;

    private List<StalkerEnemyComponent> stalkersList;
    private List<AmbusherEnemyComponent> ambushersList;
    private List<CorrosiveEnemyComponent> corrosivesList;
    private NightmareEnemyComponent nightmareEnemy;
    private bool nightmareAlreadySpawnedTonight = false;
    private bool canNightmareSpawn = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        CreateMonstersPool();
        DeactivateMonsters();
    }

    private void Start()
    {
        BindSpawnerCallbacks();
    }

    private void BindSpawnerCallbacks()
    {
        GameManager gameManager = GameManager.Instance;

        gameManager.nightStart += SpawnMonsters;
        gameManager.nightEnd += DeactivateMonsters;
        gameManager.hourChanged += EnableNightmareSpawn;

        playerReference.GetComponent<SanityComponent>().onInsane.AddListener(SpawnNightmare);
    }

    private void CreateMonstersPool()
    {
        stalkersList = new List<StalkerEnemyComponent>(stalkersCount);
        for (int i = 0; i < stalkersCount; i++)
        {
            GameObject stalker = Instantiate(stalkerPrefab, transform);
            stalkersList.Add(stalker.GetComponent<StalkerEnemyComponent>());
        }

        ambushersList = new List<AmbusherEnemyComponent>(ambushersCount);
        for (int i = 0; i < ambushersCount; i++)
        {
            GameObject ambusher = Instantiate(ambusherPrefab, transform);
            ambushersList.Add(ambusher.GetComponent<AmbusherEnemyComponent>());
        }

        corrosivesList = new List<CorrosiveEnemyComponent>(corrosivesCount);
        for (int i = 0; i < corrosivesCount; i++)
        {
            GameObject corrosive = Instantiate(corrosivePrefab, transform);
            corrosivesList.Add(corrosive.GetComponent<CorrosiveEnemyComponent>());
        }

        nightmareEnemy = Instantiate(nightmarePrefab, transform).GetComponent<NightmareEnemyComponent>();
        nightmareEnemy.GetComponent<NightmareEnemyComponent>().attackTarget = playerReference.gameObject;
    }

    private void SpawnMonsters(object sender, int hour)
    {
        List<int> spawnedPositionsIndexes = new List<int>(stalkersCount);
        int randomIndex;

        foreach (StalkerEnemyComponent stalker in stalkersList)
        {
            randomIndex = GetRandomIndex(stalkersCount, spawnedPositionsIndexes);
            stalker.Spawn(stalkerSpawns[randomIndex]);
            spawnedPositionsIndexes.Add(randomIndex);
        }

        spawnedPositionsIndexes.Clear();

        foreach (CorrosiveEnemyComponent corrosive in corrosivesList)
        {
            randomIndex = GetRandomIndex(corrosivesCount, spawnedPositionsIndexes);
            corrosive.Spawn(corrosiveSpawns[randomIndex]);
            spawnedPositionsIndexes.Add(randomIndex);
        }

        spawnedPositionsIndexes.Clear();

        foreach (AmbusherEnemyComponent ambusher in ambushersList)
        {
            randomIndex = GetRandomIndex(ambushersCount, spawnedPositionsIndexes);
            ambusher.Spawn(ambusherSpawns[randomIndex]);
            spawnedPositionsIndexes.Add(randomIndex);
        }

        nightmareAlreadySpawnedTonight = false;
    }

    private void SpawnNightmare()
    {
        if (nightmareAlreadySpawnedTonight || !canNightmareSpawn) return;

        nightmareEnemy.Spawn(nightmareSpawn);
        nightmareAlreadySpawnedTonight = true;
    }

    private void DeactivateMonsters(object sender = null, int hour = 0)
    {
        foreach (StalkerEnemyComponent stalker in stalkersList)
            stalker.FadeAndDeactivate();

        foreach (CorrosiveEnemyComponent corrosive in corrosivesList)
            corrosive.FadeAndDeactivate();

        foreach (AmbusherEnemyComponent ambusher in ambushersList)
            ambusher.FadeAndDeactivate();

        nightmareEnemy.FadeAndDeactivate();
        canNightmareSpawn = false;
    }

    private void OnDestroy()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.nightStart -= SpawnMonsters;
            gameManager.nightEnd -= DeactivateMonsters;
        }
    }

    private int GetRandomIndex(int listSize, List<int> invalidIndexes)
    {
        if (invalidIndexes.Count >= listSize) return 0;

        int randomIndex = Random.Range(0, listSize);

        while(invalidIndexes.Contains(randomIndex))
        {
            randomIndex++;
            if (randomIndex >= listSize)
                randomIndex = 0;
        }

        return randomIndex;
    }

    private void EnableNightmareSpawn(object sender, int hour)
    {
        if (hour == 22)
        {
            canNightmareSpawn = true;
            if (playerReference.GetComponent<SanityComponent>().IsInsane())
            {
                SpawnNightmare();
            }
        }
    }
}
