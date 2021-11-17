using System.Collections.Generic;
using UnityEngine;

public class BoardCoordinates
{
    private readonly List<BoardLocation> _boardPoints = new List<BoardLocation>();
    private readonly int _maxX;
    private readonly int _maxY;

    public BoardCoordinates()
    {
        var maxes = GameObject.Find("SceneInfo");
        _maxX = maxes.GetComponent<SceneInfo>().boardMaxX;
        _maxY = maxes.GetComponent<SceneInfo>().boardMaxY;
        FillBoard();
    }
    
    public bool IsValidBoardPoint(Vector2 position)
    {
        var boardLocation = _boardPoints.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }
    
    private void FillBoard()
    {
        for (var x = _maxX * -1; x <= _maxX; x++)
        {
            for (var y = _maxY * -1; y <= _maxY; y++)
            {
                var boardLocation = new BoardLocation($"BoardPoint ({x},{y})", "BoardPoint", new Vector2(x, y));
                _boardPoints.Add(boardLocation);
            }
        }
    }
}

