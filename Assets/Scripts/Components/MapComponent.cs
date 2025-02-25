using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class MapComponent : MonoBehaviour
{
    [SerializeField]
    private List<Tilemap> tilemapsWithCollisions = new List<Tilemap>();
    [SerializeField]
    private List<Tilemap> topTilemaps = new List<Tilemap>();
    [SerializeField]
    private Tilemap groundTilemap = null;

    public static MapComponent Instance;
    private bool hasToUpdateTilemaps = false;
    private float tilemapsTargetOpacity = 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (hasToUpdateTilemaps)
        {
            UpdateTilemapsOpacity();
        }
    }

    public Vector3 GetPositionAlignedToTileset(Vector3 pos)
    {
        if (groundTilemap == null) return pos;

        return groundTilemap.CellToWorld(groundTilemap.WorldToCell(pos)) + groundTilemap.cellSize/2;
    }

    public Vector2 GetTilesetCellSize()
    {
        if (groundTilemap == null) return Vector2.zero;

        return groundTilemap.cellSize;
    }

    public bool IsPositionFree(Vector3 pos)
    {
        foreach (Tilemap t in tilemapsWithCollisions)
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

    public void SetTopTilemapsVisibility(bool newVisibility)
    {
        tilemapsTargetOpacity = newVisibility ? 1f : 0f;
        hasToUpdateTilemaps = true;
    }

    private void UpdateTilemapsOpacity()
    {
        if (topTilemaps.Count == 0) return;

        // Calculates final color
        Color currentColor = topTilemaps[0].color;
        if (currentColor.a > tilemapsTargetOpacity)
            currentColor.a = Mathf.Max(currentColor.a - Time.deltaTime, tilemapsTargetOpacity);
        else
            currentColor.a = Mathf.Min(currentColor.a + Time.deltaTime, tilemapsTargetOpacity);
            
        if (currentColor.a == tilemapsTargetOpacity)
        {
            currentColor.a = tilemapsTargetOpacity;
            hasToUpdateTilemaps = false;
        }

        // Updates all tilemaps color
        foreach (Tilemap t in topTilemaps)
        {
            t.color = currentColor;
        }
    }
}
