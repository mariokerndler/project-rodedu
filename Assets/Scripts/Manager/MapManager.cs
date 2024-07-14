using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the map and overlay tiles in the game.
/// </summary>
public class MapManager : Singleton<MapManager>
{
    public OverlayTile overlayPrefab;
    public GameObject overlayContainer;
    public GameObject creatureContainer;

    public CreatureContainer Creatures;
    
    public Dictionary<Vector2Int, OverlayTile> Map { get; private set; }
    public bool ignoreBottomTiles;

    private Tilemap _activeTilemap;
    public Dictionary<Vector3, Creature> CurrentCreatures
    {
        get;
        private set;
    }
    
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
        _activeTilemap = tm;
        
        ConstructMapDict(tm);
    }

    public void LoadCreatures(int room, int floor)
    {
        if (Map is null || Map.Count <= 0) return;
        
        if (!Creatures || Creatures.CreatureStrengths.Count <= 0) return;
        
        CurrentCreatures = new Dictionary<Vector3, Creature>();
        
        var creaturesToLoad = GetCreatures(room, floor);

        var enemyTilePositions = GetFilteredTiles(false);

        foreach (var creature in creaturesToLoad)
        {
            // Check if there is enough room
            if (creaturesToLoad.Count > enemyTilePositions.Count)
            {
                // TODO: Maybe handle this differently, if there is no space, we shrink the amount of enemies spawned.
                Debug.LogError("Not enough space to spawn enemies.");
                break;
            }
            
            // Get random spawn tile
            var randomTile = GetRandomTile(enemyTilePositions);

            var createdCreature = Instantiate(creature.gameObject, creatureContainer.transform);
            createdCreature.transform.position = randomTile.transform.position;
            createdCreature.GetComponent<SpriteRenderer>().sortingOrder =
                randomTile.GetComponent<SpriteRenderer>().sortingOrder;

            CurrentCreatures[randomTile.transform.position] = createdCreature.GetComponent<Creature>();
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

    public List<OverlayTile> GetFilteredTiles(bool isPlayer)
    {
        var size = isPlayer 
            ? _activeTilemap.cellBounds.xMin + CalculateRoundedEven(_activeTilemap.cellBounds.size.x) 
            : _activeTilemap.cellBounds.xMax - CalculateRoundedEven(_activeTilemap.cellBounds.size.x);
        
        var filteredTiles = new List<OverlayTile>();
    
        foreach (var (gridPosition, tile) in Map)
        {
            if (isPlayer ? gridPosition.x < size : gridPosition.x >= size)
            {
                filteredTiles.Add(tile);
            }
        }
    
        if (filteredTiles.Count != 0) 
            return filteredTiles;
    
        Debug.LogWarning("No elements found with key's x coordinate above the threshold.");
        return null;
    }


    private OverlayTile GetRandomTile(List<OverlayTile> tiles)
    {
        OverlayTile randomTile;
        do
        {
            var randomIndex = Random.Range(0, tiles.Count);
            randomTile = tiles[randomIndex];
        } while (CurrentCreatures.ContainsKey(randomTile.transform.position));
        
        return randomTile;
    }
    
    // TODO: VERY MUCH WIP
    private List<Creature> GetCreatures(int room, int floor)
    {
        var creaturesToLoad = new List<Creature>();
        switch (room)
        {
            case 1:
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor, 2));
                break;
            case 2:
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor, 3));
                break;
            case 3: 
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor, 2));
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor + 1, 1));
                break;
            case 4: 
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor, 2));
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor + 1, 2));
                break;
            case 5: 
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor, 1));
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor + 1, 3));
                break;
            case 6: 
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor, 2));
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor + 1, 2));
                creaturesToLoad.AddRange(GetCreaturesForStrength(floor + 2, 1));
                break;
        }

        return creaturesToLoad;
    }

    private List<Creature> GetCreaturesForStrength(int strength, int amount)
    {
        var creatureStrengths = Creatures.CreatureStrengths;
        if (creatureStrengths.Count <= 0) return new List<Creature>();

        var foundCreatures = creatureStrengths.Find(x => x.StrengthLevel == strength);

        var creatures = new List<Creature>();
        
        for (var i = 0; i < amount; i++)
        {
            var creature = foundCreatures.Creatures[Random.Range(0, foundCreatures.Creatures.Count)];
            creatures.Add(creature);
        }

        return creatures;
    }
    
    private void ConstructMapDict(Tilemap tm)
    {
        // Initialize the map dictionary.
        Map = new Dictionary<Vector2Int, OverlayTile>();
        
        var bound = tm.cellBounds;

        // Loop through each cell in the Tilemap.
        for (var z = bound.max.z; z >= bound.min.z; z--)
        {
            // Ignore bottom layer tiles if specified.
            if (z == 0 && ignoreBottomTiles) continue;
            
            for (var y = bound.min.y; y < bound.max.y; y++)
            {
                for (var x = bound.min.x; x < bound.max.x; x++)
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
    
    private static int CalculateRoundedEven(int x)
    {
        var eightyPercent = x * 0.7f;
        return (int)(Mathf.Ceil(eightyPercent / 2));
    }
}
