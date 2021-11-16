using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class BoardCoordinates
{
    private readonly List<BoardLocation> _boardPoints = new List<BoardLocation>();

    public BoardCoordinates()
    {
        FillBoard();
    }
    
    public bool IsValidBoardPoint(Vector2 position)
    {
        var boardLocation = _boardPoints.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }
    
    private void FillBoard()
    {
        const int maxX = 9;
        const int maxY = 5;
        for (var x = maxX * -1; x <= maxX; x++)
        {
            for (var y = maxY * -1; y <= maxY; y++)
            {
                var boardLocation = new BoardLocation($"BoardPoint ({x},{y})", "BoardPoint", new Vector2(x, y));
                _boardPoints.Add(boardLocation);
            }
        }
    }
}

