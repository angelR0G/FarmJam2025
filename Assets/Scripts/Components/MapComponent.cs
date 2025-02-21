using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class MapComponent : MonoBehaviour
{
    [SerializeField]
    private List<Tilemap> tilemaps = new List<Tilemap>();
    public static MapComponent Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetPositionAlignedToTileset(Vector3 pos)
    {
        if (tilemaps.Count == 0 || tilemaps[0] == null) return pos;

        return tilemaps[0].CellToWorld(tilemaps[0].WorldToCell(pos)) + tilemaps[0].cellSize/2;
    }

    public Vector2 GetTilesetCellSize()
    {
        if (tilemaps.Count == 0 || tilemaps[0] == null) return Vector2.zero;

        return tilemaps[0].cellSize;
    }

    public bool IsPositionFree(Vector3 pos)
    {
        foreach (Tilemap t in tilemaps)
        {
            if (t != null && !IsPositionFreeOnTilemap(pos, t)) return false;
        }

        return true;
    }

    private bool IsPositionFreeOnTilemap(Vector3 pos, Tilemap t)
    {
        TileBase tile = t.GetTile(t.WorldToCell(pos));

        return true;
    }

    public Vector3 GetWalkablePositionAroundPoint(Vector3 origin, float minDistance, float maxDistance)
    {
        Vector3 direction = Vector3.zero;
        float distance = 0;
        Vector3 randomPoint = Vector3.zero;
        ushort remainingAttempts = 10;

        do
        {
            remainingAttempts--;

            // Calculate a random point from a random direction and distance
            direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            distance = minDistance + (maxDistance-minDistance) * Random.value;
            randomPoint = origin + direction * distance;

            // If there is no collision in the tilemap, the point is valid and stop loop
            if (IsPositionFree(randomPoint))
                return randomPoint;

        } while(remainingAttempts > 0);

        // If no point could be found, return the origin
        return origin;
    }
}
