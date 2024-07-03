using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// PathFinder class for finding paths between tiles on a grid.
/// </summary>
public class PathFinder
{
    // Dictionary to store searchable tiles.
    private Dictionary<Vector2Int, OverlayTile> _searchableTiles;
    
    /// <summary>
    /// Finds a path from the start tile to the end tile within the range of tiles.
    /// </summary>
    /// <param name="start">The starting tile.</param>
    /// <param name="end">The ending tile.</param>
    /// <param name="inRangeTiles">The list of tiles that are in range.</param>
    /// <returns>A list of tiles representing the path from start to end.</returns>
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> inRangeTiles)
    {
        _searchableTiles = new Dictionary<Vector2Int, OverlayTile>();

        // Lists to keep track of open and closed tiles.
        var openList = new List<OverlayTile>();
        var closedList = new HashSet<OverlayTile>();

        // Populate searchable tiles from inRangeTiles if provided, else use the entire map.
        if (inRangeTiles.Count > 0)
        {
            foreach (var item in inRangeTiles)
            {
                _searchableTiles.Add(item.Grid2DLocation, MapManager.Instance.Map[item.Grid2DLocation]);
            }
        }
        else
        {
            _searchableTiles = MapManager.Instance.Map;
        }

        // Add the starting tile to the open list.
        openList.Add(start);

        // Main pathfinding loop.
        while (openList.Count > 0)
        {
            // Get the tile with the lowest F score.
            var currentOverlayTile = openList.OrderBy(x => x.F).First();

            // Move current tile from open to closed list.
            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            // Check if we reached the end tile.
            if (currentOverlayTile == end)
            {
                return GetFinishedList(start, end);
            }

            // Check neighbouring tiles.
            foreach (var tile in GetNeightbourOverlayTiles(currentOverlayTile).Where(tile => !tile.isBlocked && !closedList.Contains(tile) && !(Mathf.Abs(currentOverlayTile.transform.position.z - tile.transform.position.z) > 1)))
            {
                tile.G = GetManhattenDistance(start, tile);
                tile.H = GetManhattenDistance(end, tile);

                tile.Previous = currentOverlayTile;

                // Add tile to open list if not already present.
                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                }
            }
        }

        // Return an empty path if no path is found.
        return new List<OverlayTile>();
    }

    /// <summary>
    /// Constructs the finished path by backtracking from the end tile to the start tile.
    /// </summary>
    /// <param name="start">The starting tile.</param>
    /// <param name="end">The ending tile.</param>
    /// <returns>A list of tiles representing the path.</returns>
    private static List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        var finishedList = new List<OverlayTile>();
        var currentTile = end;

        // Backtrack from end to start tile.
        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.Previous;
        }

        // Reverse the path to get the correct order.
        finishedList.Reverse();

        return finishedList;
    }

    /// <summary>
    /// Calculates the Manhattan distance between two tiles.
    /// </summary>
    /// <param name="start">The starting tile.</param>
    /// <param name="tile">The target tile.</param>
    /// <returns>The Manhattan distance between the tiles.</returns>
    private static int GetManhattenDistance(OverlayTile start, OverlayTile tile)
    {
        return Mathf.Abs(start.GridLocation.x - tile.GridLocation.x) + Mathf.Abs(start.GridLocation.y - tile.GridLocation.y);
    }

    /// <summary>
    /// Gets the neighbouring tiles of the current tile.
    /// </summary>
    /// <param name="currentOverlayTile">The current tile.</param>
    /// <returns>A list of neighbouring tiles.</returns>
    private static List<OverlayTile> GetNeightbourOverlayTiles(OverlayTile currentOverlayTile)
    {
        var map = MapManager.Instance.Map;

        var neighbours = new List<OverlayTile>();

        // Right
        var locationToCheck = new Vector2Int(
            currentOverlayTile.GridLocation.x + 1,
            currentOverlayTile.GridLocation.y
        );

        if (map.TryGetValue(locationToCheck, out var right))
        {
            neighbours.Add(right);
        }

        // Left
        locationToCheck = new Vector2Int(
            currentOverlayTile.GridLocation.x - 1,
            currentOverlayTile.GridLocation.y
        );

        if (map.TryGetValue(locationToCheck, out var left))
        {
            neighbours.Add(left);
        }

        // Top
        locationToCheck = new Vector2Int(
            currentOverlayTile.GridLocation.x,
            currentOverlayTile.GridLocation.y + 1
        );

        if (map.TryGetValue(locationToCheck, out var top))
        {
            neighbours.Add(top);
        }

        // Bottom
        locationToCheck = new Vector2Int(
            currentOverlayTile.GridLocation.x,
            currentOverlayTile.GridLocation.y - 1
        );

        if (map.TryGetValue(locationToCheck, out var bottom))
        {
            neighbours.Add(bottom);
        }

        return neighbours;
    }
}