using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapManager : Singleton<MapManager>
{
    public OverlayTile overlayPrefab;
    public GameObject overlayContainer;
    
    public Dictionary<Vector2Int, OverlayTile> map;
    public bool ignoreBottomTiles;

    private void Start()
    {
        var tileMaps = gameObject.transform.GetComponentsInChildren<Tilemap>()
            .OrderByDescending(x => x.GetComponent<TilemapRenderer>().sortingOrder);
        map = new Dictionary<Vector2Int, OverlayTile>();

        foreach (var tm in tileMaps)
        {
            var bounds = tm.cellBounds;

            for (var z = bounds.max.z; z >= bounds.min.z; z--)
            {
                for (var y = bounds.min.y; y < bounds.max.y; y++)
                {
                    for (var x = bounds.min.x; x < bounds.max.x; x++)
                    {
                        if (z == 0 && ignoreBottomTiles)
                            return;

                        if (!tm.HasTile(new Vector3Int(x, y, z))) continue;
                        if (map.ContainsKey(new Vector2Int(x, y))) continue;

                        var overlayTile = Instantiate(overlayPrefab, overlayContainer.transform);
                        var cellWorldPosition = tm.GetCellCenterWorld(new Vector3Int(x, y, z));
                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y,
                            cellWorldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder =
                            tm.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gameObject.GetComponent<OverlayTile>().gridLocation = new Vector3Int(x, y, z);

                        map.Add(new Vector2Int(x, y), overlayTile.gameObject.GetComponent<OverlayTile>());
                    }
                }
            }
        }
    }
    
    public List<OverlayTile> GetSurroundingTiles(Vector2Int originTile)
    {
        var surroundingTiles = new List<OverlayTile>();
        
        var tileToCheck = new Vector2Int(originTile.x + 1, originTile.y);
        if (map.ContainsKey(tileToCheck))
        {
            if (Mathf.Abs(map[tileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[tileToCheck]);
        }

        tileToCheck = new Vector2Int(originTile.x - 1, originTile.y);
        if (map.ContainsKey(tileToCheck))
        {
            if (Mathf.Abs(map[tileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[tileToCheck]);
        }

        tileToCheck = new Vector2Int(originTile.x, originTile.y + 1);
        if (map.ContainsKey(tileToCheck))
        {
            if (Mathf.Abs(map[tileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
                surroundingTiles.Add(map[tileToCheck]);
        }

        tileToCheck = new Vector2Int(originTile.x, originTile.y - 1);
        if (!map.ContainsKey(tileToCheck)) return surroundingTiles;
        
        if (Mathf.Abs(map[tileToCheck].transform.position.z - map[originTile].transform.position.z) <= 1)
            surroundingTiles.Add(map[tileToCheck]);

        return surroundingTiles;
    }
}
