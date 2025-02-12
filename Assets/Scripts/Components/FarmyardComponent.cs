using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmyardComponent : MonoBehaviour
{
    private BoxCollider2D area;
    public GameObject pigPrefab;

    private List<FoodContainerComponent> troughs;

    // Start is called before the first frame update
    void Start()
    {
        area = GetComponent<BoxCollider2D>();
        SaveTroughsReferences();

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
        GameObject pig = Instantiate(pigPrefab, transform);

        pig.transform.position = GetRandomPositionInFarmyard();
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
}
