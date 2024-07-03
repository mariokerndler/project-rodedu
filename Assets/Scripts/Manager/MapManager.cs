using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

/// <summary>
/// Manages the map and overlay tiles in the game.
/// </summary>
public class MapManager : Singleton<MapManager>
{
    public OverlayTile overlayPrefab;
    public GameObject overlayContainer;
    
    public Dictionary<Vector2Int, OverlayTile> Map;
    public bool ignoreBottomTiles;
    
    public void LoadMap(int index)
    {
        // Get all tilemaps
        var tilemaps = GetComponentsInChildren<Tilemap>();

        // Deactivate them
        foreach (var tilemap in tilemaps)
        {
            tilemap.gameObject.SetActive(false);
        }
        
        if (index > tilemaps.Length - 1 || index < 0)
        {
            Debug.LogError("Index out of bounds, cannot load map.");
            return;
        }
        
        // Clear container
        for (var i = 0; i < overlayContainer.transform.childCount; i++)
        {
            Destroy(overlayContainer.transform.GetChild(i));
        }

        var tm = tilemaps[index];
        tm.gameObject.SetActive(true);
        
        ConstructMapDict(tm);
    }
    
    private void ConstructMapDict(Tilemap tm)
    {
        // Initialize the map dictionary.
        Map = new Dictionary<Vector2Int, OverlayTile>();
        
        var bounds = tm.cellBounds;

        // Loop through each cell in the Tilemap.
        for (var z = bounds.max.z; z >= bounds.min.z; z--)
        {
            // Ignore bottom layer tiles if specified.
            if (z == 0 && ignoreBottomTiles) continue;
            
            for (var y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (var x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var cellPosition = new Vector3Int(x, y, z);
                    
                    // Skip cells that do not have a tile.
                    if (!tm.HasTile(cellPosition)) continue;

                    var gridPosition = new Vector2Int(x, y);
                    
                    // Skip cells that already have an overlay tile.
                    if (Map.ContainsKey(gridPosition)) continue;

                    // Instantiate and position the overlay tile.
                    var overlayTile = Instantiate(overlayPrefab, overlayContainer.transform);
                    var cellWorldPosition = tm.GetCellCenterWorld(cellPosition);
                    overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y,
                        cellWorldPosition.z + 1);
                    overlayTile.GetComponent<SpriteRenderer>().sortingOrder =
                        tm.GetComponent<TilemapRenderer>().sortingOrder;
                    overlayTile.GridLocation = cellPosition;

                    // Add the overlay tile to the map.
                    Map.Add(gridPosition, overlayTile);
                }
            }
        }
        
    }
    
    /// <summary>
    /// Gets the tiles surrounding a given tile.
    /// </summary>
    /// <param name="originTile">The origin tile position.</param>
    /// <returns>A list of surrounding overlay tiles.</returns>
    public List<OverlayTile> GetSurroundingTiles(Vector2Int originTile)
    {
        // Define directions to check (right, left, up, down).
        var directions = new[]
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        // Check each direction and collect valid surrounding tiles.
        return directions
            .Select(direction => originTile + direction)
            .Where(tileToCheck => Map.ContainsKey(tileToCheck))
            .Where(tileToCheck => Mathf.Abs(Map[tileToCheck].transform.position.z - Map[originTile].transform.position.z) <= 1)
            .Select(tileToCheck => Map[tileToCheck])
            .ToList();
    }
}
