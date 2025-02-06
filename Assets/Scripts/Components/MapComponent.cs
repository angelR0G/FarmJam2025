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
}
