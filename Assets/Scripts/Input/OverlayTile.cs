using System.Collections.Generic;
using UnityEngine;
using static ArrowTranslator;

/// <summary>
/// Represents a tile in the overlay grid. This class manages the tile's state,
/// including its display properties and pathfinding-related values.
/// </summary>
public class OverlayTile : MonoBehaviour
{
    public int G { get; set; } // Movement cost from the start node to this tile.
    public int H { get; set; } // Estimated movement cost from this tile to the end node.
    public int F => G + H; // Total cost function (F = G + H) for A* pathfinding.

    public bool isBlocked = false;
    public OverlayTile Previous { get; set; }
    public Vector3Int GridLocation { get; set; }
    public Vector2Int Grid2DLocation => new(GridLocation.x, GridLocation.y);
    public List<Sprite> arrows;

    private SpriteRenderer _mainSpriteRenderer;
    private SpriteRenderer _childSpriteRenderer;

    private void Awake()
    {
        _mainSpriteRenderer = GetComponent<SpriteRenderer>();
        _childSpriteRenderer = GetComponentsInChildren<SpriteRenderer>()[1];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //HideTile();
        }
    }
    
    /// <summary>
    /// Hides the tile by setting its color to fully transparent.
    /// </summary>
    public void HideTile()
    {
        if (_mainSpriteRenderer)
        {
            _mainSpriteRenderer.color = new Color(1, 1, 1, 0);
        }
    }

    /// <summary>
    /// Shows the tile by setting its color to fully opaque.
    /// </summary>
    public void ShowTile()
    {
        if (_mainSpriteRenderer)
        {
            _mainSpriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }
    
    /// <summary>
    /// Sets the sprite of the child SpriteRenderer based on the given direction.
    /// </summary>
    /// <param name="d">The direction to set the arrow sprite.</param>
    public void SetSprite(ArrowDirection d)
    {
        if (!_childSpriteRenderer) return;
        
        if (d == ArrowDirection.None)
        {
            _childSpriteRenderer.color = new Color(1, 1, 1, 0);
        }
        else
        {
            _childSpriteRenderer.color = new Color(1, 1, 1, 1);
            _childSpriteRenderer.sprite = arrows[(int)d];
            _childSpriteRenderer.sortingOrder = _mainSpriteRenderer ? _mainSpriteRenderer.sortingOrder : 0;
        }
    }
}
