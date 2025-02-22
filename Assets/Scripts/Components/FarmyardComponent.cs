using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmyardComponent : MonoBehaviour
{
    private const int PIG_SPAWN_INTERVAL = 3;

    private BoxCollider2D area;
    public GameObject pigPrefab;

    private int daysUntilNewPigSpawn = PIG_SPAWN_INTERVAL;
    private List<FoodContainerComponent> troughs;

    // Start is called before the first frame update
    void Start()
    {
        area = GetComponent<BoxCollider2D>();
        SaveTroughsReferences();

        GameManager.Instance.nightEnd += OnNewDay;
        SpawnPig();
    }

    public Vector3 GetRandomPositionInFarmyard()
    {
        Vector3 randomPosition = new Vector3();

        randomPosition.x = area.bounds.min.x + Random.value * (area.bounds.extents.x * 2);
        randomPosition.y = area.bounds.min.y + Random.value * (area.bounds.extents.y * 2);

        return randomPosition;
    }

    // Returns whether there is a trough filled with food. If it is found, returns its position in the out parameter
    public bool GetFilledTroughPosition(out Vector3 troughPosition)
    {
        foreach (FoodContainerComponent t in troughs)
        {
            if (t.HasFood)
            {
                troughPosition = t.transform.position;
                return true;
            }
        }

        troughPosition = Vector3.zero;
        return false;
    }

    public void SpawnPig()
    {
        Vector3 spawnPosition = GetSpawnPosition();

        GameObject pig = Instantiate(pigPrefab, transform);

        pig.transform.position = spawnPosition;
        pig.GetComponent<PigComponent>().farmyard = this;
    }

    public void SaveTroughsReferences()
    {
        troughs = new List<FoodContainerComponent>();

        FoodContainerComponent trough;
        foreach (Transform child in transform)
        {
            if (child.gameObject.TryGetComponent<FoodContainerComponent>(out trough))
            {
                troughs.Add(trough);
            }
        }
    }

    public Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;

        List<Collider2D> result = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;

        int spawnAttempts = 5;
        while (spawnAttempts > 0)
        {
            spawnPosition = GetRandomPositionInFarmyard();

            Physics2D.OverlapCircle(spawnPosition, 0.15f, filter, result);
            if (result.Count == 0)
                return spawnPosition;

            spawnAttempts--;
        }

        // If cannot find an empty position, returns the center of the farmyard
        return transform.position;
    }

    private void OnNewDay(object sender, int hour)
    {
        daysUntilNewPigSpawn--;

        if (daysUntilNewPigSpawn <= 0)
        {
            SpawnPig();
            daysUntilNewPigSpawn = PIG_SPAWN_INTERVAL;
        }
    }
}
