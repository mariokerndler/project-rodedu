using System.Collections.Generic;
using UnityEngine;
using static ArrowTranslator;

public class OverlayTile : MonoBehaviour
{
    [HideInInspector] public int G;
    [HideInInspector] public int H;
    public int F => G + H;

    [HideInInspector] public bool isBlocked = false;
    [HideInInspector] public OverlayTile Previous;
    [HideInInspector] public Vector3Int gridLocation;
    public Vector2Int grid2DLocation => new(gridLocation.x, gridLocation.y);
    public List<Sprite> arrows;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTile();
        }
    }
    
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }
    
    public void SetSprite(ArrowDirection d)
    {
        if (d == ArrowDirection.None)
            GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(1, 1, 1, 0);
        else
        {
            GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(1, 1, 1, 1);
            GetComponentsInChildren<SpriteRenderer>()[1].sprite = arrows[(int)d];
            GetComponentsInChildren<SpriteRenderer>()[1].sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        }
    }
}
