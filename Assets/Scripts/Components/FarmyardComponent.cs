using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmyardComponent : MonoBehaviour
{
    private BoxCollider2D area;
    public GameObject pigPrefab;

    // Start is called before the first frame update
    void Start()
    {
        area = GetComponent<BoxCollider2D>();
        SpawnPig();
    }

    public Vector3 GetRandomPositionInFarmyard()
    {
        Vector3 randomPosition = new Vector3();

        randomPosition.x = area.bounds.min.x + Random.value * (area.bounds.extents.x * 2);
        randomPosition.y = area.bounds.min.y + Random.value * (area.bounds.extents.y * 2);

        return randomPosition;
    }

    public void SpawnPig()
    {
        GameObject pig = Instantiate(pigPrefab, transform);

        pig.transform.position = GetRandomPositionInFarmyard();
        pig.GetComponent<PigComponent>().farmyard = this;
    }
}
