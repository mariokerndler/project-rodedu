using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ArrowTranslator;

/// <summary>
/// Handles mouse interactions for character movement and tile selection.
/// </summary>
public class MouseController : MonoBehaviour
{
    public GameObject cursor;
    public float speed;
    public int range;
    public GameObject characterPrefab;
    
    private CharacterInfo _character;
    private PathFinder _pathFinder;
    private List<OverlayTile> _path;
    private List<OverlayTile> _rangeFinderTiles;
    private bool _isMoving;

    private void Start()
    {
        _pathFinder = new PathFinder();
        _path = new List<OverlayTile>();
        _isMoving = false;
        _rangeFinderTiles = new List<OverlayTile>();
    }

    private void LateUpdate()
    {
        var hit = GetFocusedOnTile();

        if (hit.HasValue)
        {
            var tile = hit.Value.collider.GetComponent<OverlayTile>();
            UpdateCursor(tile);

            if (_rangeFinderTiles.Contains(tile) && !_isMoving)
            {
                UpdatePath(tile);
                UpdateTileArrows();
            }

            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick(tile);
            }
        }

        if (_isMoving && _path.Count > 0)
        {
            MoveAlongPath();
        }
    }
    
    /// <summary>
    /// Updates the cursor position and sorting order.
    /// </summary>
    /// <param name="tile">The tile under the cursor.</param>
    private void UpdateCursor(OverlayTile tile)
    {
        cursor.transform.position = tile.transform.position;
        cursor.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
    }
    
    /// <summary>
    /// Updates the path to the selected tile.
    /// </summary>
    /// <param name="tile">The target tile.</param>
    private void UpdatePath(OverlayTile tile)
    {
        _path = _pathFinder.FindPath(_character.standingOnTile, tile, _rangeFinderTiles);
    }
    
    /// <summary>
    /// Updates the arrows on the tiles along the path.
    /// </summary>
    private void UpdateTileArrows()
    {
        foreach (var item in _rangeFinderTiles)
        {
            MapManager.Instance.Map[item.Grid2DLocation].SetSprite(ArrowDirection.None);
        }

        for (var i = 0; i < _path.Count; i++)
        {
            var previousTile = i > 0 ? _path[i - 1] : _character.standingOnTile;
            var futureTile = i < _path.Count - 1 ? _path[i + 1] : null;

            var arrow = TranslateDirection(previousTile, _path[i], futureTile);
            _path[i].SetSprite(arrow);
        }
    }
    
    /// <summary>
    /// Handles the mouse click event.
    /// </summary>
    /// <param name="tile">The tile that was clicked.</param>
    private void HandleMouseClick(OverlayTile tile)
    {
        tile.ShowTile();

        if (_character == null)
        {
            _character = Instantiate(characterPrefab).GetComponent<CharacterInfo>();
            PositionCharacterOnTile(tile);
            GetInRangeTiles();
        }
        else
        {
            _isMoving = true;
            tile.HideTile();
        }
    }
    
    /// <summary>
    /// Moves the character along the path.
    /// </summary>
    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;
        var targetPosition = _path[0].transform.position;
        _character.transform.position = Vector2.MoveTowards(_character.transform.position, targetPosition, step);
        _character.transform.position = new Vector3(_character.transform.position.x, _character.transform.position.y, targetPosition.z);

        if (Vector2.Distance(_character.transform.position, targetPosition) < 0.00001f)
        {
            PositionCharacterOnTile(_path[0]);
            _path.RemoveAt(0);
        }

        if (_path.Count != 0) return;
        
        GetInRangeTiles();
        _isMoving = false;
    }

    /// <summary>
    /// Positions the character on the specified tile.
    /// </summary>
    /// <param name="tile">The tile to position the character on.</param>
    private void PositionCharacterOnTile(OverlayTile tile)
    {
        _character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
        _character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        _character.standingOnTile = tile;
    }
    
    /// <summary>
    /// Gets the tiles within range of the character.
    /// </summary>
    private void GetInRangeTiles()
    {
        _rangeFinderTiles = RangeFinder.GetTilesInRange(new Vector2Int(_character.standingOnTile.GridLocation.x, _character.standingOnTile.GridLocation.y), range);

        foreach (var item in _rangeFinderTiles)
        {
            item.ShowTile();
        }
    }
    
    /// <summary>
    /// Gets the tile that the mouse is currently over.
    /// </summary>
    /// <returns>The tile that the mouse is currently over, or null if no tile is found.</returns>
    private static RaycastHit2D? GetFocusedOnTile()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mousePos2d = new Vector2(mousePos.x, mousePos.y);

        var hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }
}
