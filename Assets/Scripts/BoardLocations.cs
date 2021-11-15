using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardLocations
{
    private List<BoardLocation> _gameObjectLocations = new List<BoardLocation>();
    private List<BoardLocation> _boardLocations = new List<BoardLocation>();
    private List<BoardLocation> _rangeLocations = new List<BoardLocation>();
    
    public List<BoardLocation> GetBoardLocations(bool noFill = false)
    {
        if (noFill && _gameObjectLocations != null) return _gameObjectLocations;
        _gameObjectLocations = new List<BoardLocation>();
        FillGameObjects();
        return _gameObjectLocations;
    }

    public List<BoardLocation> GetFullBoard()
    {
        _boardLocations = new List<BoardLocation>();
        FillBoard();
        return _boardLocations; 
    }

    public List<BoardLocation> GetGrid()
    {
        _rangeLocations = new List<BoardLocation>();
        SetGrid();
        return _rangeLocations;
    }

    public bool CheckGameObjects(string findTag, Vector2 position)
    {
        var boardLocation = _gameObjectLocations.Find(brdLoc => brdLoc.Tag == findTag && brdLoc.Location == position);
        return boardLocation is { };
    }
    
    public bool CheckGrid(Vector2 position)
    {
        var boardLocation = _rangeLocations.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }

    public bool CheckBoard(Vector2 position)
    {
        var boardLocation = _boardLocations.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }

    private void FillGameObjects()
    {
        var allGameObjectLocations = GameObject.FindGameObjectsWithTag("CircleCollider");
        foreach (var gO in allGameObjectLocations)
        {
            using var boardLocation = new BoardLocation(gO.name, gO.tag, gO.transform.position);
            _gameObjectLocations.Add(boardLocation);
        }
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
                _boardLocations.Add(boardLocation);
            }
        }
    }
    
    private void SetGrid()
    {
        ResetGridPoints();
        var player = GameObject.Find("Bird Token Grey");
        var currentPos = player.transform.position;
        var range = player.GetComponent<PlayerController>().range;
        Debug.Log($"Base Location for Player in Grid {currentPos.x}, {currentPos.y} with Range of {range}");
        for (var x = range * -1; x <= range; x++)
        {
            for (var y = range * -1; y <= range; y++)
            {
                var totalDistance = Math.Abs(x) + Math.Abs(y);
                if (totalDistance > range) continue;
                var newPos = new Vector2(x + currentPos.x, y + currentPos.y);
                var boardLocation = new BoardLocation($"GridPoint ({newPos.x},{newPos.y})", "GridPoint", newPos);
                _rangeLocations.Add(boardLocation);
                var gridPointType = totalDistance switch
                {
                    0 => "GridPoint Home",
                    1 => "GridPoint Green",
                    2 => "GridPoint Blue",
                    3 => "GridPoint Red",
                    _ => "GridPoint Home"
                };
                InitGridPoint(gridPointType, newPos);
            }
        }
    }

    private static void InitGridPoint(string gpType, Vector2 position)
    {
        var requestedGo = GameObject.Find(gpType);
        BoardActions.Init(requestedGo, position);
    }

    private static void ResetGridPoints()
    {
        var allGridPoints = GameObject.FindGameObjectsWithTag("RangePoint");
        foreach (var gridPoint in allGridPoints)
        {
            BoardActions.Remove(gridPoint);
        }
    }
}

