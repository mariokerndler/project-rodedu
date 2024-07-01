using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder
{
    public List<OverlayTile> GetTilesInRange(Vector2Int location, int range)
    {
        var startingTile = MapManager.Instance.map[location];
        var inRangeTiles = new List<OverlayTile>();
        var stepCount = 0;

        inRangeTiles.Add(startingTile);
        
        var tilesForPreviousStep = new List<OverlayTile> { startingTile };
        while (stepCount < range)
        {
            var surroundingTiles = new List<OverlayTile>();

            foreach (var item in tilesForPreviousStep)
            {
                surroundingTiles.AddRange(MapManager.Instance.GetSurroundingTiles(new Vector2Int(item.gridLocation.x, item.gridLocation.y)));
            }

            inRangeTiles.AddRange(surroundingTiles);
            tilesForPreviousStep = surroundingTiles.Distinct().ToList();
            stepCount++;
        }

        return inRangeTiles.Distinct().ToList();
    }
}