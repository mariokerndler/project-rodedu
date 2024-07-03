using UnityEngine;

public static class ArrowTranslator
{
    public enum ArrowDirection
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        TopRight = 5,
        BottomRight = 6,
        TopLeft = 7,
        BottomLeft = 8,
        UpFinished = 9,
        DownFinished = 10,
        LeftFinished = 11,
        RightFinished = 12
    }
    
    public static ArrowDirection TranslateDirection(OverlayTile previousTile, OverlayTile currentTile, OverlayTile futureTile)
        {
            var isFinal = futureTile == null;

            var pastDirection = previousTile != null ? (Vector2Int)(currentTile.GridLocation - previousTile.GridLocation) : new Vector2Int(0, 0);
            var futureDirection = futureTile != null ? (Vector2Int)(futureTile.GridLocation - currentTile.GridLocation) : new Vector2Int(0, 0);
            var direction = pastDirection != futureDirection ? pastDirection + futureDirection : futureDirection;

            if (direction == new Vector2(0, 1) && !isFinal)
            {
                return ArrowDirection.Up;
            }

            if (direction == new Vector2(0, -1) && !isFinal)
            {
                return ArrowDirection.Down;
            }

            if (direction == new Vector2(1, 0) && !isFinal)
            {
                return ArrowDirection.Right;
            }

            if (direction == new Vector2(-1, 0) && !isFinal)
            {
                return ArrowDirection.Left;
            }

            if (direction == new Vector2(1, 1))
            {
                return pastDirection.y < futureDirection.y ? ArrowDirection.BottomLeft : ArrowDirection.TopRight;
            }

            if (direction == new Vector2(-1, 1))
            {
                return pastDirection.y < futureDirection.y ? ArrowDirection.BottomRight : ArrowDirection.TopLeft;
            }

            if (direction == new Vector2(1, -1))
            {
                return pastDirection.y > futureDirection.y ? ArrowDirection.TopLeft : ArrowDirection.BottomRight;
            }

            if (direction == new Vector2(-1, -1))
            {
                return pastDirection.y > futureDirection.y ? ArrowDirection.TopRight : ArrowDirection.BottomLeft;
            }

            if (direction == new Vector2(0, 1) && isFinal)
            {
                return ArrowDirection.UpFinished;
            }

            if (direction == new Vector2(0, -1) && isFinal)
            {
                return ArrowDirection.DownFinished;
            }

            if (direction == new Vector2(-1, 0) && isFinal)
            {
                return ArrowDirection.LeftFinished;
            }

            if (direction == new Vector2(1, 0) && isFinal)
            {
                return ArrowDirection.RightFinished;
            }

            return ArrowDirection.None;
        }
}